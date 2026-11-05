import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/core/api/generated/clients/access_client.dart';
import 'package:memorio/core/api/generated/clients/albums_client.dart';
import 'package:memorio/core/api/generated/clients/memories_client.dart';
import 'package:memorio/core/api/generated/clients/notifications_client.dart';
import 'package:memorio/core/result.dart';
import 'package:memorio/features/albums/albums.dart';

import '../../helpers/fake_http.dart';

void main() {
  HttpAlbumsRepository repository(FakeHandler handler) {
    final dio = Dio(BaseOptions(baseUrl: 'https://api.example.invalid'))..httpClientAdapter = FakeHttpAdapter(handler);
    return HttpAlbumsRepository(
      albums: AlbumsClient(dio),
      access: AccessClient(dio),
      notifications: NotificationsClient(dio),
      memories: MemoriesClient(dio),
      uploads: dio,
    );
  }

  test('an album comes with its members, memories, numbers and pool', () async {
    final result = await repository(
      (_) => (
        200,
        {
          'id': 'a1',
          'name': 'Rodzina',
          'isOwner': false,
          'isMember': true,
          'isReadOnly': false,
          'successorAccountId': null,
          'members': [
            {'accountId': 'o1', 'email': 'o@example.invalid', 'isOwner': true},
            {'accountId': 'p1', 'email': null, 'isOwner': false},
          ],
          'memories': [
            {
              'id': 'm1',
              'authorAccountId': 'p1',
              'authorEmail': null,
              'isMine': true,
              'placeName': 'Giewont',
              'dateFrom': '2026-07-01',
              'photos': 2,
              'taggedAccountIds': ['o1'],
            },
          ],
          'myMemories': 1,
          'myPhotos': 2,
          'freeBytes': 0,
          'limitBytes': 2147483648,
          'myCuratorAccountId': 'o1',
        },
      ),
    ).loadAlbum('a1');

    final album = (result as Ok<AlbumDetails, AlbumFailure>).value;
    expect(album.members.last.label, 'p1');
    expect(album.memories.single.photos, 2);
    expect(album.memories.single.taggedAccountIds, ['o1']);
    expect(album.poolIsFull, isTrue);
    expect(album.limitBytes, 2147483648);
    expect(album.myCuratorAccountId, 'o1');
  });

  test('seats come with who is invited', () async {
    final result = await repository(
      (_) => (
        200,
        {
          'taken': 2,
          'invited': 1,
          'free': 7,
          'invitations': [
            {'id': 'i1', 'email': 'n@example.invalid'},
          ],
        },
      ),
    ).loadSeats('a1');

    final seats = (result as Ok<AlbumSeats, AlbumFailure>).value;
    expect((seats.taken, seats.invited, seats.free), (2, 1, 7));
    expect(seats.invitations.single.email, 'n@example.invalid');
  });

  test('an invitation goes to the address of the album under its id', () async {
    late RequestOptions sent;
    final result = await repository((request) {
      sent = request;
      return (204, null);
    }).invite('a1', 'i1', 'n@example.invalid');

    expect(result, isA<Ok<void, AlbumFailure>>());
    expect(sent.path, contains('/v1/albums/a1/invitations'));
    expect(sent.data, containsPair('email', 'n@example.invalid'));
    expect(sent.data, containsPair('id', 'i1'));
  });

  group('failures by what the backend answered', () {
    final cases = <String, (int, Object?, Type)>{
      'blocked person': (403, {'code': 'Access.Blocked'}, AlbumBlocked),
      'link gone': (409, {'code': 'Access.LinkRevoked'}, AlbumLinkGone),
      'only the owner': (403, {'code': 'Access.OnlyOwnerBlocksParticipant'}, AlbumOnlyOwnerBlocks),
      'access decided': (409, {'code': 'Access.AlreadyDecided'}, AlbumAlreadyDecided),
      'no seat': (409, {'code': 'Album.NoFreeSeats'}, AlbumNoFreeSeats),
      'invitation limit': (409, {'code': 'Album.InvitationLimitReached'}, AlbumInvitationLimit),
      'blocked': (409, {'code': 'Album.AddressBlocked'}, AlbumAddressBlocked),
      'invitation gone': (409, {'code': 'Album.InvitationGone'}, AlbumInvitationGone),
      'pool': (409, {'code': 'Album.PoolFull'}, AlbumPoolFull),
      'owner': (409, {'code': 'Album.OwnerCannotLeave'}, AlbumOwnerCannotLeave),
      'decided': (409, {'code': 'Album.AlreadyDecided'}, AlbumAlreadyDecided),
      'succession decided': (409, {'code': 'Succession.AlreadyDecided'}, AlbumAlreadyDecided),
      'another conflict': (409, {'code': 'Album.IdTaken'}, AlbumAlreadyDecided),
      'no access': (403, {'code': 'Album.NoAccess'}, AlbumNoAccess),
      'bad input': (400, {'code': 'Album.InvalidName'}, AlbumInvalidInput),
      'server': (500, null, AlbumServerError),
    };

    cases.forEach((name, answer) {
      test(name, () async {
        final result = await repository((_) => (answer.$1, answer.$2)).create('a1', 'Rodzina');

        expect((result as Err<void, AlbumFailure>).failure.runtimeType, answer.$3);
      });
    });
  });
}
