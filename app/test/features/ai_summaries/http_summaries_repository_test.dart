import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/core/api/generated/clients/ai_client.dart';
import 'package:memorio/core/result.dart';
import 'package:memorio/features/ai_summaries/ai_summaries.dart';

import '../../helpers/fake_http.dart';

void main() {
  HttpSummariesRepository repository(FakeHandler handler) {
    final dio = Dio(BaseOptions(baseUrl: 'https://api.example.invalid'))..httpClientAdapter = FakeHttpAdapter(handler);
    return HttpSummariesRepository(ai: AiClient(dio));
  }

  Map<String, Object?> summary(String state) => {
    'id': 's1',
    'accountId': 'a1',
    'albumId': 'album-1',
    'from': '2026-07-01',
    'to': '2026-07-31',
    'state': state,
    'text': 'W lipcu byliście w górach.',
    'sentences': [
      {
        'text': 'W lipcu byliście w górach.',
        'memoryIds': ['m1'],
      },
    ],
    'isAiGenerated': true,
    'model': 'test-model',
    'promptVersion': 'period-summary@2',
    'generatedAt': '2026-11-09T10:00:00Z',
    'usedMemories': 2,
    'leftOutMemories': 1,
    'leftOutPeople': 1,
    'droppedSentences': 3,
    'addedToAlbum': false,
    'invalidReason': null,
  };

  test('a summary comes with its sentences, state and what it was made from', () async {
    final sent = <String>[];
    final result = await repository((request) {
      sent.add('${request.method} ${request.path} ${request.data}');
      return (200, summary('Draft'));
    }).request('s1', 'album-1', '2026-07-01', '2026-07-31');

    final value = (result as Ok<PeriodSummary, SummaryFailure>).value;
    expect(value.state, SummaryState.draft);
    expect(value.sentences, ['W lipcu byliście w górach.']);
    expect(value.usedMemories, 2);
    expect(value.leftOutPeople, 1);
    expect(value.droppedSentences, 3);
    expect(sent.single, startsWith('POST /v1/ai/summaries '));
    expect(sent.single, contains('s1'));
  });

  test('each step has its address and every state its name', () async {
    final paths = <String>[];
    final summaries = repository((request) {
      paths.add('${request.method} ${request.path}');
      return switch (request.path) {
        final path when path.endsWith('ai-consent') && request.method == 'GET' => (200, {'given': true}),
        final path when path.endsWith('ai-consent') => (204, null),
        final path when path.endsWith('ai-summaries') => (200, [summary('Saved')]),
        final path when path.endsWith('save') => (200, summary('Saved')),
        _ => (200, summary('Invalidated')),
      };
    });

    expect((await summaries.consent('album-1') as Ok<bool, SummaryFailure>).value, isTrue);
    await summaries.setConsent('album-1', given: true);
    await summaries.setConsent('album-1', given: false);
    expect((await summaries.save('s1') as Ok<PeriodSummary, SummaryFailure>).value.state, SummaryState.saved);
    expect(
      (await summaries.addToAlbum('s1') as Ok<PeriodSummary, SummaryFailure>).value.state,
      SummaryState.invalidated,
    );
    expect((await summaries.inAlbum('album-1') as Ok<List<PeriodSummary>, SummaryFailure>).value, hasLength(1));
    expect(paths, [
      'GET /v1/albums/album-1/ai-consent',
      'PUT /v1/albums/album-1/ai-consent',
      'DELETE /v1/albums/album-1/ai-consent',
      'POST /v1/ai/summaries/s1/save',
      'POST /v1/ai/summaries/s1/album',
      'GET /v1/albums/album-1/ai-summaries',
    ]);
  });

  test('failures: the AI rules by their codes, the rest by their status', () async {
    final cases = <(int, String?), SummaryFailure>{
      (403, 'AiNarration.ConsentRequired'): const SummaryConsentRequired(),
      (400, 'AiNarration.NoMaterials'): const SummaryNoMaterials(),
      (409, 'AiNarration.LimitReached'): const SummaryLimitReached(),
      (500, 'AiNarration.NothingTold'): const SummaryNothingTold(),
      (409, 'AiNarration.MaterialsChanged'): const SummaryMaterialsChanged(),
      (409, 'AiNarration.Invalidated'): const SummaryInvalidated(),
      (400, 'AiNarration.InvalidPeriod'): const SummaryInvalidPeriod(),
      (403, 'AiNarration.NotMember'): const SummaryNoAccess(),
      (404, null): const SummaryNoAccess(),
      (409, null): const SummaryAlreadyDecided(),
      (500, null): const SummaryServerError(),
    };
    for (final MapEntry(key: (status, code), value: expected) in cases.entries) {
      final result = await repository((_) => (status, {'code': code})).save('s1');
      expect((result as Err<PeriodSummary, SummaryFailure>).failure, expected, reason: '$status $code');
    }
  });
}
