import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_riverpod/misc.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/app/router.dart';
import 'package:memorio/app/routes.dart';
import 'package:memorio/features/plans/plans.dart';

import '../../helpers/app.dart';
import '../../helpers/fake_albums.dart';
import '../../helpers/fake_plans.dart';

void main() {
  late FakePlansRepository plans;
  final trimAt = DateTime.utc(2028, 12, 24, 12);

  setUp(() => plans = FakePlansRepository());

  Future<ProviderContainer> open(
    WidgetTester tester,
    String location, {
    Stream<bool>? online,
    List<Override> overrides = const [],
  }) async {
    tester.view.physicalSize = const Size(1000, 2400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    final container = await pumpMemorioApp(
      tester,
      session: signedIn,
      online: online,
      plans: plans,
      overrides: overrides,
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

  const messages = <PlanFailure, String>{
    PlanOffline(): 'Brak połączenia. Sprawdź internet i spróbuj ponownie.',
    PlanAlreadyDecided(): 'To już rozstrzygnięte. Odśwież i sprawdź aktualny stan.',
    PlanServerError(): 'Błąd serwera. Spróbuj ponownie za chwilę.',
  };

  group('S18', () {
    final location = const PlanRoute().location;

    testWidgets('a free plan has a state too, with what is used', (tester) async {
      await open(tester, location);

      expect(find.text('Plan darmowy'), findsOneWidget);
      expect(find.text('Zajęte miejsce: 1.0 GB z 2.0 GB'), findsOneWidget);
      expect(find.text('AI w tym miesiącu: 1 z 20'), findsOneWidget);
      expect(find.byKey(const Key('s18-trim')), findsNothing);
      expect(find.text('Wybierz, co zostanie'), findsNothing);
    });

    testWidgets('an active plan says until when', (tester) async {
      plans.state = planState(mode: PlanMode.active, entitledUntil: DateTime.utc(2027, 3, 31, 12));
      await open(tester, location);

      expect(find.text('Plan Premium'), findsOneWidget);
      expect(find.text('Opłacony do: 31.03.2027'), findsOneWidget);
    });

    testWidgets('after the plan ended it says when the trim comes and that nothing is lost yet', (tester) async {
      plans.state = planState(mode: PlanMode.readOnly, readOnlySince: DateTime.utc(2026, 12, 24, 12), trimAt: trimAt);
      await open(tester, location);

      expect(find.text('Nic nowego ponad darmowy limit'), findsOneWidget);
      expect(find.byKey(const Key('s18-read-only')), findsOneWidget);
      expect(find.textContaining('24.12.2028'), findsOneWidget);
      expect(find.textContaining('Nic nie zniknie'), findsOneWidget);
      expect(find.text('Wybierz, co zostanie'), findsNothing);
    });

    testWidgets('an album says read only instead', (tester) async {
      plans.state = planState(mode: PlanMode.readOnly, readOnlySince: DateTime.utc(2026, 12, 24, 12), trimAt: trimAt);
      await open(tester, const AlbumPlanRoute('album-1').location);

      expect(find.text('Plan albumu'), findsOneWidget);
      expect(find.text('Tylko do odczytu'), findsOneWidget);
    });

    testWidgets('a held trim names no date', (tester) async {
      plans.state = planState(
        mode: PlanMode.readOnly,
        readOnlySince: DateTime.utc(2026, 12, 24, 12),
        trimAt: trimAt,
        isHeld: true,
      );
      await open(tester, location);

      expect(find.byKey(const Key('s18-held')), findsOneWidget);
      expect(find.byKey(const Key('s18-trim')), findsNothing);
    });

    testWidgets('in the window the choice opens S19', (tester) async {
      plans.state = planState(
        mode: PlanMode.readOnly,
        readOnlySince: DateTime.utc(2026, 12, 24, 12),
        trimAt: trimAt,
        isChoiceOpen: true,
      );
      final container = await open(tester, location);

      await tester.tap(find.text('Wybierz, co zostanie'));
      await tester.pumpAndSettle();

      expect(container.read(routerProvider).state.uri.path, const PlanMaterialRoute().location);
    });

    testWidgets('refreshing asks the source', (tester) async {
      await open(tester, location);

      await tester.tap(find.text('Przywróć zakupy / odśwież'));
      await tester.pumpAndSettle();

      expect(plans.actions, ['refresh:null']);
    });

    testWidgets('a refused refresh is a message on the screen', (tester) async {
      plans.actionFailure = const PlanAlreadyDecided();
      await open(tester, location);

      await tester.tap(find.text('Przywróć zakupy / odśwież'));
      await tester.pumpAndSettle();

      expect(find.text(messages[const PlanAlreadyDecided()]!), findsOneWidget);
    });

    testWidgets('offline shows a banner and turns the action off', (tester) async {
      await open(tester, location, online: Stream.value(false));

      expect(find.byKey(const Key('s18-offline')), findsOneWidget);
      expect(isEnabled(tester, 'Przywróć zakupy / odśwież'), isFalse);
    });

    for (final failure in messages.entries) {
      testWidgets('a failed load says why and offers a retry: ${failure.value}', (tester) async {
        plans.loadFailure = failure.key;
        await open(tester, location);

        expect(find.text(failure.value), findsOneWidget);
        plans.loadFailure = null;
        await tester.tap(find.text('Ponów'));
        await tester.pumpAndSettle();
        expect(find.text('Plan darmowy'), findsOneWidget);
      });
    }

    testWidgets('someone else sees no plan of an album and no retry', (tester) async {
      plans.loadFailure = const PlanNoAccess();
      await open(tester, const AlbumPlanRoute('album-1').location);

      expect(find.byKey(const Key('plan-load-error')), findsOneWidget);
      expect(find.text('Ponów'), findsNothing);
    });

    testWidgets('loading shows the skeleton', (tester) async {
      tester.view.physicalSize = const Size(1000, 2400);
      tester.view.devicePixelRatio = 1;
      addTearDown(tester.view.reset);
      final container = await pumpMemorioApp(
        tester,
        session: signedIn,
        plans: plans,
        overrides: [
          planProvider.overrideWith(
            (ref, albumId) => Future<PlanState>.delayed(const Duration(days: 1), () => plans.state),
          ),
        ],
      );
      container.read(routerProvider).go(location);
      await tester.pump();
      await tester.pump();

      expect(find.byKey(const Key('plan-skeleton')), findsOneWidget);
      await tester.pumpWidget(const SizedBox());
      await tester.pump(const Duration(days: 1));
    });

    testWidgets('the account screen leads here', (tester) async {
      final container = await open(tester, const AccountRoute().location);

      await tester.tap(find.byTooltip('Plan i subskrypcja'));
      await tester.pumpAndSettle();

      expect(container.read(routerProvider).state.uri.path, location);
    });
  });

  group('S19', () {
    final location = const PlanMaterialRoute().location;

    void openWindow({List<String> kept = const []}) {
      plans
        ..state = planState(
          mode: PlanMode.readOnly,
          readOnlySince: DateTime.utc(2026, 12, 24, 12),
          trimAt: trimAt,
          isChoiceOpen: true,
          kept: kept,
        )
        ..material = const [oldest, middle, newest];
    }

    bool isChecked(WidgetTester tester, TrimMaterial item) =>
        tester.widget<CheckboxListTile>(find.byKey(Key('s19-item-${item.memoryId}'))).value!;

    testWidgets('the oldest start selected up to the free limit, with the limit and the deadline', (tester) async {
      openWindow();
      await open(tester, location);

      expect(isChecked(tester, oldest), isTrue);
      expect(isChecked(tester, middle), isTrue);
      expect(isChecked(tester, newest), isFalse);
      expect(find.text('Wybrano 2.0 GB z darmowego limitu 2.0 GB'), findsOneWidget);
      expect(find.textContaining('24.12.2028'), findsOneWidget);
    });

    testWidgets('what was stored comes back selected', (tester) async {
      openWindow(kept: ['m3']);
      await open(tester, location);

      expect(isChecked(tester, oldest), isFalse);
      expect(isChecked(tester, newest), isTrue);
    });

    testWidgets('a choice is stored', (tester) async {
      openWindow();
      await open(tester, location);

      await tester.tap(find.text('Giewont'));
      await tester.pump();
      await tester.tap(find.text('Kraków'));
      await tester.pump();
      await tester.tap(find.text('Zachowaj wybrane'));
      await tester.pumpAndSettle();

      expect(plans.actions, ['keep:null:m2,m3']);
      expect(find.byKey(const Key('s19-saved')), findsOneWidget);
    });

    testWidgets('over the limit it says so and nothing can be stored', (tester) async {
      openWindow();
      await open(tester, location);

      await tester.tap(find.text('Kraków'));
      await tester.pump();

      expect(find.byKey(const Key('s19-too-much')), findsOneWidget);
      expect(isEnabled(tester, 'Zachowaj wybrane'), isFalse);
    });

    testWidgets('a refusal is a message and the selection stays', (tester) async {
      openWindow();
      plans.actionFailure = const PlanChoiceClosed();
      await open(tester, location);

      await tester.tap(find.text('Giewont'));
      await tester.pump();
      await tester.tap(find.text('Zachowaj wybrane'));
      await tester.pumpAndSettle();

      expect(find.byKey(const Key('plan-error')), findsOneWidget);
      expect(isChecked(tester, oldest), isFalse);
      expect(isChecked(tester, middle), isTrue);
    });

    testWidgets('outside the window nothing can be changed', (tester) async {
      plans
        ..state = planState(mode: PlanMode.readOnly, trimAt: trimAt)
        ..material = const [oldest];
      await open(tester, location);

      expect(find.byKey(const Key('s19-closed')), findsOneWidget);
      expect(isEnabled(tester, 'Zachowaj wybrane'), isFalse);
      expect(tester.widget<CheckboxListTile>(find.byKey(const Key('s19-item-m1'))).onChanged, isNull);
    });

    testWidgets('offline shows a banner and turns the actions off', (tester) async {
      openWindow();
      await open(tester, location, online: Stream.value(false));

      expect(find.byKey(const Key('s19-offline')), findsOneWidget);
      expect(isEnabled(tester, 'Zachowaj wybrane'), isFalse);
    });

    testWidgets('someone else is told it is not theirs', (tester) async {
      plans.loadFailure = const PlanNoAccess();
      await open(tester, const AlbumPlanMaterialRoute('album-1').location);

      expect(find.byKey(const Key('plan-load-error')), findsOneWidget);
    });

    testWidgets('an album stores under its own id', (tester) async {
      openWindow();
      await open(tester, const AlbumPlanMaterialRoute('album-1').location);

      await tester.tap(find.text('Giewont'));
      await tester.pump();
      await tester.tap(find.text('Zachowaj wybrane'));
      await tester.pumpAndSettle();

      expect(plans.actions, ['keep:album-1:m2']);
    });
  });

  group('S10', () {
    testWidgets('a read-only album says so and adds nothing', (tester) async {
      final albums = FakeAlbumsRepository()..details = albumDetails(isReadOnly: true);
      tester.view.physicalSize = const Size(1000, 2400);
      tester.view.devicePixelRatio = 1;
      addTearDown(tester.view.reset);
      final container = await pumpMemorioApp(tester, session: signedIn, albums: albums);
      container.read(routerProvider).go(const AlbumRoute('album-1').location);
      await tester.pumpAndSettle();

      expect(find.byKey(const Key('s10-read-only')), findsOneWidget);
      expect(isEnabled(tester, 'Dodaj moje wspomnienie'), isFalse);
      expect(isEnabled(tester, 'Dodaj zdjęcia'), isFalse);
    });

    testWidgets('its owner reaches the plan', (tester) async {
      final albums = FakeAlbumsRepository()..details = albumDetails(isOwner: true);
      tester.view.physicalSize = const Size(1000, 2400);
      tester.view.devicePixelRatio = 1;
      addTearDown(tester.view.reset);
      final container = await pumpMemorioApp(tester, session: signedIn, albums: albums, plans: plans);
      container.read(routerProvider).go(const AlbumRoute('album-1').location);
      await tester.pumpAndSettle();

      await tester.tap(find.byTooltip('Plan albumu'));
      await tester.pumpAndSettle();

      expect(container.read(routerProvider).state.uri.path, const AlbumPlanRoute('album-1').location);
    });
  });
}
