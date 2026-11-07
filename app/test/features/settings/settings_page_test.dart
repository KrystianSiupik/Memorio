import 'dart:convert';
import 'dart:io';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/app/router.dart';
import 'package:memorio/app/routes.dart';
import 'package:memorio/core/settings/settings.dart';

import '../../helpers/app.dart';

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

    expect(find.text('Jak w urządzeniu'), findsOneWidget);
    expect(find.text('Polski'), findsOneWidget);
    expect(find.text('English'), findsOneWidget);
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
