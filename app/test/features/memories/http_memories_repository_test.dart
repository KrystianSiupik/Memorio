import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/core/api/generated/clients/ai_client.dart';
import 'package:memorio/core/api/generated/clients/memories_client.dart';
import 'package:memorio/core/result.dart';
import 'package:memorio/features/memories/memories.dart';

import '../../helpers/fake_http.dart';

void main() {
  HttpMemoriesRepository repository(FakeHandler handler) {
    final dio = Dio(BaseOptions(baseUrl: 'https://api.example.invalid'))..httpClientAdapter = FakeHttpAdapter(handler);
    return HttpMemoriesRepository(memories: MemoriesClient(dio), ai: AiClient(dio), uploads: dio);
  }

  test('the trash comes with its numbers', () async {
    final result = await repository(
      (_) => (
        200,
        [
          {
            'id': 'm1',
            'placeName': 'Giewont',
            'dateFrom': '2026-07-01',
            'description': null,
            'photos': 2,
            'trashedAt': '2026-10-20T12:00:00Z',
            'daysLeft': 17,
          },
        ],
      ),
    ).loadTrash();

    final item = (result as Ok<List<TrashedMemory>, MemoryFailure>).value.single;
    expect(item.photos, 2);
    expect(item.daysLeft, 17);
  });

  group('failures by what the backend answered', () {
    final cases = <String, (int, Object?, Type)>{
      'already decided': (409, {'code': 'Archive.AlreadyDecided'}, MemoryAlreadyDecided),
      'no access': (403, {'code': 'Archive.NoAccess'}, MemoryNoAccess),
      'space': (409, {'code': 'Archive.SpaceLimitExceeded'}, MemorySpaceFull),
      'terms': (409, {'code': 'Archive.TermsNotAccepted'}, MemoryTermsNotAccepted),
      'future': (400, {'code': 'Archive.DateInFuture'}, MemoryDateInFuture),
      'bad input': (400, {'code': 'Archive.InvalidDate'}, MemoryInvalidInput),
      'server': (500, null, MemoryServerError),
    };

    cases.forEach((name, answer) {
      test(name, () async {
        final (status, body, expected) = answer;
        final result = await repository((_) => (status, body)).restore('m1');

        expect((result as Err<void, MemoryFailure>).failure.runtimeType, expected);
      });
    });
  });
}
