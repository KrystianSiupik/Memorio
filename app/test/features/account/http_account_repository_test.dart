import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/core/api/generated/clients/account_client.dart';
import 'package:memorio/core/result.dart';
import 'package:memorio/features/account/account.dart';

import '../../helpers/fake_http.dart';

void main() {
  HttpAccountRepository repository(FakeHandler handler) {
    final dio = Dio(BaseOptions(baseUrl: 'https://api.example.invalid'))..httpClientAdapter = FakeHttpAdapter(handler);
    return HttpAccountRepository(account: AccountClient(dio));
  }

  test('the overview joins the effects with the open window', () async {
    final result = await repository(
      (request) => request.path == '/v1/account/deletion'
          ? (200, {'windowEndsAt': '2026-12-03T10:00:00Z'})
          : (
              200,
              {'memories': 1, 'photos': 2, 'ownAlbums': 3, 'ownAlbumsWithoutSuccessor': 1, 'otherPeoplesAlbums': 4},
            ),
    ).loadOverview();

    final overview = (result as Ok<AccountOverview, AccountFailure>).value;
    expect(overview.impact.photos, 2);
    expect(overview.impact.ownAlbumsWithoutSuccessor, 1);
    expect(overview.deletionWindowEndsAt, DateTime.utc(2026, 12, 3, 10));
  });

  test('accepting the terms sends the version the backend offers', () async {
    final sent = <Object?>[];
    final result = await repository((request) {
      if (request.path == '/v1/account/me') {
        return (200, {'accountId': 'account-1', 'email': null, 'currentTermsVersion': '2026-11'});
      }
      sent.add(request.data);
      return (204, null);
    }).acceptCurrentTerms();

    expect(result, isA<Ok<void, AccountFailure>>());
    expect(sent.single, containsPair('version', '2026-11'));
  });

  test('requesting the deletion sends the consent', () async {
    final sent = <Object?>[];
    await repository((request) {
      sent.add(request.data);
      return (200, {'windowEndsAt': '2026-12-03T10:00:00Z'});
    }).requestDeletion(consentsToAlbumExpiry: true);

    expect(sent.single, containsPair('consentsToAlbumExpiry', true));
  });

  group('failures by what the backend answered', () {
    final cases = <String, (int, Object?, Type)>{
      'already decided': (409, {'code': 'Account.DeletionAlreadyDecided'}, DeletionAlreadyDecided),
      'album without a successor': (409, {'code': 'Account.AlbumWithoutSuccessor'}, AlbumWithoutSuccessor),
      'no access': (403, null, AccountForbidden),
      'signed out': (401, null, AccountForbidden),
      'server': (500, null, AccountServerError),
    };

    cases.forEach((name, answer) {
      test(name, () async {
        final (status, body, expected) = answer;
        final result = await repository((_) => (status, body)).cancelDeletion();

        expect((result as Err<void, AccountFailure>).failure.runtimeType, expected);
      });
    });

    test('no answer at all means offline', () async {
      final dio = Dio(BaseOptions(baseUrl: 'https://api.example.invalid'))..httpClientAdapter = _Unreachable();
      final result = await HttpAccountRepository(account: AccountClient(dio)).cancelDeletion();

      expect((result as Err<void, AccountFailure>).failure, isA<AccountOffline>());
    });
  });
}

class _Unreachable extends FakeHttpAdapter {
  _Unreachable() : super((_) => (0, null));

  @override
  Future<ResponseBody> fetch(RequestOptions options, Stream<List<int>>? requestStream, Future<void>? cancelFuture) =>
      throw DioException.connectionError(requestOptions: options, reason: 'unreachable');
}
