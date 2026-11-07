import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

/// MEM-E13-PLAT-01: the language of the app; by default the device's.
enum AppLanguage {
  system(null),
  polish('pl'),
  english('en');

  const AppLanguage(this.code);

  final String? code;

  Locale? get locale => code == null ? null : Locale(code!);
}

/// MEM-E13-PLAT-02: light, dark, or as the device is set.
enum AppTheme {
  system(ThemeMode.system),
  light(ThemeMode.light),
  dark(ThemeMode.dark);

  const AppTheme(this.mode);

  final ThemeMode mode;
}

/// What the person chose on this device (E13). Nothing of it goes to the server.
@immutable
class AppSettings {
  const AppSettings({
    this.language = AppLanguage.system,
    this.theme = AppTheme.system,
    this.photoPoints = false,
    this.pointScale = 1.0,
  });

  static const minPointScale = 1.0;
  static const maxPointScale = 2.0;

  final AppLanguage language;
  final AppTheme theme;

  /// MEM-E13-PLAT-05: memories on the map as circles with their first photo; off by default.
  final bool photoPoints;

  /// MEM-E13-PLAT-05: how much bigger the points are, from 1 to 2.
  final double pointScale;

  AppSettings copyWith({AppLanguage? language, AppTheme? theme, bool? photoPoints, double? pointScale}) => AppSettings(
    language: language ?? this.language,
    theme: theme ?? this.theme,
    photoPoints: photoPoints ?? this.photoPoints,
    pointScale: (pointScale ?? this.pointScale).clamp(minPointScale, maxPointScale),
  );

  Map<String, Object?> toJson() => {
    'language': language.name,
    'theme': theme.name,
    'photoPoints': photoPoints,
    'pointScale': pointScale,
  };

  /// Whatever cannot be read falls back to the default, value by value.
  static AppSettings fromJson(Map<String, Object?> json) => const AppSettings().copyWith(
    language: AppLanguage.values.asNameMap()[json['language']],
    theme: AppTheme.values.asNameMap()[json['theme']],
    photoPoints: json['photoPoints'] is bool ? json['photoPoints']! as bool : null,
    pointScale: json['pointScale'] is num ? (json['pointScale']! as num).toDouble() : null,
  );

  @override
  bool operator ==(Object other) =>
      other is AppSettings &&
      other.language == language &&
      other.theme == theme &&
      other.photoPoints == photoPoints &&
      other.pointScale == pointScale;

  @override
  int get hashCode => Object.hash(language, theme, photoPoints, pointScale);
}

abstract interface class SettingsStore {
  Future<String?> read();

  Future<void> write(String value);
}

/// On the device, next to the session (Keychain, Keystore, encrypted browser storage).
class SecureSettingsStore implements SettingsStore {
  const SecureSettingsStore([this._storage = const FlutterSecureStorage()]);

  static const _key = 'memorio.settings';

  final FlutterSecureStorage _storage;

  @override
  Future<String?> read() => _storage.read(key: _key);

  @override
  Future<void> write(String value) => _storage.write(key: _key, value: value);
}

/// For tests, and the default until app/ wires the real one.
class InMemorySettingsStore implements SettingsStore {
  InMemorySettingsStore([this.value]);

  String? value;

  @override
  Future<String?> read() async => value;

  @override
  Future<void> write(String value) async => this.value = value;
}

/// Overridden in app/ with [SecureSettingsStore].
final settingsStoreProvider = Provider<SettingsStore>((ref) => InMemorySettingsStore());

final settingsProvider = NotifierProvider<SettingsNotifier, AppSettings>(SettingsNotifier.new);

/// The defaults at once, then what the device remembers: the app never waits for the storage to start.
class SettingsNotifier extends Notifier<AppSettings> {
  var _changed = false;

  @override
  AppSettings build() {
    Future.microtask(_load);
    return const AppSettings();
  }

  Future<void> _load() async {
    final stored = await ref.read(settingsStoreProvider).read();
    if (stored == null || _changed || !ref.mounted) {
      return;
    }
    try {
      state = AppSettings.fromJson(jsonDecode(stored) as Map<String, Object?>);
    } on Object {
      // Unreadable: the defaults stay, and the next change writes a good copy.
    }
  }

  Future<void> update(AppSettings Function(AppSettings current) change) async {
    _changed = true;
    state = change(state);
    await ref.read(settingsStoreProvider).write(jsonEncode(state.toJson()));
  }
}
