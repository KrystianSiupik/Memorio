import '../../../core/l10n/l10n.dart';
import '../domain/memory_failure.dart';

/// A failure in words, the same on every screen of this feature.
String memoryFailureText(AppLocalizations l10n, Object? failure) => switch (failure) {
  MemoryOffline() => l10n.authOffline,
  MemoryNoAccess() => l10n.memoryNoAccess,
  MemoryAlreadyDecided() => l10n.alreadyDecided,
  MemoryTermsNotAccepted() => l10n.termsRequired,
  MemoryDateInFuture() => l10n.dateInFuture,
  MemoryPlaceMissing() => l10n.placeMissing,
  MemorySpaceFull() => l10n.spaceFull,
  AiLimitReached() => l10n.aiLimitReached,
  AiProviderDown() => l10n.aiProviderDown,
  MemoryInvalidInput() => l10n.invalidInput,
  _ => l10n.serverError,
};

/// dd.MM.yyyy from a date; the time zone of the device.
String dayText(DateTime date) {
  final local = date.toLocal();
  String twoDigits(int value) => value.toString().padLeft(2, '0');
  return '${twoDigits(local.day)}.${twoDigits(local.month)}.${local.year}';
}

/// The date as exactly as the person gave it (S2:C1), with the hour when there is one.
String memoryDateText(String precision, String from, String to, String? time) {
  final parts = from.split('-');
  return switch (precision) {
    'Year' => parts[0],
    'Month' when parts.length == 3 => '${parts[1]}.${parts[0]}',
    'Period' => '${dayTextOf(from)} – ${dayTextOf(to)}',
    _ => time == null ? dayTextOf(from) : '${dayTextOf(from)}, $time',
  };
}

/// dd.MM.yyyy from the yyyy-MM-dd the backend sends.
String dayTextOf(String date) {
  final parts = date.split('-');
  return parts.length == 3 ? '${parts[2]}.${parts[1]}.${parts[0]}' : date;
}
