import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/app/memorio_app.dart';

void main() {
  testWidgets('the app starts on the start page', (tester) async {
    await tester.pumpWidget(const ProviderScope(child: MemorioApp()));
    await tester.pumpAndSettle();

    expect(find.text('Memorio'), findsOneWidget);
  });
}
