/// Why an operation on memories did not work; the screens say it in words. [code] is what the offline
/// queue keeps for an operation refused for good.
sealed class MemoryFailure implements Exception {
  const MemoryFailure(this.code);

  final String code;
}

final class MemoryOffline extends MemoryFailure {
  const MemoryOffline() : super('Memory.Offline');
}

/// Not visible to this person: gone, or never theirs.
final class MemoryNoAccess extends MemoryFailure {
  const MemoryNoAccess() : super('Archive.NoAccess');
}

/// The state changed under this operation, e.g. the window of the trash ended.
final class MemoryAlreadyDecided extends MemoryFailure {
  const MemoryAlreadyDecided() : super('Archive.AlreadyDecided');
}

final class MemoryTermsNotAccepted extends MemoryFailure {
  const MemoryTermsNotAccepted() : super('Archive.TermsNotAccepted');
}

final class MemoryDateInFuture extends MemoryFailure {
  const MemoryDateInFuture() : super('Archive.DateInFuture');
}

final class MemoryPlaceMissing extends MemoryFailure {
  const MemoryPlaceMissing() : super('Archive.PlaceNotFound');
}

final class MemorySpaceFull extends MemoryFailure {
  const MemorySpaceFull() : super('Archive.SpaceLimitExceeded');
}

final class AiLimitReached extends MemoryFailure {
  const AiLimitReached() : super('AiNarration.LimitReached');
}

final class AiProviderDown extends MemoryFailure {
  const AiProviderDown() : super('AiNarration.ProviderUnavailable');
}

/// The backend refused the input.
final class MemoryInvalidInput extends MemoryFailure {
  const MemoryInvalidInput(super.code);
}

final class MemoryServerError extends MemoryFailure {
  const MemoryServerError() : super('Memory.ServerError');
}
