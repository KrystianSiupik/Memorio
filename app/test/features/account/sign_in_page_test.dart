import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/core/result.dart';
import 'package:memorio/features/account/account.dart';

import '../../helpers/app.dart';
import '../../helpers/fake_account.dart';

void main() {
  late MutableSessionRepository sessions;
  late FakeAuthRepository auth;
  late FakeAccountRepository account;

  setUp(() {
    sessions = MutableSessionRepository();
    auth = FakeAuthRepository(sessions: sessions);
    account = FakeAccountRepository();
  });

  Future<void> pump(WidgetTester tester, {Stream<bool>? online, AuthRepository? authRepository}) => pumpMemorioApp(
    tester,
    sessions: sessions,
    account: account,
    online: online,
    overrides: [authRepositoryProvider.overrideWithValue(authRepository ?? auth)],
  );

  Finder submit(String label) => find.widgetWithText(FilledButton, label);

  bool isEnabled(WidgetTester tester, Finder button) => tester.widget<FilledButton>(button).onPressed != null;

  Future<void> fill(WidgetTester tester) async {
    await tester.enterText(find.widgetWithText(TextField, 'E-mail'), 'ola@example.com');
    await tester.enterText(find.widgetWithText(TextField, 'Hasło'), 'long-enough-password');
  }

  Future<void> tapText(WidgetTester tester, String text) async {
    await tester.tap(find.text(text));
    await tester.pump();
  }

  testWidgets('shows the fields and creating an account waits for the 16+ declaration', (tester) async {
    await pump(tester);

    for (final label in ['E-mail', 'Hasło', 'Mam ukończone 16 lat', 'Akceptuję regulamin']) {
      expect(find.text(label), findsOneWidget, reason: label);
    }
    expect(isEnabled(tester, submit('Załóż konto')), isFalse);

    await tapText(tester, 'Mam ukończone 16 lat');

    expect(isEnabled(tester, submit('Załóż konto')), isTrue);
  });

  testWidgets('S01 → S03: registering with the terms records both and opens the map', (tester) async {
    await pump(tester);
    await fill(tester);
    await tapText(tester, 'Mam ukończone 16 lat');
    await tapText(tester, 'Akceptuję regulamin');

    await tester.tap(submit('Załóż konto'));
    await tester.pumpAndSettle();

    expect(auth.registered.single.declaresMinimumAge, isTrue);
    expect(auth.registered.single.email, 'ola@example.com');
    expect(account.termsAccepted, 1);
    expect(find.widgetWithText(AppBar, 'Mapa'), findsOneWidget);
  });

  testWidgets('registering without the terms does not record them', (tester) async {
    await pump(tester);
    await fill(tester);
    await tapText(tester, 'Mam ukończone 16 lat');

    await tester.tap(submit('Załóż konto'));
    await tester.pumpAndSettle();

    expect(account.termsAccepted, 0);
  });

  testWidgets('S01 → S03: someone with an account signs in', (tester) async {
    await pump(tester);
    await tapText(tester, 'Mam już konto');
    await fill(tester);

    expect(find.text('Mam ukończone 16 lat'), findsNothing);
    await tester.tap(submit('Zaloguj się'));
    await tester.pumpAndSettle();

    expect(auth.signedIn.single.email, 'ola@example.com');
    expect(find.widgetWithText(AppBar, 'Mapa'), findsOneWidget);
  });

  testWidgets('loading: the actions are inactive until the answer comes', (tester) async {
    final gate = Completer<void>();
    await pump(tester, authRepository: _WaitingAuth(gate.future));
    await fill(tester);
    await tapText(tester, 'Mam ukończone 16 lat');

    await tester.tap(submit('Załóż konto'));
    await tester.pump();

    expect(find.byKey(const Key('s01-loading')), findsOneWidget);
    expect(isEnabled(tester, submit('Załóż konto')), isFalse);

    gate.complete();
    await tester.pumpAndSettle();
  });

  group('error: the message depends on the failure, the input stays and it can be retried', () {
    const messages = <AuthFailure, String>{
      AuthOffline(): 'Brak połączenia. Sprawdź internet i spróbuj ponownie.',
      EmailTaken(): 'Konto z tym adresem już istnieje. Zaloguj się.',
      InvalidPassword(): 'Hasło musi mieć co najmniej 10 znaków.',
      InvalidEmail(): 'To nie wygląda na poprawny adres e-mail.',
      InvalidCredentials(): 'Nieprawidłowy e-mail lub hasło.',
      AuthServerError(): 'Błąd serwera. Spróbuj ponownie za chwilę.',
    };

    messages.forEach((failure, message) {
      testWidgets(message, (tester) async {
        auth.failure = failure;
        await pump(tester);
        await fill(tester);
        await tapText(tester, 'Mam ukończone 16 lat');

        await tester.tap(submit('Załóż konto'));
        await tester.pumpAndSettle();

        expect(find.text(message), findsOneWidget);
        expect(find.text('ola@example.com'), findsOneWidget);

        auth.failure = null;
        await tester.tap(find.text('Ponów'));
        await tester.pumpAndSettle();

        expect(auth.registered, hasLength(2));
        expect(find.widgetWithText(AppBar, 'Mapa'), findsOneWidget);
      });
    });
  });

  testWidgets('offline: the banner shows and creating an account is off', (tester) async {
    await pump(tester, online: Stream.value(false));
    await tapText(tester, 'Mam ukończone 16 lat');

    expect(find.text('Brak połączenia'), findsOneWidget);
    expect(isEnabled(tester, submit('Załóż konto')), isFalse);
  });
}

class _WaitingAuth extends FakeAuthRepository {
  _WaitingAuth(this.gate);

  final Future<void> gate;

  @override
  Future<Result<Session, AuthFailure>> register({
    required String email,
    required String password,
    required bool declaresMinimumAge,
  }) async {
    await gate;
    return const Err(AuthServerError());
  }
}
