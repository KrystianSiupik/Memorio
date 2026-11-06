import '../../../core/result.dart';
import '../../memories/memories.dart' show PickedPhoto;
import 'album.dart';
import 'album_failure.dart';

abstract interface class AlbumsRepository {
  Future<Result<List<AlbumSummary>, AlbumFailure>> loadAlbums();

  Future<Result<List<SharedAlbum>, AlbumFailure>> loadShared();

  Future<Result<List<AlbumInvitation>, AlbumFailure>> loadInvitations();

  Future<Result<AlbumDetails, AlbumFailure>> loadAlbum(String id);

  Future<Result<AlbumSeats, AlbumFailure>> loadSeats(String id);

  Future<Result<List<KeptContribution>, AlbumFailure>> loadKept();

  Future<Result<List<EntrustedMemory>, AlbumFailure>> loadEntrusted();

  Future<Result<List<OwnMemory>, AlbumFailure>> loadOwnMemories();

  /// The token of the working link, or null.
  Future<Result<String?, AlbumFailure>> loadLink(String albumId);

  Future<Result<String, AlbumFailure>> shareByLink(String albumId);

  Future<Result<void, AlbumFailure>> revokeLink(String albumId);

  Future<Result<List<AlbumViewer>, AlbumFailure>> loadViewers(String albumId);

  Future<Result<void, AlbumFailure>> revokeAccess(String albumId, String accountId);

  Future<Result<void, AlbumFailure>> block(String albumId, String accountId);

  Future<Result<void, AlbumFailure>> unblock(String albumId, String accountId);

  Future<Result<List<AlbumViewer>, AlbumFailure>> loadBlocked(String albumId);

  Future<Result<AlbumLinkPreview, AlbumFailure>> previewLink(String token);

  /// The album it let the person into.
  Future<Result<String, AlbumFailure>> enterByLink(String token);

  Future<Result<void, AlbumFailure>> create(String id, String name);

  Future<Result<void, AlbumFailure>> invite(String albumId, String invitationId, String email);

  Future<Result<void, AlbumFailure>> withdrawInvitation(String albumId, String invitationId);

  Future<Result<void, AlbumFailure>> removeParticipant(String albumId, String accountId);

  Future<Result<void, AlbumFailure>> nameSuccessor(String albumId, String accountId);

  Future<Result<void, AlbumFailure>> leave(String albumId, {required bool keepContributions});

  /// D-307: the handover that waits, if any.
  Future<Result<AlbumHandover, AlbumFailure>> loadHandover(String albumId);

  Future<Result<void, AlbumFailure>> offerHandover(
    String albumId,
    String toAccountId, {
    required bool keepContributions,
  });

  Future<Result<void, AlbumFailure>> withdrawHandover(String albumId);

  Future<Result<void, AlbumFailure>> answerHandover(String albumId, {required bool accepted});

  Future<Result<void, AlbumFailure>> addMemory(String albumId, String memoryId);

  Future<Result<void, AlbumFailure>> addPhoto(String albumId, String memoryId, String photoId, PickedPhoto photo);

  Future<Result<void, AlbumFailure>> tag(String albumId, String memoryId, String accountId);

  Future<Result<void, AlbumFailure>> removeMyTag(String albumId, String memoryId);

  Future<Result<void, AlbumFailure>> revokeKept(String albumId);

  Future<Result<void, AlbumFailure>> entrust(String albumId, String curatorAccountId);

  Future<Result<void, AlbumFailure>> revokeEntrustment(String albumId);

  Future<Result<void, AlbumFailure>> arrange(String albumId, List<String> memoryIds);

  Future<Result<void, AlbumFailure>> accept(String invitationId);

  Future<Result<void, AlbumFailure>> decline(String invitationId);

  Future<Result<void, AlbumFailure>> blockInvitations();
}
