import 'package:dio/dio.dart';

import '../../../core/api/generated/clients/access_client.dart';
import '../../../core/api/generated/clients/albums_client.dart';
import '../../../core/api/generated/clients/memories_client.dart';
import '../../../core/api/generated/clients/notifications_client.dart';
import '../../../core/api/generated/models/add_album_photo_request.dart';
import '../../../core/api/generated/models/add_memory_request.dart';
import '../../../core/api/generated/models/arrange_request.dart';
import '../../../core/api/generated/models/create_album_request.dart';
import '../../../core/api/generated/models/entrust_request.dart';
import '../../../core/api/generated/models/invite_request.dart';
import '../../../core/api/generated/models/leave_request.dart';
import '../../../core/api/generated/models/name_successor_request.dart';
import '../../../core/api/generated/models/offer_handover_request.dart';
import '../../../core/api/generated/models/start_upload_request.dart';
import '../../../core/result.dart';
import '../../memories/memories.dart' show PickedPhoto;
import '../domain/album.dart';
import '../domain/album_failure.dart';
import '../domain/albums_repository.dart';

/// [AlbumsRepository] on API v1. The contract describes numbers as "integer or string", so the
/// generated models have them dynamic; the backend always sends numbers (D-212).
class HttpAlbumsRepository implements AlbumsRepository {
  const HttpAlbumsRepository({
    required this.albums,
    required this.access,
    required this.notifications,
    required this.memories,
    required this.uploads,
  });

  final AlbumsClient albums;
  final AccessClient access;
  final NotificationsClient notifications;
  final MemoriesClient memories;

  /// Sends files straight to the storage: its address is signed, so no sign-in and no API headers.
  final Dio uploads;

  @override
  Future<Result<List<AlbumSummary>, AlbumFailure>> loadAlbums() => _attempt(() async {
    final items = await albums.listAlbums();
    return [
      for (final item in items)
        AlbumSummary(
          id: item.id,
          name: item.name,
          isOwner: item.isOwner,
          members: item.members as int,
          memories: item.memories as int,
        ),
    ];
  });

  @override
  Future<Result<List<SharedAlbum>, AlbumFailure>> loadShared() => _attempt(() async {
    final items = await access.getSharedWithMe();
    return [for (final item in items) SharedAlbum(id: item.id, name: item.name)];
  });

  @override
  Future<Result<List<AlbumInvitation>, AlbumFailure>> loadInvitations() => _attempt(() async {
    final items = await albums.listInvitations();
    return [
      for (final item in items)
        AlbumInvitation(id: item.id, albumId: item.albumId, albumName: item.albumName, invitedBy: item.invitedBy),
    ];
  });

  @override
  Future<Result<AlbumDetails, AlbumFailure>> loadAlbum(String id) => _attempt(() async {
    final album = await albums.getAlbum(id: id);
    return AlbumDetails(
      id: album.id,
      name: album.name,
      isOwner: album.isOwner,
      isMember: album.isMember,
      isReadOnly: album.isReadOnly,
      isOwnerless: album.isOwnerless,
      successorAccountId: album.successorAccountId,
      members: [
        for (final member in album.members)
          AlbumMember(accountId: member.accountId, email: member.email, isOwner: member.isOwner),
      ],
      memories: [
        for (final memory in album.memories)
          AlbumMemory(
            id: memory.id,
            authorEmail: memory.authorEmail,
            isMine: memory.isMine,
            placeName: memory.placeName,
            dateFrom: memory.dateFrom,
            photos: memory.photos as int,
            taggedAccountIds: memory.taggedAccountIds,
          ),
      ],
      myMemories: album.myMemories as int,
      myPhotos: album.myPhotos as int,
      freeBytes: album.freeBytes as int,
      limitBytes: album.limitBytes as int,
      myCuratorAccountId: album.myCuratorAccountId,
    );
  });

  @override
  Future<Result<AlbumSeats, AlbumFailure>> loadSeats(String id) => _attempt(() async {
    final seats = await albums.getAlbumSeats(id: id);
    return AlbumSeats(
      taken: seats.taken as int,
      invited: seats.invited as int,
      free: seats.free as int,
      invitations: [for (final item in seats.invitations) AlbumInvited(id: item.id, email: item.email)],
    );
  });

  @override
  Future<Result<List<KeptContribution>, AlbumFailure>> loadKept() => _attempt(() async {
    final items = await albums.listKeptContributions();
    return [
      for (final item in items)
        KeptContribution(
          albumId: item.albumId,
          albumName: item.albumName,
          chosenByAuthor: item.chosenByAuthor,
          memories: item.memories as int,
        ),
    ];
  });

  @override
  Future<Result<List<EntrustedMemory>, AlbumFailure>> loadEntrusted() => _attempt(() async {
    final items = await albums.listEntrusted();
    return [
      for (final item in items)
        EntrustedMemory(
          albumId: item.albumId,
          albumName: item.albumName,
          memoryId: item.memoryId,
          placeName: item.placeName,
          dateFrom: item.dateFrom,
        ),
    ];
  });

  @override
  Future<Result<List<OwnMemory>, AlbumFailure>> loadOwnMemories() => _attempt(() async {
    final items = await memories.listMyMemories();
    return [for (final item in items) OwnMemory(id: item.id, placeName: item.placeName, dateFrom: item.dateFrom)];
  });
  @override
  Future<Result<String?, AlbumFailure>> loadLink(String albumId) =>
      _attempt(() async => (await access.getAlbumLink(id: albumId)).token);

  @override
  Future<Result<String, AlbumFailure>> shareByLink(String albumId) =>
      _attempt(() async => (await access.shareAlbumByLink(id: albumId)).token!);

  @override
  Future<Result<void, AlbumFailure>> revokeLink(String albumId) => _attempt(() => access.revokeAlbumLink(id: albumId));

  @override
  Future<Result<List<AlbumViewer>, AlbumFailure>> loadViewers(String albumId) => _attempt(() async {
    final items = await access.getAlbumViewers(id: albumId);
    return [for (final item in items) AlbumViewer(accountId: item.accountId, email: item.email)];
  });

  @override
  Future<Result<void, AlbumFailure>> revokeAccess(String albumId, String accountId) =>
      _attempt(() => access.revokeAlbumAccess(id: albumId, accountId: accountId));

  @override
  Future<Result<void, AlbumFailure>> block(String albumId, String accountId) =>
      _attempt(() => access.blockUser(id: albumId, accountId: accountId));

  @override
  Future<Result<void, AlbumFailure>> unblock(String albumId, String accountId) =>
      _attempt(() => access.unblockUser(id: albumId, accountId: accountId));

  @override
  Future<Result<List<AlbumViewer>, AlbumFailure>> loadBlocked(String albumId) => _attempt(() async {
    final items = await access.getAlbumBlocks(id: albumId);
    return [for (final item in items) AlbumViewer(accountId: item.accountId, email: item.email)];
  });

  @override
  Future<Result<AlbumLinkPreview, AlbumFailure>> previewLink(String token) => _attempt(() async {
    final preview = await access.previewAlbumLink(token: token);
    return AlbumLinkPreview(albumId: preview.albumId, albumName: preview.albumName);
  });

  @override
  Future<Result<String, AlbumFailure>> enterByLink(String token) =>
      _attempt(() async => (await access.enterAlbumByLink(token: token)).albumId);

  @override
  Future<Result<void, AlbumFailure>> create(String id, String name) => _attempt(
    () => albums.createAlbum(
      body: CreateAlbumRequest(id: id, name: name),
    ),
  );

  @override
  Future<Result<void, AlbumFailure>> invite(String albumId, String invitationId, String email) => _attempt(
    () => albums.inviteToAlbum(
      id: albumId,
      body: InviteRequest(id: invitationId, email: email),
    ),
  );

  @override
  Future<Result<void, AlbumFailure>> withdrawInvitation(String albumId, String invitationId) =>
      _attempt(() => albums.withdrawInvitation(id: albumId, invitationId: invitationId));

  @override
  Future<Result<void, AlbumFailure>> removeParticipant(String albumId, String accountId) =>
      _attempt(() => albums.removeParticipant(id: albumId, accountId: accountId));

  @override
  Future<Result<void, AlbumFailure>> nameSuccessor(String albumId, String accountId) => _attempt(
    () => albums.nameSuccessor(
      id: albumId,
      body: NameSuccessorRequest(accountId: accountId),
    ),
  );

  @override
  Future<Result<void, AlbumFailure>> leave(String albumId, {required bool keepContributions}) => _attempt(
    () => albums.leaveAlbum(
      id: albumId,
      body: LeaveRequest(keepContributions: keepContributions),
    ),
  );

  @override
  Future<Result<void, AlbumFailure>> addMemory(String albumId, String memoryId) => _attempt(
    () => albums.addMemoryToAlbum(
      id: albumId,
      body: AddMemoryRequest(memoryId: memoryId),
    ),
  );

  @override
  Future<Result<void, AlbumFailure>> addPhoto(String albumId, String memoryId, String photoId, PickedPhoto photo) =>
      _attempt(() async {
        final upload = await memories.startPhotoUpload(body: StartUploadRequest(contentType: photo.contentType));
        await uploads.put<void>(
          upload.url,
          data: await photo.read(),
          options: Options(contentType: upload.contentType),
        );
        await albums.addPhotoInAlbum(
          id: albumId,
          memoryId: memoryId,
          body: AddAlbumPhotoRequest(photoId: photoId, uploadKey: upload.key),
        );
      });

  @override
  Future<Result<void, AlbumFailure>> tag(String albumId, String memoryId, String accountId) =>
      _attempt(() => albums.tagParticipant(id: albumId, memoryId: memoryId, accountId: accountId));

  @override
  Future<Result<void, AlbumFailure>> removeMyTag(String albumId, String memoryId) =>
      _attempt(() => albums.removeOwnTag(id: albumId, memoryId: memoryId));

  @override
  Future<Result<void, AlbumFailure>> revokeKept(String albumId) =>
      _attempt(() => albums.revokeKeptContributions(id: albumId));

  @override
  Future<Result<void, AlbumFailure>> entrust(String albumId, String curatorAccountId) => _attempt(
    () => albums.entrustToCurator(
      id: albumId,
      body: EntrustRequest(curatorAccountId: curatorAccountId),
    ),
  );

  @override
  Future<Result<void, AlbumFailure>> revokeEntrustment(String albumId) =>
      _attempt(() => albums.revokeEntrustment(id: albumId));

  @override
  Future<Result<void, AlbumFailure>> arrange(String albumId, List<String> memoryIds) => _attempt(
    () => albums.arrangeEntrusted(
      id: albumId,
      body: ArrangeRequest(memoryIds: memoryIds),
    ),
  );

  @override
  Future<Result<void, AlbumFailure>> accept(String invitationId) => _attempt(() async {
    await albums.acceptInvitation(id: invitationId);
  });

  @override
  Future<Result<void, AlbumFailure>> decline(String invitationId) =>
      _attempt(() => albums.declineInvitation(id: invitationId));

  @override
  Future<Result<AlbumHandover, AlbumFailure>> loadHandover(String albumId) => _attempt(() async {
    final handover = await albums.getAlbumHandover(id: albumId);
    return AlbumHandover(
      toAccountId: handover.toAccountId,
      forMe: handover.forMe,
      keepsContributions: handover.keepsContributions,
      until: handover.until,
    );
  });

  @override
  Future<Result<void, AlbumFailure>> offerHandover(
    String albumId,
    String toAccountId, {
    required bool keepContributions,
  }) => _attempt(
    () => albums.offerAlbumHandover(
      id: albumId,
      body: OfferHandoverRequest(toAccountId: toAccountId, keepContributions: keepContributions),
    ),
  );

  @override
  Future<Result<void, AlbumFailure>> withdrawHandover(String albumId) =>
      _attempt(() => albums.withdrawAlbumHandover(id: albumId));

  @override
  Future<Result<void, AlbumFailure>> answerHandover(String albumId, {required bool accepted}) =>
      _attempt(() => accepted ? albums.acceptAlbumHandover(id: albumId) : albums.refuseAlbumHandover(id: albumId));

  @override
  Future<Result<void, AlbumFailure>> blockInvitations() => _attempt(() => notifications.blockInvitations());

  Future<Result<T, AlbumFailure>> _attempt<T>(Future<T> Function() call) async {
    try {
      return Ok(await call());
    } on DioException catch (error) {
      return Err(_failureOf(error));
    }
  }

  static AlbumFailure _failureOf(DioException error) {
    final response = error.response;
    if (response == null) {
      return const AlbumOffline();
    }
    final body = response.data;
    final code = body is Map ? body['code'] : null;
    return switch (code) {
      'Access.Blocked' => const AlbumBlocked(),
      'Access.LinkRevoked' => const AlbumLinkGone(),
      'Access.OnlyOwnerBlocksParticipant' => const AlbumOnlyOwnerBlocks(),
      'Album.NoFreeSeats' => const AlbumNoFreeSeats(),
      'Album.InvitationLimitReached' => const AlbumInvitationLimit(),
      'Album.AddressBlocked' => const AlbumAddressBlocked(),
      'Album.InvitationGone' => const AlbumInvitationGone(),
      'Album.PoolFull' || 'Archive.SpaceLimitExceeded' => const AlbumPoolFull(),
      'Album.ReadOnly' => const AlbumReadOnly(),
      'Album.OwnerCannotLeave' => const AlbumOwnerCannotLeave(),
      'Album.AlreadyDecided' || 'Succession.AlreadyDecided' || 'Access.AlreadyDecided' => const AlbumAlreadyDecided(),
      _ => switch (response.statusCode) {
        403 || 404 => const AlbumNoAccess(),
        400 => const AlbumInvalidInput(),
        409 => const AlbumAlreadyDecided(),
        _ => const AlbumServerError(),
      },
    };
  }
}
