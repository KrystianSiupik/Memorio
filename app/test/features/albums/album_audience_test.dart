import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/app/router.dart';
import 'package:memorio/app/routes.dart';
import 'package:memorio/features/albums/albums.dart';

import '../../helpers/app.dart';
import '../../helpers/fake_albums.dart';

void main() {
  late FakeAlbumsRepository albums;

  setUp(() => albums = FakeAlbumsRepository());

  Future<ProviderContainer> open(WidgetTester tester, String location, {Stream<bool>? online}) async {
    tester.view.physicalSize = const Size(1000, 2800);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    final container = await pumpMemorioApp(tester, session: signedIn, albums: albums, online: online);
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

  group('S11: link and viewers', () {
    final location = const AlbumManageRoute('album-1').location;

    testWidgets('sharing shows the link, and revoking takes it away', (tester) async {
      albums.details = albumDetails(isOwner: true);
      await open(tester, location);
      expect(find.byKey(const Key('s11-no-link')), findsOneWidget);

      await tester.tap(find.text('Udostępnij linkiem'));
      await tester.pumpAndSettle();

      expect(albums.actions, ['share']);
      expect(tester.widget<SelectableText>(find.byKey(const Key('s11-link'))).data, endsWith('/albums/link/token-1'));

      await tester.tap(find.text('Unieważnij link (nikt nowy nie wejdzie)'));
      await tester.pumpAndSettle();

      expect(albums.actions.last, 'revokeLink');
      expect(find.byKey(const Key('s11-no-link')), findsOneWidget);
    });

    testWidgets('viewers are listed by name and access can be taken away', (tester) async {
      albums.details = albumDetails(isOwner: true);
      albums.viewers = const [AlbumViewer(accountId: 'viewer-1', email: 'gosc@example.invalid')];
      await open(tester, location);

      expect(find.text('gosc@example.invalid'), findsOneWidget);

      await tester.tap(
        find.descendant(of: find.byKey(const Key('s11-viewer-viewer-1')), matching: find.text('Odbierz dostęp')),
      );
      await tester.pumpAndSettle();

      expect(albums.actions, ['revokeAccess:viewer-1']);
    });

    testWidgets('a refusal is said and the link stays', (tester) async {
      albums.details = albumDetails(isOwner: true);
      albums.link = 'token-1';
      albums.actionFailure = const AlbumAlreadyDecided();
      await open(tester, location);

      await tester.tap(find.text('Unieważnij link (nikt nowy nie wejdzie)'));
      await tester.pumpAndSettle();

      expect(find.text('To już rozstrzygnięte. Odśwież i sprawdź aktualny stan.'), findsOneWidget);
    });

    testWidgets('offline turns link actions off', (tester) async {
      albums.details = albumDetails(isOwner: true);
      await open(tester, location, online: Stream.value(false));

      expect(isEnabled(tester, 'Udostępnij linkiem'), isFalse);
    });
  });

  group('S10: blocking', () {
    final location = const AlbumRoute('album-1').location;

    testWidgets('a participant chooses from viewers only, and blocking asks first', (tester) async {
      albums.details = albumDetails();
      albums.viewers = const [AlbumViewer(accountId: 'viewer-1', email: 'gosc@example.invalid')];
      await open(tester, location);

      await tester.ensureVisible(find.text('Zablokuj użytkownika'));
      await tester.tap(find.text('Zablokuj użytkownika'));
      await tester.pumpAndSettle();

      expect(find.text('inny@example.invalid'), findsOneWidget);
      await tester.tap(find.text('gosc@example.invalid'));
      await tester.pumpAndSettle();
      expect(find.text('Zablokować tę osobę?'), findsOneWidget);
      expect(albums.actions, isEmpty);

      await tester.tap(find.text('Zablokuj'));
      await tester.pumpAndSettle();

      expect(albums.actions, ['blockUser:viewer-1']);
    });

    testWidgets('cancelling blocks nobody', (tester) async {
      albums.details = albumDetails();
      albums.viewers = const [AlbumViewer(accountId: 'viewer-1', email: 'gosc@example.invalid')];
      await open(tester, location);

      await tester.ensureVisible(find.text('Zablokuj użytkownika'));
      await tester.tap(find.text('Zablokuj użytkownika'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('gosc@example.invalid'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Anuluj'));
      await tester.pumpAndSettle();

      expect(albums.actions, isEmpty);
    });

    testWidgets('the owner can also block someone in the album', (tester) async {
      albums.details = albumDetails(isOwner: true);
      await open(tester, location);

      await tester.ensureVisible(find.text('Zablokuj użytkownika'));
      await tester.tap(find.text('Zablokuj użytkownika'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('inny@example.invalid').last);
      await tester.pumpAndSettle();
      await tester.tap(find.text('Zablokuj'));
      await tester.pumpAndSettle();

      expect(albums.actions, ['blockUser:other-1']);
    });

    testWidgets('nobody to block says so', (tester) async {
      albums.details = albumDetails();
      await open(tester, location);

      await tester.ensureVisible(find.text('Zablokuj użytkownika'));
      await tester.tap(find.text('Zablokuj użytkownika'));
      await tester.pumpAndSettle();

      expect(find.text('Nie ma tu nikogo, kogo możesz zablokować.'), findsOneWidget);
    });

    testWidgets('someone reading through a link sees content, no people and no actions', (tester) async {
      albums.details = albumDetails(isMember: false);
      await open(tester, location);

      expect(find.byKey(const Key('s10-viewer')), findsOneWidget);
      expect(find.text('Giewont'), findsOneWidget);
      for (final hidden in [
        'Dodaj moje wspomnienie',
        'Oznacz uczestnika',
        'Dodaj zdjęcia',
        'Zablokuj użytkownika',
        'Skład',
        'Opuść album',
      ]) {
        expect(find.text(hidden), findsNothing, reason: hidden);
      }
      expect(find.byTooltip('Moje zgody i powierzenia'), findsNothing);
    });
  });

  group('S15', () {
    final location = const AlbumLinkRoute('token-1').location;

    testWidgets('shows the album and says the person comes in by name', (tester) async {
      await open(tester, location);

      expect(find.text('Album: Rodzina'), findsOneWidget);
      expect(find.text('Twój adres e-mail pojawi się na liście oglądających ten album.'), findsOneWidget);
    });

    testWidgets('S15 → S10: opening enters and shows the album', (tester) async {
      albums.details = albumDetails(isMember: false);
      await open(tester, location);

      await tester.tap(find.text('Otwórz album'));
      await tester.pumpAndSettle();

      expect(albums.actions, ['enter:token-1']);
      expect(find.widgetWithText(AppBar, 'Rodzina'), findsOneWidget);
    });

    testWidgets('a link that no longer works says so, without a retry', (tester) async {
      albums.loadFailure = const AlbumLinkGone();
      await open(tester, location);

      expect(find.text('Ten link już nie działa.'), findsOneWidget);
      expect(find.text('Ponów'), findsNothing);
      expect(find.text('Otwórz album'), findsNothing);
    });

    testWidgets('a blocked person has no access, without a retry', (tester) async {
      albums.loadFailure = const AlbumBlocked();
      await open(tester, location);

      expect(find.text('Nie masz dostępu do tego albumu.'), findsOneWidget);
      expect(find.text('Ponów'), findsNothing);
    });

    testWidgets('a link revoked in the meantime is said on the screen', (tester) async {
      albums.actionFailure = const AlbumLinkGone();
      await open(tester, location);

      await tester.tap(find.text('Otwórz album'));
      await tester.pumpAndSettle();

      expect(find.text('Ten link już nie działa.'), findsOneWidget);
      expect(find.text('Album: Rodzina'), findsOneWidget);
    });

    testWidgets('a failed check can be retried', (tester) async {
      albums.loadFailure = const AlbumServerError();
      await open(tester, location);
      expect(find.text('Błąd serwera. Spróbuj ponownie za chwilę.'), findsOneWidget);

      albums.loadFailure = null;
      await tester.tap(find.text('Ponów'));
      await tester.pumpAndSettle();

      expect(find.text('Album: Rodzina'), findsOneWidget);
    });

    testWidgets('offline turns the action off', (tester) async {
      await open(tester, location, online: Stream.value(false));

      expect(isEnabled(tester, 'Otwórz album'), isFalse);
    });

    testWidgets('signed out goes to sign-in, remembering the link', (tester) async {
      final container = await pumpMemorioApp(tester, albums: albums);

      container.read(routerProvider).go(location);
      await tester.pumpAndSettle();

      expect(find.widgetWithText(AppBar, 'Rejestracja'), findsOneWidget);
      expect(container.read(routerProvider).state.uri.queryParameters['from'], location);
    });
  });

  group('S16', () {
    final location = const SharedWithMeRoute().location;

    testWidgets('empty says nobody shared anything', (tester) async {
      await open(tester, location);

      expect(find.text('Nikt jeszcze nie udostępnił Ci albumu'), findsOneWidget);
    });

    testWidgets('S16 → S10: an album opens', (tester) async {
      albums.shared = const [SharedAlbum(id: 'album-1', name: 'Znajomi')];
      albums.details = albumDetails(isMember: false);
      await open(tester, location);

      await tester.tap(find.text('Znajomi'));
      await tester.pumpAndSettle();

      expect(find.byKey(const Key('s10-viewer')), findsOneWidget);
    });

    testWidgets('a failed load has a retry, and offline a banner', (tester) async {
      albums.loadFailure = const AlbumServerError();
      await open(tester, location, online: Stream.value(false));

      expect(find.byKey(const Key('s16-offline')), findsOneWidget);
      expect(find.text('Ponów'), findsOneWidget);
    });
  });
}
