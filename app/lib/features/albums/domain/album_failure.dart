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

final class AlbumAlreadyDecided extends AlbumFailure {
  const AlbumAlreadyDecided();
}

final class AlbumNoFreeSeats extends AlbumFailure {
  const AlbumNoFreeSeats();
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
