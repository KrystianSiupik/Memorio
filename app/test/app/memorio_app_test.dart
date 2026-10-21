import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import '../helpers/app.dart';
import '../helpers/fake_http.dart';

void main() {
  testWidgets('a supported app starts normally', (tester) async {
    await pumpMemorioApp(tester, session: signedIn);

    expect(find.byType(NavigationBar), findsOneWidget);
    expect(find.text('Please update Memorio'), findsNothing);
  });

  testWidgets('an app the backend no longer serves shows the update screen', (tester) async {
    await pumpMemorioApp(
      tester,
      session: signedIn,
      http: FakeHttpAdapter((_) => (426, {'code': 'App.UpdateRequired', 'minimumVersion': '1.2.0'})),
    );

    expect(find.text('Please update Memorio'), findsOneWidget);
    expect(find.textContaining('Version 1.2.0 or newer'), findsOneWidget);
  });

  testWidgets('the app starts when the backend cannot be reached', (tester) async {
    await pumpMemorioApp(
      tester,
      session: signedIn,
      http: FakeHttpAdapter(
        (request) => throw DioException.connectionError(requestOptions: request, reason: 'offline'),
      ),
    );

    expect(find.byType(NavigationBar), findsOneWidget);
  });
}
