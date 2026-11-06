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
    tester.view.physicalSize = const Size(1000, 2400);
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

  const messages = <AlbumFailure, String>{
    AlbumOffline(): 'Brak połączenia. Sprawdź internet i spróbuj ponownie.',
    AlbumAlreadyDecided(): 'To już rozstrzygnięte. Odśwież i sprawdź aktualny stan.',
    AlbumServerError(): 'Błąd serwera. Spróbuj ponownie za chwilę.',
  };

  group('S09', () {
    final location = const AlbumsRoute().location;

    testWidgets('empty invites to create the first album', (tester) async {
      await open(tester, location);

      expect(find.text('Nie masz jeszcze albumów'), findsOneWidget);
      expect(find.text('Moje albumy'), findsOneWidget);
      expect(find.text('Udostępnione mi'), findsWidgets);
      expect(find.text('Utwórz album'), findsOneWidget);
    });

    testWidgets('a name is required and creating opens the album', (tester) async {
      albums.details = albumDetails();
      await open(tester, location);

      await tester.tap(find.text('Utwórz album'));
      await tester.pumpAndSettle();
      expect(find.text('Podaj nazwę albumu.'), findsOneWidget);
      expect(albums.actions, isEmpty);

      await tester.enterText(find.widgetWithText(TextField, 'Nazwa nowego albumu'), 'Rodzina');
      await tester.tap(find.text('Utwórz album'));
      await tester.pumpAndSettle();

      expect(albums.actions.single, startsWith('create:Rodzina:'));
      expect(find.widgetWithText(AppBar, 'Rodzina'), findsOneWidget);
    });

    testWidgets('a refusal keeps the name, and sending again is the same album', (tester) async {
      albums.actionFailure = const AlbumServerError();
      await open(tester, location);
      await tester.enterText(find.widgetWithText(TextField, 'Nazwa nowego albumu'), 'Rodzina');

      await tester.tap(find.text('Utwórz album'));
      await tester.pumpAndSettle();
      expect(find.byKey(const Key('album-error')), findsOneWidget);
      expect(find.text('Rodzina'), findsOneWidget);

      await tester.tap(find.text('Utwórz album'));
      await tester.pumpAndSettle();
      expect(albums.actions.toSet(), hasLength(1));
    });

    testWidgets('invitations and albums lead to their screens', (tester) async {
      albums.albums = const [AlbumSummary(id: 'album-1', name: 'Rodzina', isOwner: true, members: 3, memories: 5)];
      albums.invitations = const [
        AlbumInvitation(id: 'inv-1', albumId: 'album-2', albumName: 'Znajomi', invitedBy: 'kasia@example.invalid'),
      ];
      albums.details = albumDetails();
      await open(tester, location);

      expect(find.text('Osoby: 3, wspomnienia: 5'), findsOneWidget);

      await tester.tap(find.text('Znajomi'));
      await tester.pumpAndSettle();
      expect(find.widgetWithText(AppBar, 'Zaproszenie do albumu'), findsOneWidget);
    });

    testWidgets('offline turns creating off', (tester) async {
      await open(tester, location, online: Stream.value(false));

      expect(find.byKey(const Key('album-offline')), findsOneWidget);
      expect(isEnabled(tester, 'Utwórz album'), isFalse);
    });

    messages.forEach((failure, message) {
      testWidgets('a failed load says "$message" and Ponów loads again', (tester) async {
        albums.loadFailure = failure;
        await open(tester, location);
        expect(find.text(message), findsOneWidget);

        albums.loadFailure = null;
        await tester.tap(find.text('Ponów'));
        await tester.pumpAndSettle();

        expect(find.text('Nie masz jeszcze albumów'), findsOneWidget);
      });
    });
  });

  group('S10', () {
    final location = const AlbumRoute('album-1').location;

    testWidgets('shows memories, members, my contributions and the pool', (tester) async {
      albums.details = albumDetails();
      await open(tester, location);

      expect(find.text('Giewont'), findsOneWidget);
      expect(find.text('Moje wkłady: wspomnienia 1, zdjęcia 2'), findsOneWidget);
      expect(find.byKey(const Key('s10-pool')), findsOneWidget);
      expect(find.text('wlasciciel@example.invalid'), findsOneWidget);
      expect(find.text('właściciel'), findsOneWidget);
    });

    testWidgets('empty says so', (tester) async {
      albums.details = albumDetails(memories: const []);
      await open(tester, location);

      expect(find.text('Album jest pusty'), findsOneWidget);
    });

    testWidgets('forbidden says the access is gone, without a retry', (tester) async {
      await open(tester, location);

      expect(find.text('Nie masz już dostępu do tego albumu.'), findsOneWidget);
      expect(find.text('Ponów'), findsNothing);
    });

    testWidgets('adding my memory chooses from my memories', (tester) async {
      albums.details = albumDetails(memories: const []);
      albums.own = const [OwnMemory(id: 'own-1', placeName: 'Chata', dateFrom: '2026-01-02')];
      await open(tester, location);

      await tester.tap(find.text('Dodaj moje wspomnienie'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Chata'));
      await tester.pumpAndSettle();

      expect(albums.actions, ['addMemory:own-1']);
    });

    testWidgets('a full pool turns photos off and a refusal says so', (tester) async {
      albums.details = albumDetails(freeBytes: 0);
      albums.own = const [OwnMemory(id: 'own-1', placeName: 'Chata', dateFrom: '2026-01-02')];
      albums.actionFailure = const AlbumPoolFull();
      await open(tester, location);

      expect(isEnabled(tester, 'Dodaj zdjęcia'), isFalse);

      await tester.tap(find.text('Dodaj moje wspomnienie'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Chata'));
      await tester.pumpAndSettle();

      expect(find.text('Pula albumu jest pełna. Właściciel dostał powiadomienie.'), findsOneWidget);
    });

    testWidgets('photos go to the album', (tester) async {
      albums.details = albumDetails();
      await open(tester, location);

      await tester.tap(find.text('Dodaj zdjęcia'));
      await tester.pumpAndSettle();

      expect(albums.actions, ['addPhoto:memory-1']);
    });

    testWidgets('tagging chooses someone in the album, and I can remove my own tag', (tester) async {
      albums.details = albumDetails();
      await open(tester, location);

      await tester.tap(find.text('Oznacz uczestnika'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('inny@example.invalid').last);
      await tester.pumpAndSettle();
      await tester.tap(find.text('Usuń moje oznaczenie'));
      await tester.pumpAndSettle();

      expect(albums.actions, ['tag:memory-1:other-1', 'untag:memory-1']);
    });

    testWidgets('leaving needs a confirmation and carries the choice', (tester) async {
      albums.details = albumDetails();
      await open(tester, location);
      await tester.ensureVisible(find.text('Zabierz moje wkłady'));

      expect(isEnabled(tester, 'Opuść album'), isFalse);
      await tester.tap(find.text('Zabierz moje wkłady'));
      await tester.pump();
      await tester.tap(find.text('Potwierdzam wyjście z albumu'));
      await tester.pump();
      await tester.ensureVisible(find.widgetWithText(OutlinedButton, 'Opuść album'));
      await tester.tap(find.widgetWithText(OutlinedButton, 'Opuść album'));
      await tester.pumpAndSettle();

      expect(albums.actions, ['leave:false']);
      expect(find.text('Moje albumy'), findsOneWidget);
    });

    testWidgets('a handover offered to me shows the choice of the owner and takes my answer', (tester) async {
      albums.details = albumDetails();
      albums.handover = AlbumHandover(
        toAccountId: 'account-1',
        forMe: true,
        keepsContributions: false,
        until: DateTime.utc(2026, 12, 6, 12),
      );
      await open(tester, location);

      expect(find.text('Właściciel chce Ci przekazać ten album. Możesz odpowiedzieć do 06.12.2026.'), findsOneWidget);
      expect(find.text('Jego wspomnienia zostaną z albumu wyjęte.'), findsOneWidget);
      await tester.tap(find.text('Przejmuję album'));
      await tester.pumpAndSettle();

      expect(albums.actions, ['handover-accept']);
    });

    testWidgets('no handover for me, no card', (tester) async {
      albums.details = albumDetails();
      await open(tester, location);

      expect(find.byKey(const Key('s10-handover')), findsNothing);
    });

    testWidgets('the owner has no way to leave but a way to manage', (tester) async {
      albums.details = albumDetails(isOwner: true);
      await open(tester, location);

      expect(find.text('Zabierz moje wkłady'), findsNothing);

      await tester.tap(find.byTooltip('Zarządzanie albumem'));
      await tester.pumpAndSettle();
      expect(find.widgetWithText(AppBar, 'Zarządzanie albumem'), findsOneWidget);
    });

    testWidgets('a participant has no way to manage', (tester) async {
      albums.details = albumDetails();
      await open(tester, location);

      expect(find.byTooltip('Zarządzanie albumem'), findsNothing);
    });

    testWidgets('offline keeps the album readable with actions off', (tester) async {
      albums.details = albumDetails();
      await open(tester, location, online: Stream.value(false));

      expect(find.text('Giewont'), findsOneWidget);
      expect(isEnabled(tester, 'Dodaj moje wspomnienie'), isFalse);
      expect(isEnabled(tester, 'Oznacz uczestnika'), isFalse);
    });
  });

  group('S11', () {
    final location = const AlbumManageRoute('album-1').location;

    testWidgets('shows seats, invited, members and the missing successor', (tester) async {
      albums.details = albumDetails(isOwner: true);
      albums.seats = const AlbumSeats(
        taken: 3,
        invited: 1,
        free: 6,
        invitations: [AlbumInvited(id: 'inv-1', email: 'nowy@example.invalid')],
      );
      await open(tester, location);

      expect(find.text('Miejsca: zajęte 3, zaproszone 1, wolne 6'), findsOneWidget);
      expect(find.text('nowy@example.invalid'), findsOneWidget);
      expect(find.byKey(const Key('s11-no-successor')), findsOneWidget);
    });

    testWidgets('no free seat turns inviting off', (tester) async {
      albums.details = albumDetails(isOwner: true);
      albums.seats = const AlbumSeats(taken: 10, invited: 0, free: 0, invitations: []);
      await open(tester, location);

      expect(isEnabled(tester, 'Zaproś'), isFalse);
    });

    testWidgets('a bad address is not sent', (tester) async {
      albums.details = albumDetails(isOwner: true);
      await open(tester, location);

      await tester.enterText(find.widgetWithText(TextField, 'E-mail zapraszanego'), 'nie-adres');
      await tester.tap(find.text('Zaproś'));
      await tester.pumpAndSettle();

      expect(find.text('To nie wygląda na adres e-mail.'), findsOneWidget);
      expect(albums.actions, isEmpty);
    });

    const refusals = <AlbumFailure, String>{
      AlbumNoFreeSeats(): 'W planie tego albumu nie ma wolnego miejsca.',
      AlbumInvitationLimit(): 'Dzisiejszy limit 10 zaproszeń został wykorzystany. Spróbuj jutro.',
      AlbumAddressBlocked(): 'Ten adres nie przyjmuje zaproszeń.',
      AlbumAlreadyDecided(): 'To już rozstrzygnięte. Odśwież i sprawdź aktualny stan.',
    };
    refusals.forEach((failure, message) {
      testWidgets('"$message" keeps the address and sending again is the same invitation', (tester) async {
        albums.details = albumDetails(isOwner: true);
        albums.actionFailure = failure;
        await open(tester, location);
        await tester.enterText(find.widgetWithText(TextField, 'E-mail zapraszanego'), 'nowy@example.invalid');

        await tester.tap(find.text('Zaproś'));
        await tester.pumpAndSettle();
        expect(find.text(message), findsOneWidget);
        expect(find.text('nowy@example.invalid'), findsOneWidget);

        await tester.tap(find.text('Zaproś'));
        await tester.pumpAndSettle();
        expect(albums.actions.toSet(), hasLength(1));
      });
    });

    testWidgets('withdrawing, removing and naming a successor', (tester) async {
      albums.details = albumDetails(isOwner: true);
      albums.seats = const AlbumSeats(
        taken: 3,
        invited: 1,
        free: 6,
        invitations: [AlbumInvited(id: 'inv-1', email: 'nowy@example.invalid')],
      );
      await open(tester, location);

      await tester.tap(find.text('Wycofaj'));
      await tester.pumpAndSettle();
      await tester.tap(
        find.descendant(of: find.byKey(const Key('s11-member-other-1')), matching: find.text('Wskaż następcę')),
      );
      await tester.pumpAndSettle();
      await tester.tap(
        find.descendant(of: find.byKey(const Key('s11-member-other-1')), matching: find.text('Usuń uczestnika')),
      );
      await tester.pumpAndSettle();

      expect(albums.actions, ['withdraw:inv-1', 'successor:other-1', 'remove:other-1']);
    });

    testWidgets('handing over asks what becomes of my own memories', (tester) async {
      albums.details = albumDetails(isOwner: true);
      await open(tester, location);

      await tester.tap(find.byKey(const Key('s11-hand-over-other-1')));
      await tester.pumpAndSettle();
      expect(find.text('Przekazać album: inny@example.invalid?'), findsOneWidget);
      await tester.tap(find.byKey(const Key('s11-handover-take')));
      await tester.pumpAndSettle();

      expect(albums.actions, ['handover:other-1:take']);
    });

    testWidgets('a handover that waits shows who and until when, and can be withdrawn', (tester) async {
      albums.details = albumDetails(isOwner: true);
      albums.handover = AlbumHandover(toAccountId: 'other-1', until: DateTime.utc(2026, 12, 6, 12));
      await open(tester, location);
      await tester.scrollUntilVisible(
        find.byKey(const Key('s11-handover-waiting')),
        200,
        scrollable: find.byType(Scrollable).first,
      );

      expect(find.text('Czeka na odpowiedź: inny@example.invalid, do 06.12.2026.'), findsOneWidget);
      expect(find.byKey(const Key('s11-hand-over-other-1')), findsNothing);
      await tester.tap(find.text('Wycofaj propozycję'));
      await tester.pumpAndSettle();

      expect(albums.actions, ['handover-withdraw']);
    });

    testWidgets('not the owner: forbidden', (tester) async {
      albums.loadFailure = const AlbumNoAccess();
      await open(tester, location);

      expect(find.text('Nie masz już dostępu do tego albumu.'), findsOneWidget);
    });
  });

  group('S12', () {
    testWidgets('from the list: what stayed, revoked only after confirming', (tester) async {
      albums.kept = const [
        KeptContribution(albumId: 'album-1', albumName: 'Rodzina', chosenByAuthor: false, memories: 2),
      ];
      await open(tester, const MyConsentsRoute().location);

      expect(find.text('Rodzina: wspomnienia 2'), findsOneWidget);
      expect(find.text('Usunięto Cię z albumu. Wybierz, czy treści mają zostać.'), findsOneWidget);
      expect(isEnabled(tester, 'Odwołaj zgodę „zostaw”'), isFalse);

      await tester.tap(find.text('Wiem, że treści znikną z albumu'));
      await tester.pump();
      await tester.tap(find.text('Odwołaj zgodę „zostaw”'));
      await tester.pumpAndSettle();

      expect(albums.actions, ['revokeKept:album-1']);
    });

    testWidgets('nothing kept says so', (tester) async {
      await open(tester, const MyConsentsRoute().location);

      expect(find.text('Nie masz wkładów pozostawionych po wyjściu'), findsOneWidget);
    });

    testWidgets('from an album: the curator, set and revoked', (tester) async {
      albums.details = albumDetails();
      final container = await open(tester, const AlbumConsentsRoute('album-1').location);

      expect(find.byKey(const Key('s12-no-curator')), findsOneWidget);
      await tester.tap(
        find.descendant(of: find.byKey(const Key('s12-curator-other-1')), matching: find.text('Powierz kuratorowi')),
      );
      await tester.pumpAndSettle();
      expect(albums.actions, ['entrust:other-1']);

      albums.details = albumDetails(curator: 'other-1');
      container.invalidate(albumDetailsProvider('album-1'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Odwołaj powierzenie'));
      await tester.pumpAndSettle();
      expect(albums.actions.last, 'revokeEntrustment:album-1');
    });
  });

  group('S13', () {
    final location = const EntrustedRoute().location;

    testWidgets('empty says nobody entrusted anything', (tester) async {
      await open(tester, location);

      expect(find.text('Nikt nie powierzył Ci treści'), findsOneWidget);
    });

    testWidgets('moving and arranging sends the new order', (tester) async {
      albums.entrusted = const [
        EntrustedMemory(
          albumId: 'album-1',
          albumName: 'Rodzina',
          memoryId: 'a',
          placeName: 'Giewont',
          dateFrom: '2026-01-01',
        ),
        EntrustedMemory(
          albumId: 'album-1',
          albumName: 'Rodzina',
          memoryId: 'b',
          placeName: 'Chata',
          dateFrom: '2026-01-02',
        ),
      ];
      await open(tester, location);

      await tester.tap(find.descendant(of: find.byKey(const Key('s13-memory-b')), matching: find.byTooltip('Wyżej')));
      await tester.pump();
      await tester.tap(find.text('Uporządkuj'));
      await tester.pumpAndSettle();

      expect(albums.actions, ['arrange:b,a']);
    });
  });

  group('S14', () {
    const invitation = AlbumInvitation(
      id: 'inv-1',
      albumId: 'album-1',
      albumName: 'Rodzina',
      invitedBy: 'kasia@example.invalid',
    );
    final location = const InvitationRoute('inv-1').location;

    testWidgets('shows who invites and the album', (tester) async {
      albums.invitations = const [invitation];
      await open(tester, location);

      expect(find.text('Kto zaprasza: kasia@example.invalid'), findsOneWidget);
      expect(find.text('Album: Rodzina'), findsOneWidget);
    });

    testWidgets('an invitation no longer there says so', (tester) async {
      await open(tester, location);

      expect(find.byKey(const Key('s14-gone')), findsOneWidget);
    });

    testWidgets('S14 → S10: accepting opens the album', (tester) async {
      albums.invitations = const [invitation];
      albums.details = albumDetails();
      await open(tester, location);

      await tester.tap(find.text('Przyjmij'));
      await tester.pumpAndSettle();

      expect(albums.actions, ['accept:inv-1']);
      expect(find.widgetWithText(AppBar, 'Rodzina'), findsOneWidget);
    });

    testWidgets('S14 → S09: declining goes back to the list', (tester) async {
      albums.invitations = const [invitation];
      await open(tester, location);

      await tester.tap(find.text('Odrzuć'));
      await tester.pumpAndSettle();

      expect(albums.actions, ['decline:inv-1']);
      expect(find.text('Moje albumy'), findsOneWidget);
    });

    testWidgets('no more invitations also answers this one', (tester) async {
      albums.invitations = const [invitation];
      await open(tester, location);

      await tester.tap(find.text('Nie chcę zaproszeń z GEO'));
      await tester.pumpAndSettle();

      expect(albums.actions, ['block', 'decline:inv-1']);
    });

    testWidgets('a refusal stays on the screen', (tester) async {
      albums.invitations = const [invitation];
      albums.actionFailure = const AlbumInvitationGone();
      await open(tester, location);

      await tester.tap(find.text('Przyjmij'));
      await tester.pumpAndSettle();

      expect(find.byKey(const Key('album-error')), findsOneWidget);
      expect(find.text('Album: Rodzina'), findsOneWidget);
    });
  });
}
