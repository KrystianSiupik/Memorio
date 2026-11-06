import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/core/api/generated/clients/moderation_client.dart';
import 'package:memorio/core/result.dart';
import 'package:memorio/features/moderation/moderation.dart';

import '../../helpers/fake_http.dart';

void main() {
  HttpModerationRepository repository(FakeHandler handler) {
    final dio = Dio(BaseOptions(baseUrl: 'https://api.example.invalid'))..httpClientAdapter = FakeHttpAdapter(handler);
    return HttpModerationRepository(moderation: ModerationClient(dio));
  }

  test('a report sends what the person chose', () async {
    Object? sent;
    final result = await repository((request) {
      sent = request.data;
      return (200, {'id': 'r1'});
    }).report('r1', ReportTarget.memory, 'm1', ReportReason.threat, 'Grozi.');

    expect(result, isA<Ok<void, ModerationFailure>>());
    expect(sent.toString(), contains('Threat'));
    expect(sent.toString(), contains('Memory'));
  });

  test('one\'s own content and content no longer seen say so', () async {
    expect(
      (await repository(
        (_) => (400, {'code': 'Moderation.OwnContent'}),
      ).report('r', ReportTarget.album, 'a', ReportReason.other, 'x') as Err).failure,
      isA<ModerationOwnContent>(),
    );
    expect(
      (await repository(
        (_) => (403, {'code': 'Moderation.NoAccess'}),
      ).report('r', ReportTarget.album, 'a', ReportReason.other, 'x') as Err).failure,
      isA<ModerationNoAccess>(),
    );
  });

  test('my reports and the decisions about mine come with their reasons', () async {
    final result = await repository(
      (_) => (
        200,
        {
          'reports': [
            {
              'id': 'r1',
              'target': 'Album',
              'targetId': 'a1',
              'reason': 'Copyright',
              'state': 'Open',
              'verdict': null,
              'statement': null,
              'reportedAt': '2026-11-01T12:00:00Z',
              'decidedAt': null,
            },
          ],
          'aboutMine': [
            {
              'id': 'r2',
              'target': 'Memory',
              'targetId': 'm1',
              'reason': 'PersonalData',
              'category': 'Disputed',
              'state': 'Decided',
              'hidden': true,
              'verdict': 'Removed',
              'statement': 'Dane.',
              'reportedAt': '2026-11-01T12:00:00Z',
              'decidedAt': '2026-11-02T12:00:00Z',
              'appealUntil': '2027-05-01T12:00:00Z',
              'appealReversed': null,
              'appealStatement': null,
            },
          ],
        },
      ),
    ).loadMine();

    final mine = (result as Ok<MyModeration, ModerationFailure>).value;
    expect(mine.reports.single.reason, ReportReason.copyright);
    final about = mine.aboutMine.single;
    expect(about.reason, ReportReason.personalData);
    expect(about.statement, 'Dane.');
    expect(about.canAppeal(DateTime.utc(2026, 12, 1)), isTrue);
  });
}
