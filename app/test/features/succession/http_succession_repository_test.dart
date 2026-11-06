import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/core/api/generated/clients/access_client.dart';
import 'package:memorio/core/api/generated/clients/albums_client.dart';
import 'package:memorio/core/api/generated/clients/memories_client.dart';
import 'package:memorio/core/api/generated/clients/succession_client.dart';
import 'package:memorio/core/result.dart';
import 'package:memorio/features/memories/memories.dart' show PickedPhoto;
import 'package:memorio/features/succession/succession.dart';

import '../../helpers/fake_http.dart';

void main() {
  HttpSuccessionRepository repository(FakeHandler handler) {
    final dio = Dio(BaseOptions(baseUrl: 'https://api.example.invalid'))..httpClientAdapter = FakeHttpAdapter(handler);
    return HttpSuccessionRepository(
      succession: SuccessionClient(dio),
      albums: AlbumsClient(dio),
      access: AccessClient(dio),
      memories: MemoriesClient(dio),
      uploads: dio,
    );
  }

  test('an offer comes with its album, deadline and what the founder brought', () async {
    final result = await repository(
      (_) => (
        200,
        {'id': 'c1', 'albumId': 'a1', 'albumName': 'Rodzina', 'answerBy': '2030-02-01T12:00:00Z', 'founderMemories': 3},
      ),
    ).loadOffer('c1');

    final offer = (result as Ok<TakeoverOffer, SuccessionFailure>).value;
    expect(offer.albumName, 'Rodzina');
    expect(offer.answerBy, DateTime.utc(2030, 2, 1, 12));
    expect(offer.founderMemories, 3);
  });

  test('a report sends the file first and names it', () async {
    final paths = <String>[];
    final result = await repository((request) {
      paths.add('${request.method} ${request.path}');
      return request.path.endsWith('/uploads')
          ? (200, {'key': 'uploads/x', 'url': 'https://storage.example.invalid/x', 'contentType': 'image/jpeg'})
          : (204, null);
    }).report('r1', 'a1', PickedPhoto(name: 'a.jpg', contentType: 'image/jpeg', read: () async => [1]));

    expect(result, isA<Ok<void, SuccessionFailure>>());
    expect(paths, [
      'POST /v1/succession/uploads',
      'PUT https://storage.example.invalid/x',
      'POST /v1/albums/a1/death-reports',
    ]);
  });

  test('a refusal is named', () async {
    Future<SuccessionFailure> failureOf(int status, [String? code]) async {
      final result = await repository((_) => (status, {'code': code})).answer('c1', accepted: true);
      return (result as Err<void, SuccessionFailure>).failure;
    }

    expect(await failureOf(403, 'Succession.NotSuccessor'), isA<SuccessionNotSuccessor>());
    expect(await failureOf(409, 'Succession.AlreadyDecided'), isA<SuccessionAlreadyDecided>());
    expect(await failureOf(409, 'Succession.AlreadyReported'), isA<SuccessionAlreadyReported>());
    expect(await failureOf(409, 'Album.TakeoverReserved'), isA<SuccessionReservedByOther>());
    expect(await failureOf(400, 'Succession.InvalidEmail'), isA<SuccessionInvalidEmail>());
    expect(await failureOf(403), isA<SuccessionNoAccess>());
    expect(await failureOf(500), isA<SuccessionServerError>());
  });

  test('the takeover, the contact and the archive have their addresses', () async {
    final paths = <String>[];
    final succession = repository((request) {
      paths.add('${request.method} ${request.path}');
      return switch (request.path) {
        '/v1/albums/a1/takeover' => (
          200,
          {'isOwnerless': true, 'canReserve': true, 'reservedByMe': false, 'reservedUntil': null},
        ),
        '/v1/legacy-contact' when request.method == 'GET' => (200, {'email': 'ania@example.invalid'}),
        '/v1/legacy-archives' => (
          200,
          [
            {'ownerAccountId': 'o1', 'email': null},
          ],
        ),
        '/v1/memories/legacy/o1' => (
          200,
          [
            {'id': 'm1', 'placeName': 'Giewont', 'dateFrom': '1990-07-01'},
          ],
        ),
        _ => (204, null),
      };
    });

    expect(((await succession.loadTakeover('a1')) as Ok<AlbumTakeover, SuccessionFailure>).value.canReserve, isTrue);
    await succession.reserve('a1');
    expect(((await succession.loadContact()) as Ok<String?, SuccessionFailure>).value, 'ania@example.invalid');
    await succession.nameContact('kasia@example.invalid');
    await succession.removeContact();
    expect(
      ((await succession.loadArchives()) as Ok<List<LegacyArchive>, SuccessionFailure>).value.single.ownerAccountId,
      'o1',
    );
    expect(
      ((await succession.loadArchive('o1')) as Ok<List<LegacyMemory>, SuccessionFailure>).value.single.placeName,
      'Giewont',
    );

    expect(
      paths,
      containsAll(['POST /v1/albums/a1/takeover-reservation', 'PUT /v1/legacy-contact', 'DELETE /v1/legacy-contact']),
    );
  });
}
