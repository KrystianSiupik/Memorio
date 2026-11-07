import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../core/api/api.dart';
import '../core/l10n/l10n.dart';
import '../core/settings/settings.dart';
import 'router.dart';
import 'update_required_screen.dart';

/// Root widget. `app/` is the composition root: the only place that knows every feature
/// and wires implementations (provider overrides) to the interfaces features depend on.
class MemorioApp extends ConsumerWidget {
  const MemorioApp({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    ref.watch(appVersionCheckProvider);
    final minimumVersion = ref.watch(updateRequiredProvider);
    final settings = ref.watch(settingsProvider);

    return MaterialApp.router(
      title: 'Memorio',
      theme: ThemeData(colorSchemeSeed: Colors.teal, useMaterial3: true),
      localizationsDelegates: AppLocalizations.localizationsDelegates,
      supportedLocales: AppLocalizations.supportedLocales,
      // Null: the device's language, or Polish when the app does not speak it.
      locale: settings.language.locale,
      // A device language the app does not speak gets Polish, the language the product starts in.
      localeResolutionCallback: (device, supported) => supported.firstWhere(
        (locale) => locale.languageCode == device?.languageCode,
        orElse: () => const Locale('pl'),
      ),
      routerConfig: ref.watch(routerProvider),
      builder: (context, child) => minimumVersion == null
          ? child ?? const SizedBox.shrink()
          : UpdateRequiredScreen(minimumVersion: minimumVersion),
    );
  }
}
