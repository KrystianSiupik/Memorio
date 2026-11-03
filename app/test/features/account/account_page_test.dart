import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/app/router.dart';
import 'package:memorio/app/routes.dart';
import 'package:memorio/core/result.dart';
import 'package:memorio/features/account/account.dart';

import '../../helpers/app.dart';
import '../../helpers/fake_account.dart';

void main() {
  late FakeAccountRepository account;

  setUp(() => account = FakeAccountRepository());

  Future<void> open(WidgetTester tester, {Stream<bool>? online, AccountRepository? repository}) async {
    // Tall enough to build the whole list, also under the offline banners.
    tester.view.physicalSize = const Size(1000, 2400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    final container = await pumpMemorioApp(tester, session: signedIn, account: repository ?? account, online: online);
    container.read(routerProvider).go(const AccountRoute().location);
    await tester.pump();
    await tester.pump();
  }

  Future<void> tapText(WidgetTester tester, String text) async {
    await tester.ensureVisible(find.text(text));
    await tester.pump();
    await tester.tap(find.text(text));
    await tester.pumpAndSettle();
  }

  bool isEnabled(WidgetTester tester, String label) =>
      tester.widget<FilledButton>(find.widgetWithText(FilledButton, label)).onPressed != null;

  const withoutSuccessor = AccountOverview(
    impact: DeletionImpact(memories: 1, photos: 1, ownAlbums: 2, ownAlbumsWithoutSuccessor: 1, otherPeoplesAlbums: 0),
  );

  testWidgets('shows the effects in numbers and the legacy contact', (tester) async {
    await open(tester);

    expect(find.text('Wspomnienia: 3'), findsOneWidget);
    expect(find.text('Zdjęcia: 12'), findsOneWidget);
    expect(find.text('Twoje albumy: 1, w tym bez następcy: 0'), findsOneWidget);
    expect(find.text('Cudze albumy, w których uczestniczysz: 2'), findsOneWidget);
    expect(find.text('Kontakt spadkowy'), findsOneWidget);
    expect(find.text('Wskaż kontakt spadkowy'), findsOneWidget);
  });

  testWidgets('deleting waits for the confirmation; then the window opens and can be cancelled', (tester) async {
    await open(tester);
    expect(isEnabled(tester, 'Usuń konto'), isFalse);

    await tapText(tester, 'Rozumiem skutki usunięcia konta');
    expect(isEnabled(tester, 'Usuń konto'), isTrue);

    await tapText(tester, 'Usuń konto');

    expect(account.deletionRequests, [false]);
    expect(find.byKey(const Key('s24-window')), findsOneWidget);
    expect(find.text('Usuń konto'), findsNothing);

    await tapText(tester, 'Cofnij usunięcie');

    expect(account.cancellations, 1);
    expect(find.byKey(const Key('s24-window')), findsNothing);
    expect(find.text('Usuń konto'), findsOneWidget);
  });

  testWidgets('an owner without a successor also has to choose', (tester) async {
    account.overview = withoutSuccessor;
    await open(tester);

    await tapText(tester, 'Rozumiem skutki usunięcia konta');
    expect(isEnabled(tester, 'Usuń konto'), isFalse);

    await tapText(tester, 'Zgadzam się na wygaśnięcie albumów bez następcy');
    expect(isEnabled(tester, 'Usuń konto'), isTrue);

    await tapText(tester, 'Usuń konto');

    expect(account.deletionRequests, [true]);
  });

  testWidgets('a 409 is a message and keeps what was marked', (tester) async {
    account.overview = withoutSuccessor;
    await open(tester);
    await tapText(tester, 'Rozumiem skutki usunięcia konta');
    await tapText(tester, 'Zgadzam się na wygaśnięcie albumów bez następcy');
    account.actionFailure = const DeletionAlreadyDecided();

    await tapText(tester, 'Usuń konto');

    expect(find.text('To już rozstrzygnięte: okno usunięcia konta się zakończyło.'), findsOneWidget);
    expect(isEnabled(tester, 'Usuń konto'), isTrue);
    final boxes = tester.widgetList<CheckboxListTile>(find.byType(CheckboxListTile));
    expect(boxes.every((box) => box.value == true), isTrue);
  });

  testWidgets('loading: the shape of the screen without actions', (tester) async {
    final gate = Completer<void>();
    await open(tester, repository: _WaitingAccount(gate.future));

    expect(find.byKey(const Key('s24-skeleton')), findsOneWidget);
    expect(find.text('Usuń konto'), findsNothing);

    gate.complete();
    await tester.pumpAndSettle();

    expect(find.text('Usuń konto'), findsOneWidget);
  });

  group('error: the message depends on the failure', () {
    const messages = <AccountFailure, String>{
      AccountOffline(): 'Brak połączenia. Sprawdź internet i spróbuj ponownie.',
      AccountForbidden(): 'Brak dostępu. Zaloguj się ponownie.',
      AccountServerError(): 'Błąd serwera. Spróbuj ponownie za chwilę.',
    };

    messages.forEach((failure, message) {
      testWidgets(message, (tester) async {
        account.loadFailure = failure;
        await open(tester);
        await tester.pumpAndSettle();

        expect(find.text(message), findsOneWidget);

        account.loadFailure = null;
        await tester.tap(find.text('Ponów'));
        await tester.pumpAndSettle();

        expect(find.text('Wspomnienia: 3'), findsOneWidget);
      });
    });

    testWidgets('a failed action can be retried', (tester) async {
      await open(tester);
      await tapText(tester, 'Rozumiem skutki usunięcia konta');
      account.actionFailure = const AccountServerError();
      await tapText(tester, 'Usuń konto');

      expect(find.byKey(const Key('s24-error')), findsOneWidget);

      account.actionFailure = null;
      await tapText(tester, 'Ponów');

      expect(account.deletionRequests, [false]);
      expect(find.byKey(const Key('s24-window')), findsOneWidget);
    });
  });

  testWidgets('offline: the banner, the data read only, the actions off', (tester) async {
    await open(tester, online: Stream.value(false));

    expect(find.byKey(const Key('s24-offline')), findsOneWidget);
    expect(find.text('Wspomnienia: 3'), findsOneWidget);
    expect(isEnabled(tester, 'Usuń konto'), isFalse);
    final box = tester.widget<CheckboxListTile>(find.byType(CheckboxListTile));
    expect(box.onChanged, isNull);
  });
}

class _WaitingAccount extends FakeAccountRepository {
  _WaitingAccount(this.gate);

  final Future<void> gate;

  @override
  Future<Result<AccountOverview, AccountFailure>> loadOverview() async {
    await gate;
    return super.loadOverview();
  }
}
