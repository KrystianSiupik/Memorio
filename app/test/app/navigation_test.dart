import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/app/router.dart';
import 'package:memorio/app/routes.dart';
import 'package:memorio/features/account/account.dart';

import '../helpers/app.dart';

void main() {
  group('S02 tabs', () {
    testWidgets('a signed-in user starts on the map with the four tabs', (tester) async {
      await pumpMemorioApp(tester, session: signedIn);

      final bar = find.byType(NavigationBar);
      for (final label in ['Map', 'Albums', 'Shared with me', 'Account']) {
        expect(find.descendant(of: bar, matching: find.text(label)), findsOneWidget);
      }
      expect(find.widgetWithText(AppBar, 'Map'), findsOneWidget);
    });

    testWidgets('each tab opens its screen', (tester) async {
      await pumpMemorioApp(tester, session: signedIn);

      for (final label in ['Albums', 'Shared with me', 'Account', 'Map']) {
        await tester.tap(find.descendant(of: find.byType(NavigationBar), matching: find.text(label)));
        await tester.pumpAndSettle();
        expect(find.widgetWithText(AppBar, label), findsOneWidget, reason: label);
      }
    });

    testWidgets('a typed route opens its tab', (tester) async {
      final container = await pumpMemorioApp(tester, session: signedIn);

      container.read(routerProvider).go(const AccountRoute().location);
      await tester.pumpAndSettle();

      expect(find.widgetWithText(AppBar, 'Account'), findsOneWidget);
    });
  });

  group('signed-out users (S02 forbidden → S01)', () {
    testWidgets('start on sign-in without the tabs', (tester) async {
      await pumpMemorioApp(tester);

      expect(find.widgetWithText(AppBar, 'Sign in'), findsOneWidget);
      expect(find.byType(NavigationBar), findsNothing);
    });

    testWidgets('cannot open a tab directly', (tester) async {
      final container = await pumpMemorioApp(tester);

      container.read(routerProvider).go(const AlbumsRoute().location);
      await tester.pumpAndSettle();

      expect(find.widgetWithText(AppBar, 'Sign in'), findsOneWidget);
    });
  });

  group('redirectFor', () {
    const loading = AsyncLoading<Session?>();
    const signedOut = AsyncData<Session?>(null);
    const signedInValue = AsyncData<Session?>(signedIn);

    String? redirect(AsyncValue<Session?> session, String location) =>
        redirectFor(session: session, uri: Uri.parse(location));

    test('keeps the requested location while the session is restored', () {
      expect(redirect(loading, '/albums'), '/?from=%2Falbums');
      expect(redirect(loading, startPath), isNull);
    });

    test('sends signed-out users to sign-in, also after a failed restore', () {
      expect(redirect(signedOut, '/map'), SignInRoute.path);
      expect(redirect(signedOut, startPath), SignInRoute.path);
      expect(redirect(signedOut, SignInRoute.path), isNull);
      expect(redirect(AsyncError<Session?>(Exception('storage'), StackTrace.empty), '/map'), SignInRoute.path);
    });

    test('sends signed-in users to the requested location or the map', () {
      expect(redirect(signedInValue, '/?from=%2Falbums'), '/albums');
      expect(redirect(signedInValue, startPath), MapRoute.path);
      expect(redirect(signedInValue, SignInRoute.path), MapRoute.path);
      expect(redirect(signedInValue, '/?from=%2Fsign-in'), MapRoute.path);
      expect(redirect(signedInValue, '/shared'), isNull);
    });
  });

  test('tab routes have distinct paths in navigation bar order', () {
    expect(TabRoute.all.map((route) => route.location), ['/map', '/albums', '/shared', '/account']);
  });

  test('GoRouter knows every typed route', () {
    final container = ProviderContainer(
      overrides: [sessionRepositoryProvider.overrideWithValue(const FakeSessionRepository(signedIn))],
    );
    addTearDown(container.dispose);
    final router = container.read(routerProvider);

    for (final route in [const SignInRoute(), ...TabRoute.all]) {
      expect(router.configuration.findMatch(Uri.parse(route.location)).matches, isNotEmpty, reason: route.location);
    }
  });
}
