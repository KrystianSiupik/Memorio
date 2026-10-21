import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/core/api/api.dart';

import '../helpers/fake_http.dart';

void main() {
  ProviderContainer containerWith(FakeHttpAdapter adapter) {
    final container = ProviderContainer(
      overrides: [
        appVersionProvider.overrideWithValue('1.0.0'),
        apiBaseUrlProvider.overrideWithValue('https://api.example.invalid'),
      ],
    );
    container.read(dioProvider).httpClientAdapter = adapter;
    addTearDown(container.dispose);
    return container;
  }

  test('the generated client calls the version policy endpoint with the app version', () async {
    final adapter = FakeHttpAdapter((_) => (200, {'minimumVersion': '0.9.0'}));
    final container = containerWith(adapter);

    final policy = await container.read(apiClientProvider).app.getAppVersionPolicy();

    expect(policy.minimumVersion, '0.9.0');
    expect(adapter.requests.single.uri.toString(), 'https://api.example.invalid/v1/app/version-policy');
    expect(adapter.requests.single.headers['X-App-Version'], '1.0.0');
    expect(container.read(updateRequiredProvider), isNull);
  });

  test('a 426 answer marks the app as needing an update', () async {
    final container = containerWith(
      FakeHttpAdapter((_) => (426, {'code': 'App.UpdateRequired', 'minimumVersion': '1.2.0'})),
    );

    await expectLater(container.read(apiClientProvider).app.getAppVersionPolicy(), throwsA(isA<DioException>()));

    expect(container.read(updateRequiredProvider), '1.2.0');
  });

  test('other errors do not ask for an update', () async {
    final container = containerWith(FakeHttpAdapter((_) => (500, null)));

    await expectLater(container.read(apiClientProvider).app.getAppVersionPolicy(), throwsA(isA<DioException>()));

    expect(container.read(updateRequiredProvider), isNull);
  });
}
