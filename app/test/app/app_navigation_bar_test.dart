import 'package:flutter/gestures.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/app/app_navigation_bar.dart';
import 'package:memorio/app/app_theme.dart';

void main() {
  const items = [
    AppNavigationItem(icon: Icons.public_outlined, selectedIcon: Icons.public, label: 'Mapa'),
    AppNavigationItem(icon: Icons.photo_album_outlined, selectedIcon: Icons.photo_album, label: 'Albumy'),
  ];

  Future<List<int>> pump(WidgetTester tester, ThemeData theme) async {
    final chosen = <int>[];
    await tester.pumpWidget(
      MaterialApp(
        theme: theme,
        home: Scaffold(
          bottomNavigationBar: AppNavigationBar(items: items, selectedIndex: 0, onSelected: chosen.add),
        ),
      ),
    );
    return chosen;
  }

  Color iconColor(WidgetTester tester, IconData icon) => tester.widget<Icon>(find.byIcon(icon)).color!;

  double scaleOf(WidgetTester tester, String label) => tester
      .widget<AnimatedScale>(
        find.descendant(of: find.byKey(Key('app-navigation-icon-$label')), matching: find.byType(AnimatedScale)),
      )
      .scale;

  Color badgeColor(WidgetTester tester, String label) =>
      (tester.widget<AnimatedContainer>(find.byKey(Key('app-navigation-badge-$label'))).decoration! as BoxDecoration)
          .color!;

  testWidgets('the chosen tab has its badge in the accent, the others not, and a tap chooses', (tester) async {
    final chosen = await pump(tester, AppThemes.light);

    expect(badgeColor(tester, 'Mapa'), AppThemes.lightAccent);
    expect(badgeColor(tester, 'Albumy'), isNot(AppThemes.lightAccent));
    expect(iconColor(tester, Icons.photo_album_outlined), isNot(AppThemes.lightAccent));

    await tester.tap(find.text('Albumy'));
    expect(chosen, [1]);
  });

  testWidgets('hovering an icon makes it blue, bigger and raised; leaving puts it back', (tester) async {
    await pump(tester, AppThemes.light);
    final mouse = await tester.createGesture(kind: PointerDeviceKind.mouse);
    addTearDown(mouse.removePointer);

    await mouse.addPointer(location: Offset.zero);
    await mouse.moveTo(tester.getCenter(find.byIcon(Icons.photo_album_outlined)));
    await tester.pumpAndSettle();

    expect(iconColor(tester, Icons.photo_album_outlined), AppThemes.lightAccent);
    expect(scaleOf(tester, 'Albumy'), greaterThan(1));
    expect(tester.widget<AnimatedSlide>(find.byKey(const Key('app-navigation-icon-Albumy'))).offset.dy, lessThan(0));

    await mouse.moveTo(const Offset(1, 1));
    await tester.pumpAndSettle();
    expect(scaleOf(tester, 'Albumy'), 1);
  });

  testWidgets('in the dark look the hovered icon is gold', (tester) async {
    await pump(tester, AppThemes.dark);
    final mouse = await tester.createGesture(kind: PointerDeviceKind.mouse);
    addTearDown(mouse.removePointer);

    await mouse.addPointer(location: Offset.zero);
    await mouse.moveTo(tester.getCenter(find.byIcon(Icons.photo_album_outlined)));
    await tester.pumpAndSettle();

    expect(iconColor(tester, Icons.photo_album_outlined), AppThemes.darkAccent);
  });

  testWidgets('the pad is low: the bigger icons stand out above it, the labels lie on it', (tester) async {
    await pump(tester, AppThemes.light);

    final pad = tester.getRect(find.byKey(const Key('app-navigation-pad')));
    final badge = tester.getRect(find.byKey(const Key('app-navigation-badge-Albumy')));
    final label = tester.getRect(find.byKey(const Key('app-navigation-label-Albumy')));
    expect(pad.height, AppNavigationBar.padHeight);
    expect(badge.top, lessThan(pad.top - 10));
    expect(badge.bottom, greaterThan(pad.top));
    expect(pad.contains(label.center), isTrue);
    expect(tester.getSize(find.byIcon(Icons.photo_album_outlined)).width, AppNavigationBar.iconSize);
  });

  testWidgets('the bar stays small in the middle of a wide screen', (tester) async {
    tester.view.physicalSize = const Size(1600, 900);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    await pump(tester, AppThemes.light);

    final bar = tester.getRect(find.byKey(const Key('app-navigation')));
    expect(bar.width, AppNavigationBar.maxWidth);
    expect(bar.center.dx, closeTo(800, 1));
  });
}
