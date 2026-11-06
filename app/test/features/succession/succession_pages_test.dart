import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/app/router.dart';
import 'package:memorio/app/routes.dart';
import 'package:memorio/features/albums/albums.dart' show albumDetailsProvider;
import 'package:memorio/features/succession/succession.dart';

import '../../helpers/app.dart';
import '../../helpers/fake_albums.dart';
import '../../helpers/fake_succession.dart';

void main() {
  late FakeSuccessionRepository succession;
  late FakeAlbumsRepository albums;

  setUp(() {
    succession = FakeSuccessionRepository();
    albums = FakeAlbumsRepository()..details = albumDetails();
  });

  Future<ProviderContainer> open(WidgetTester tester, String location, {Stream<bool>? online}) async {
    tester.view.physicalSize = const Size(1000, 2400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    final container = await pumpMemorioApp(
      tester,
      session: signedIn,
      albums: albums,
      succession: succession,
      online: online,
    );
    container.read(routerProvider).go(location);
    await tester.pumpAndSettle();
    return container;
  }

  bool isEnabled(WidgetTester tester, String label) => switch (tester.widget(
    find
        .ancestor(of: find.text(label), matching: find.byWidgetPredicate((widget) => widget is ButtonStyleButton))
        .first,
  )) {
    final ButtonStyleButton button => button.onPressed != null,
    _ => false,
  };

  String pathOf(ProviderContainer container) => container.read(routerProvider).state.uri.path;

  group('S23', () {
    final location = const TakeoverOfferRoute('case-1').location;

    testWidgets('shows the album, the deadline and what the founder brought', (tester) async {
      await open(tester, location);

      expect(find.text('Rodzina'), findsOneWidget);
      expect(find.text('Na decyzję masz czas do: 01.02.2030'), findsOneWidget);
      expect(find.text('Wspomnienia fundatora w albumie: 3. Zostają w nim.'), findsOneWidget);
    });

    testWidgets('taking it over answers and goes to the albums', (tester) async {
      final container = await open(tester, location);

      await tester.tap(find.text('Przejmuję album'));
      await tester.pumpAndSettle();

      expect(succession.actions, ['answer:case-1:true']);
      expect(pathOf(container), const AlbumsRoute().location);
    });

    testWidgets('refusing answers', (tester) async {
      await open(tester, location);

      await tester.tap(find.text('Odmawiam'));
      await tester.pumpAndSettle();

      expect(succession.actions, ['answer:case-1:false']);
    });

    testWidgets('a conflict is a message and the screen stays', (tester) async {
      succession.actionFailure = const SuccessionAlreadyDecided();
      final container = await open(tester, location);

      await tester.tap(find.text('Przejmuję album'));
      await tester.pumpAndSettle();

      expect(find.text('Sprawa już rozstrzygnięta.'), findsOneWidget);
      expect(pathOf(container), location);
    });

    testWidgets('someone else is told, without a retry', (tester) async {
      succession.offer = null;
      await open(tester, location);

      expect(find.text('Nie jesteś wskazanym następcą.'), findsOneWidget);
      expect(find.text('Ponów'), findsNothing);
    });

    testWidgets('a decided case is told, without a retry', (tester) async {
      succession.loadFailure = const SuccessionAlreadyDecided();
      await open(tester, location);

      expect(find.text('Sprawa już rozstrzygnięta.'), findsOneWidget);
      expect(find.text('Ponów'), findsNothing);
    });

    testWidgets('a server error offers a retry that works', (tester) async {
      succession.loadFailure = const SuccessionServerError();
      await open(tester, location);

      expect(find.text('Błąd serwera. Spróbuj ponownie za chwilę.'), findsOneWidget);
      succession.loadFailure = null;
      await tester.tap(find.text('Ponów'));
      await tester.pumpAndSettle();
      expect(find.text('Rodzina'), findsOneWidget);
    });

    testWidgets('offline shows a banner and turns the actions off', (tester) async {
      await open(tester, location, online: Stream.value(false));

      expect(find.byKey(const Key('succession-offline')), findsOneWidget);
      expect(isEnabled(tester, 'Przejmuję album'), isFalse);
      expect(isEnabled(tester, 'Odmawiam'), isFalse);
    });

    testWidgets('S09 leads here when an offer waits', (tester) async {
      succession.offers = [takeoverOffer()];
      final container = await open(tester, const AlbumsRoute().location);

      await tester.tap(find.text('Wskazano Cię jako następcę: Rodzina'));
      await tester.pumpAndSettle();

      expect(pathOf(container), location);
    });
  });

  group('S25', () {
    final location = const ReportDeathRoute('album-1').location;

    testWidgets('nothing is sent without a certificate', (tester) async {
      await open(tester, location);

      expect(find.text('Rodzina'), findsOneWidget);
      expect(isEnabled(tester, 'Wyślij zgłoszenie'), isFalse);
    });

    testWidgets('a certificate is sent and it says what happens next', (tester) async {
      await open(tester, location);

      await tester.tap(find.text('Wybierz zdjęcie aktu zgonu'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Wyślij zgłoszenie'));
      await tester.pumpAndSettle();

      expect(succession.actions, ['report:album-1:a.jpg']);
      expect(find.byKey(const Key('s25-sent')), findsOneWidget);
    });

    testWidgets('a refusal is a message and the file stays chosen', (tester) async {
      succession.actionFailure = const SuccessionAlreadyReported();
      await open(tester, location);

      await tester.tap(find.text('Wybierz zdjęcie aktu zgonu'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Wyślij zgłoszenie'));
      await tester.pumpAndSettle();

      expect(find.text('Zgłoszenie dotyczące tego albumu jest już sprawdzane.'), findsOneWidget);
      expect(find.text('a.jpg'), findsOneWidget);
    });

    testWidgets('offline turns the actions off', (tester) async {
      await open(tester, location, online: Stream.value(false));

      expect(find.byKey(const Key('succession-offline')), findsOneWidget);
      expect(isEnabled(tester, 'Wybierz zdjęcie aktu zgonu'), isFalse);
    });

    testWidgets('S10 leads here for someone in the album and not for the owner', (tester) async {
      final container = await open(tester, const AlbumRoute('album-1').location);

      await tester.tap(find.text('Zgłoś zgon właściciela'));
      await tester.pumpAndSettle();
      expect(pathOf(container), location);

      albums.details = albumDetails(isOwner: true);
      container.invalidate(albumDetailsProvider);
      container.read(routerProvider).go(const AlbumRoute('album-1').location);
      await tester.pumpAndSettle();
      expect(find.text('Zgłoś zgon właściciela'), findsNothing);
    });
  });

  group('S10 without an owner', () {
    final location = const AlbumRoute('album-1').location;

    testWidgets('anyone in it may reserve it', (tester) async {
      albums.details = albumDetails(isOwnerless: true);
      await open(tester, location);

      expect(find.byKey(const Key('s10-ownerless')), findsOneWidget);
      expect(find.text('Zgłoś zgon właściciela'), findsNothing);
      await tester.tap(find.text('Przejmuję'));
      await tester.pumpAndSettle();

      expect(succession.actions, ['reserve:album-1']);
    });

    testWidgets('one\'s own reservation says until when', (tester) async {
      albums.details = albumDetails(isOwnerless: true);
      succession.takeover = AlbumTakeover(
        canReserve: false,
        reservedByMe: true,
        reservedUntil: DateTime(2030, 1, 1, 10, 30),
      );
      await open(tester, location);

      expect(find.byKey(const Key('s10-reserved-mine')), findsOneWidget);
      expect(find.textContaining('10:30'), findsOneWidget);
      expect(find.text('Przejmuję'), findsNothing);
    });

    testWidgets('someone else holding it is told', (tester) async {
      albums.details = albumDetails(isOwnerless: true);
      succession.takeover = AlbumTakeover(
        canReserve: false,
        reservedByMe: false,
        reservedUntil: DateTime(2030, 1, 1, 10, 30),
      );
      await open(tester, location);

      expect(find.byKey(const Key('s10-reserved-other')), findsOneWidget);
    });

    testWidgets('losing the race is a message', (tester) async {
      albums.details = albumDetails(isOwnerless: true);
      succession.actionFailure = const SuccessionReservedByOther();
      await open(tester, location);

      await tester.tap(find.text('Przejmuję'));
      await tester.pumpAndSettle();

      expect(find.text('Ktoś właśnie przejmuje ten album.'), findsOneWidget);
    });
  });

  group('Legacy contact', () {
    final location = const LegacyContactRoute().location;

    testWidgets('names someone and says who it is', (tester) async {
      await open(tester, location);
      expect(find.text('Nikogo nie wskazano.'), findsOneWidget);

      await tester.enterText(find.byType(TextField), 'ania@example.invalid');
      await tester.tap(find.text('Zapisz'));
      await tester.pumpAndSettle();

      expect(succession.actions, ['name:ania@example.invalid']);
    });

    testWidgets('a refusal keeps what was typed', (tester) async {
      succession.actionFailure = const SuccessionInvalidEmail();
      await open(tester, location);

      await tester.enterText(find.byType(TextField), 'nie adres');
      await tester.tap(find.text('Zapisz'));
      await tester.pumpAndSettle();

      expect(find.text('Podaj poprawny adres innej osoby.'), findsOneWidget);
      expect(find.text('nie adres'), findsOneWidget);
    });

    testWidgets('can be taken back', (tester) async {
      succession.contact = 'ania@example.invalid';
      await open(tester, location);

      expect(find.text('Wskazano: ania@example.invalid'), findsOneWidget);
      await tester.tap(find.text('Odwołaj wskazanie'));
      await tester.pumpAndSettle();

      expect(succession.actions, ['remove']);
    });

    testWidgets('the account screen leads here', (tester) async {
      final container = await open(tester, const AccountRoute().location);

      await tester.tap(find.byTooltip('Kontakt spadkowy'));
      await tester.pumpAndSettle();

      expect(pathOf(container), location);
    });
  });

  group('Legacy archive', () {
    testWidgets('S16 lists it and it opens read only', (tester) async {
      succession
        ..archives = [const LegacyArchive(ownerAccountId: 'owner-1', email: 'mama@example.invalid')]
        ..memories = [const LegacyMemory(id: 'memory-1', placeName: 'Giewont', dateFrom: '1990-07-01')];
      final container = await open(tester, const SharedWithMeRoute().location);

      await tester.tap(find.text('mama@example.invalid'));
      await tester.pumpAndSettle();

      expect(pathOf(container), const LegacyArchiveRoute('owner-1').location);
      expect(find.text('Giewont'), findsOneWidget);
      expect(find.text('01.07.1990'), findsOneWidget);
    });

    testWidgets('someone else is told', (tester) async {
      succession.loadFailure = const SuccessionNoAccess();
      await open(tester, const LegacyArchiveRoute('owner-1').location);

      expect(find.byKey(const Key('succession-load-error')), findsOneWidget);
      expect(find.text('Ponów'), findsNothing);
    });

    testWidgets('an empty archive says so', (tester) async {
      await open(tester, const LegacyArchiveRoute('owner-1').location);

      expect(find.text('Nie ma tu żadnych wspomnień.'), findsOneWidget);
    });
  });
}
