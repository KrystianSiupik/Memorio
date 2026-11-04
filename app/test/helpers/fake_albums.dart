import 'package:memorio/core/result.dart';
import 'package:memorio/features/albums/albums.dart';
import 'package:memorio/features/memories/memories.dart';

/// Albums from memory; tests set the next answers.
class FakeAlbumsRepository implements AlbumsRepository {
  List<AlbumSummary> albums = [];
  List<SharedAlbum> shared = [];
  List<AlbumInvitation> invitations = [];
  AlbumDetails? details;
  AlbumSeats seats = const AlbumSeats(taken: 1, invited: 0, free: 9, invitations: []);
  List<KeptContribution> kept = [];
  List<EntrustedMemory> entrusted = [];
  List<OwnMemory> own = [];

  AlbumFailure? loadFailure;
  AlbumFailure? actionFailure;

  /// Every action, as "name:arguments", in order.
  final actions = <String>[];

  Result<T, AlbumFailure> _load<T>(T value) {
    final failure = loadFailure;
    return failure == null ? Ok(value) : Err(failure);
  }

  Future<Result<void, AlbumFailure>> _act(String action) async {
    actions.add(action);
    final failure = actionFailure;
    return failure == null ? const Ok(null) : Err(failure);
  }

  @override
  Future<Result<List<AlbumSummary>, AlbumFailure>> loadAlbums() async => _load(albums);

  @override
  Future<Result<List<SharedAlbum>, AlbumFailure>> loadShared() async => _load(shared);

  @override
  Future<Result<List<AlbumInvitation>, AlbumFailure>> loadInvitations() async => _load(invitations);

  @override
  Future<Result<AlbumDetails, AlbumFailure>> loadAlbum(String id) async {
    final known = details;
    return known == null ? const Err(AlbumNoAccess()) : _load(known);
  }

  @override
  Future<Result<AlbumSeats, AlbumFailure>> loadSeats(String id) async => _load(seats);

  @override
  Future<Result<List<KeptContribution>, AlbumFailure>> loadKept() async => _load(kept);

  @override
  Future<Result<List<EntrustedMemory>, AlbumFailure>> loadEntrusted() async => _load(entrusted);

  @override
  Future<Result<List<OwnMemory>, AlbumFailure>> loadOwnMemories() async => _load(own);

  @override
  Future<Result<void, AlbumFailure>> create(String id, String name) => _act('create:$name:$id');

  @override
  Future<Result<void, AlbumFailure>> invite(String albumId, String invitationId, String email) =>
      _act('invite:$email:$invitationId');

  @override
  Future<Result<void, AlbumFailure>> withdrawInvitation(String albumId, String invitationId) =>
      _act('withdraw:$invitationId');

  @override
  Future<Result<void, AlbumFailure>> removeParticipant(String albumId, String accountId) => _act('remove:$accountId');

  @override
  Future<Result<void, AlbumFailure>> nameSuccessor(String albumId, String accountId) => _act('successor:$accountId');

  @override
  Future<Result<void, AlbumFailure>> leave(String albumId, {required bool keepContributions}) =>
      _act('leave:$keepContributions');

  @override
  Future<Result<void, AlbumFailure>> addMemory(String albumId, String memoryId) => _act('addMemory:$memoryId');

  @override
  Future<Result<void, AlbumFailure>> addPhoto(String albumId, String memoryId, String photoId, PickedPhoto photo) =>
      _act('addPhoto:$memoryId');

  @override
  Future<Result<void, AlbumFailure>> tag(String albumId, String memoryId, String accountId) =>
      _act('tag:$memoryId:$accountId');

  @override
  Future<Result<void, AlbumFailure>> removeMyTag(String albumId, String memoryId) => _act('untag:$memoryId');

  @override
  Future<Result<void, AlbumFailure>> revokeKept(String albumId) => _act('revokeKept:$albumId');

  @override
  Future<Result<void, AlbumFailure>> entrust(String albumId, String curatorAccountId) =>
      _act('entrust:$curatorAccountId');

  @override
  Future<Result<void, AlbumFailure>> revokeEntrustment(String albumId) => _act('revokeEntrustment:$albumId');

  @override
  Future<Result<void, AlbumFailure>> arrange(String albumId, List<String> memoryIds) =>
      _act('arrange:${memoryIds.join(',')}');

  @override
  Future<Result<void, AlbumFailure>> accept(String invitationId) => _act('accept:$invitationId');

  @override
  Future<Result<void, AlbumFailure>> decline(String invitationId) => _act('decline:$invitationId');

  @override
  Future<Result<void, AlbumFailure>> blockInvitations() => _act('block');
}

const owner = AlbumMember(accountId: 'owner-1', email: 'wlasciciel@example.invalid', isOwner: true);
const me = AlbumMember(accountId: 'account-1', email: 'ja@example.invalid', isOwner: false);
const otherMember = AlbumMember(accountId: 'other-1', email: 'inny@example.invalid', isOwner: false);

const myMemory = AlbumMemory(
  id: 'memory-1',
  authorEmail: 'ja@example.invalid',
  isMine: true,
  placeName: 'Giewont',
  dateFrom: '2026-07-01',
  photos: 2,
  taggedAccountIds: ['account-1'],
);

AlbumDetails albumDetails({
  bool isOwner = false,
  List<AlbumMemory> memories = const [myMemory],
  int freeBytes = 1024 * 1024 * 1024,
  String? successor,
  String? curator,
}) => AlbumDetails(
  id: 'album-1',
  name: 'Rodzina',
  isOwner: isOwner,
  successorAccountId: successor,
  members: const [owner, me, otherMember],
  memories: memories,
  myMemories: 1,
  myPhotos: 2,
  freeBytes: freeBytes,
  limitBytes: 2 * 1024 * 1024 * 1024,
  myCuratorAccountId: curator,
);
