/// Why an operation on albums did not work; the screens say it in words.
sealed class AlbumFailure implements Exception {
  const AlbumFailure();
}

final class AlbumOffline extends AlbumFailure {
  const AlbumOffline();
}

/// Not in this album, or no such album: the same answer for both.
final class AlbumNoAccess extends AlbumFailure {
  const AlbumNoAccess();
}

/// The person may not come into this album in any way.
final class AlbumBlocked extends AlbumFailure {
  const AlbumBlocked();
}

/// The link was revoked, or never existed.
final class AlbumLinkGone extends AlbumFailure {
  const AlbumLinkGone();
}

final class AlbumOnlyOwnerBlocks extends AlbumFailure {
  const AlbumOnlyOwnerBlocks();
}

final class AlbumAlreadyDecided extends AlbumFailure {
  const AlbumAlreadyDecided();
}

final class AlbumNoFreeSeats extends AlbumFailure {
  const AlbumNoFreeSeats();
}

/// The daily limit of invitations of an account.
final class AlbumInvitationLimit extends AlbumFailure {
  const AlbumInvitationLimit();
}

final class AlbumAddressBlocked extends AlbumFailure {
  const AlbumAddressBlocked();
}

final class AlbumInvitationGone extends AlbumFailure {
  const AlbumInvitationGone();
}

final class AlbumPoolFull extends AlbumFailure {
  const AlbumPoolFull();
}

final class AlbumOwnerCannotLeave extends AlbumFailure {
  const AlbumOwnerCannotLeave();
}

final class AlbumInvalidInput extends AlbumFailure {
  const AlbumInvalidInput();
}

final class AlbumServerError extends AlbumFailure {
  const AlbumServerError();
}
