/// An album on the list.
class AlbumSummary {
  const AlbumSummary({
    required this.id,
    required this.name,
    required this.isOwner,
    required this.members,
    required this.memories,
  });

  final String id;
  final String name;
  final bool isOwner;
  final int members;
  final int memories;
}

/// An album someone may read without being in it.
class SharedAlbum {
  const SharedAlbum({required this.id, required this.name});

  final String id;
  final String name;
}

class AlbumMember {
  const AlbumMember({required this.accountId, required this.email, required this.isOwner});

  final String accountId;
  final String? email;
  final bool isOwner;

  String get label => email ?? accountId;
}

class AlbumMemory {
  const AlbumMemory({
    required this.id,
    required this.authorEmail,
    required this.isMine,
    required this.placeName,
    required this.dateFrom,
    required this.photos,
    required this.taggedAccountIds,
  });

  final String id;
  final String? authorEmail;
  final bool isMine;
  final String placeName;
  final String dateFrom;
  final int photos;
  final List<String> taggedAccountIds;
}

/// S10.
class AlbumDetails {
  const AlbumDetails({
    required this.id,
    required this.name,
    required this.isOwner,
    required this.isMember,
    required this.successorAccountId,
    required this.members,
    required this.memories,
    required this.myMemories,
    required this.myPhotos,
    required this.freeBytes,
    required this.limitBytes,
    required this.myCuratorAccountId,
  });

  final String id;
  final String name;
  final bool isOwner;

  /// False for someone reading through a link: no people, no addresses, no actions (D-271).
  final bool isMember;
  final String? successorAccountId;
  final List<AlbumMember> members;
  final List<AlbumMemory> memories;
  final int myMemories;
  final int myPhotos;
  final int freeBytes;
  final int limitBytes;
  final String? myCuratorAccountId;

  bool get poolIsFull => freeBytes <= 0;
}

class AlbumInvited {
  const AlbumInvited({required this.id, required this.email});

  final String id;
  final String email;
}

/// R5.
class AlbumSeats {
  const AlbumSeats({required this.taken, required this.invited, required this.free, required this.invitations});

  final int taken;
  final int invited;
  final int free;
  final List<AlbumInvited> invitations;
}

/// S14: who invites and to what, no content.
class AlbumInvitation {
  const AlbumInvitation({required this.id, required this.albumId, required this.albumName, required this.invitedBy});

  final String id;
  final String albumId;
  final String albumName;
  final String? invitedBy;
}

/// What stayed in an album the person is no longer in.
class KeptContribution {
  const KeptContribution({
    required this.albumId,
    required this.albumName,
    required this.chosenByAuthor,
    required this.memories,
  });

  final String albumId;
  final String albumName;
  final bool chosenByAuthor;
  final int memories;
}

/// S13.
class EntrustedMemory {
  const EntrustedMemory({
    required this.albumId,
    required this.albumName,
    required this.memoryId,
    required this.placeName,
    required this.dateFrom,
  });

  final String albumId;
  final String albumName;
  final String memoryId;
  final String placeName;
  final String dateFrom;
}

/// A memory of the person that they can bring to an album.
class OwnMemory {
  const OwnMemory({required this.id, required this.placeName, required this.dateFrom});

  final String id;
  final String placeName;
  final String dateFrom;
}

/// Someone reading the album through its link (R10).
class AlbumViewer {
  const AlbumViewer({required this.accountId, required this.email});

  final String accountId;
  final String? email;

  String get label => email ?? accountId;
}

/// What a link leads to (S15).
class AlbumLinkPreview {
  const AlbumLinkPreview({required this.albumId, required this.albumName});

  final String albumId;
  final String albumName;
}
