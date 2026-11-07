import 'dart:convert';
import 'dart:io';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/app/app_theme.dart';
import 'package:memorio/app/router.dart';
import 'package:memorio/app/routes.dart';
import 'package:memorio/core/settings/settings.dart';

import '../../helpers/app.dart';
import '../../helpers/fake_memories.dart';

void main() {
  Future<void> open(WidgetTester tester, InMemorySettingsStore store, String location) async {
    tester.view.physicalSize = const Size(1000, 2000);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    final container = await pumpMemorioApp(tester, session: signedIn, settings: store);
    container.read(routerProvider).go(location);
    await tester.pumpAndSettle();
  }

  testWidgets('S24 → settings: choosing English changes the app at once and is remembered', (tester) async {
    final store = InMemorySettingsStore();
    await open(tester, store, const AccountRoute().location);
    await tester.tap(find.byTooltip('Ustawienia'));
    await tester.pumpAndSettle();
    expect(find.text('Język'), findsOneWidget);

    await tester.tap(find.byKey(const Key('settings-language-english')));
    await tester.pumpAndSettle();

    expect(find.text('Language'), findsOneWidget);
    expect(find.text('Map'), findsWidgets);
    expect((jsonDecode(store.value!) as Map<String, Object?>)['language'], 'english');
  });

  testWidgets('what the device remembers applies from the start', (tester) async {
    await open(tester, InMemorySettingsStore(jsonEncode({'language': 'english'})), const AlbumsRoute().location);

    expect(find.text('Albums'), findsWidgets);
    expect(find.text('Albumy'), findsNothing);
  });

  testWidgets('as on the device: Polish here', (tester) async {
    await open(tester, InMemorySettingsStore(jsonEncode({'language': 'system'})), const SettingsRoute().location);

    expect(find.text('Jak w urządzeniu'), findsNWidgets(2));
    expect(find.text('Polski'), findsOneWidget);
    expect(find.text('English'), findsOneWidget);
  });

  testWidgets('the dark look applies at once with the gold accent, and is remembered', (tester) async {
    final store = InMemorySettingsStore();
    await open(tester, store, const SettingsRoute().location);
    expect(Theme.of(tester.element(find.text('Wygląd'))).brightness, Brightness.light);

    await tester.tap(find.text('Ciemny'));
    await tester.pumpAndSettle();

    final theme = Theme.of(tester.element(find.text('Wygląd')));
    expect(theme.brightness, Brightness.dark);
    expect(theme.colorScheme.primary, AppThemes.darkAccent);
    expect((jsonDecode(store.value!) as Map<String, Object?>)['theme'], 'dark');
  });

  testWidgets('a remembered light look stays light on a dark device', (tester) async {
    tester.platformDispatcher.platformBrightnessTestValue = Brightness.dark;
    addTearDown(tester.platformDispatcher.clearPlatformBrightnessTestValue);
    await open(tester, InMemorySettingsStore(jsonEncode({'theme': 'light'})), const SettingsRoute().location);

    expect(Theme.of(tester.element(find.text('Wygląd'))).colorScheme.primary, AppThemes.lightAccent);
  });

  testWidgets('points: dots by default; the switch and the size show in the picture and are remembered', (
    tester,
  ) async {
    final store = InMemorySettingsStore();
    await open(tester, store, const SettingsRoute().location);

    expect(find.text('Zdjęcia zamiast kropek'), findsOneWidget);
    expect(tester.widget<SwitchListTile>(find.byKey(const Key('settings-photo-points'))).value, isFalse);
    final dotBefore = tester.getSize(find.byKey(const Key('settings-preview-dot')));

    await tester.tap(find.byKey(const Key('settings-photo-points')));
    await tester.pumpAndSettle();
    await tester.drag(find.byKey(const Key('settings-point-size')), const Offset(400, 0));
    await tester.pumpAndSettle();

    final saved = jsonDecode(store.value!) as Map<String, Object?>;
    expect(saved['photoPoints'], isTrue);
    expect(saved['pointScale'], AppSettings.maxPointScale);
    expect(tester.getSize(find.byKey(const Key('settings-preview-dot'))).width, greaterThan(dotBefore.width));
    expect(tester.getSize(find.byKey(const Key('settings-preview-photo'))).width, 29 * AppSettings.maxPointScale);
  });

  testWidgets('with photo points on, the map asks for the photos', (tester) async {
    final memories = FakeMemoriesRepository();
    tester.view.physicalSize = const Size(1000, 2000);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    await pumpMemorioApp(
      tester,
      session: signedIn,
      memories: memories,
      settings: InMemorySettingsStore(jsonEncode({'photoPoints': true})),
    );

    expect(memories.pinPhotoRequests.last, isTrue);
  });

  test('settings that cannot be read fall back value by value, and the point size stays in range', () {
    final read = AppSettings.fromJson({'language': 'klingon', 'theme': 'dark', 'photoPoints': 'yes', 'pointScale': 9});

    expect(read.language, AppLanguage.system);
    expect(read.theme, AppTheme.dark);
    expect(read.photoPoints, isFalse);
    expect(read.pointScale, AppSettings.maxPointScale);
  });

  test('every text has its English translation', () {
    Set<String> keys(String file) =>
        (jsonDecode(File('lib/core/l10n/$file').readAsStringSync()) as Map<String, Object?>).keys
            .where((key) => !key.startsWith('@'))
            .toSet();

    final polish = keys('app_pl.arb');
    final english = keys('app_en.arb');

    expect(polish.difference(english), isEmpty, reason: 'missing in English');
    expect(english.difference(polish), isEmpty, reason: 'only in English');
  });
}
