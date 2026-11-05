import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/core/api/generated/clients/albums_client.dart';
import 'package:memorio/core/api/generated/clients/memories_client.dart';
import 'package:memorio/core/api/generated/clients/plans_client.dart';
import 'package:memorio/core/result.dart';
import 'package:memorio/features/plans/plans.dart';

import '../../helpers/fake_http.dart';

void main() {
  HttpPlansRepository repository(FakeHandler handler) {
    final dio = Dio(BaseOptions(baseUrl: 'https://api.example.invalid'))..httpClientAdapter = FakeHttpAdapter(handler);
    return HttpPlansRepository(plans: PlansClient(dio), memories: MemoriesClient(dio), albums: AlbumsClient(dio));
  }

  Map<String, Object?> plan(String mode) => {
    'mode': mode,
    'entitledUntil': null,
    'readOnlySince': '2026-12-24T12:00:00Z',
    'trimAt': '2028-12-24T12:00:00Z',
    'choiceOpensAt': '2028-11-24T12:00:00Z',
    'isChoiceOpen': true,
    'isHeld': false,
    'lastCheckedAt': null,
    'keptMemoryIds': ['m1'],
    'usedBytes': 5,
    'limitBytes': 10,
    'freeLimitBytes': 2,
    'aiUsed': 1,
    'aiLimit': 20,
  };

  test('a plan comes with its mode, dates and numbers', () async {
    final result = await repository((_) => (200, plan('ReadOnly'))).load(null);

    final state = (result as Ok<PlanState, PlanFailure>).value;
    expect(state.mode, PlanMode.readOnly);
    expect(state.trimAt, DateTime.utc(2028, 12, 24, 12));
    expect(state.isChoiceOpen, isTrue);
    expect(state.keptMemoryIds, ['m1']);
    expect(state.usedBytes, 5);
    expect(state.aiLimit, 20);
  });

  test('every mode has its name, and an unknown one reads as free', () async {
    for (final (sent, expected) in [
      ('Active', PlanMode.active),
      ('Trimmed', PlanMode.trimmed),
      ('Free', PlanMode.free),
      ('Closed', PlanMode.free),
    ]) {
      final result = await repository((_) => (200, plan(sent))).load(null);
      expect((result as Ok<PlanState, PlanFailure>).value.mode, expected);
    }
  });

  test('an album has its own addresses', () async {
    final paths = <String>[];
    final plans = repository((request) {
      paths.add('${request.method} ${request.path}');
      return switch (request.method) {
        'PUT' => (204, null),
        _ when request.path.endsWith('trim-material') => (
          200,
          [
            {'memoryId': 'm', 'placeName': 'Giewont', 'dateFrom': '1990-07-01', 'sizeBytes': 7},
          ],
        ),
        _ => (200, plan('Active')),
      };
    });

    await plans.load('a1');
    await plans.refresh('a1');
    final material = await plans.loadMaterial('a1');
    await plans.keep('a1', ['m']);

    expect(paths, [
      'GET /v1/albums/a1/plan',
      'POST /v1/albums/a1/plan/refresh',
      'GET /v1/albums/a1/trim-material',
      'PUT /v1/albums/a1/plan/kept-material',
    ]);
    expect((material as Ok<List<TrimMaterial>, PlanFailure>).value.single.sizeBytes, 7);
  });

  test('the person has theirs', () async {
    final paths = <String>[];
    final plans = repository((request) {
      paths.add('${request.method} ${request.path}');
      return switch (request.method) {
        'PUT' => (204, null),
        _ when request.path.endsWith('trim-material') => (
          200,
          [
            {'memoryId': 'm', 'placeName': 'Giewont', 'dateFrom': '1990-07-01', 'sizeBytes': 7},
          ],
        ),
        _ => (200, plan('Active')),
      };
    });

    await plans.refresh(null);
    await plans.loadMaterial(null);
    await plans.keep(null, ['m']);

    expect(paths, ['POST /v1/plan/refresh', 'GET /v1/memories/trim-material', 'PUT /v1/plan/kept-material']);
  });

  test('a refusal is named', () async {
    Future<PlanFailure> failureOf(int status, [String? code]) async {
      final result = await repository((_) => (status, {'code': code})).load(null);
      return (result as Err<PlanState, PlanFailure>).failure;
    }

    expect(await failureOf(409, 'Plans.ChoiceClosed'), isA<PlanChoiceClosed>());
    expect(await failureOf(409, 'Plans.AlreadyDecided'), isA<PlanAlreadyDecided>());
    expect(await failureOf(409), isA<PlanAlreadyDecided>());
    expect(await failureOf(403), isA<PlanNoAccess>());
    expect(await failureOf(500), isA<PlanServerError>());
  });
}
