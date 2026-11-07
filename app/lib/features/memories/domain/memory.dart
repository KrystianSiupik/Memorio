/// One memory on the map.
class MemoryPin {
  const MemoryPin({required this.id, required this.latitude, required this.longitude, this.photoUrl});

  final String id;
  final double latitude;
  final double longitude;

  /// MEM-E13-PLAT-05: the first photo, signed; only when photo points are on.
  final String? photoUrl;
}

/// How exactly a person remembers the date; [apiName] is what the backend takes.
enum MemoryDatePrecision {
  day('Day'),
  month('Month'),
  year('Year'),
  period('Period');

  const MemoryDatePrecision(this.apiName);

  final String apiName;
}

/// What creating a memory takes. [id] comes from the client, so a retry is the same memory.
class NewMemory {
  const NewMemory({
    required this.id,
    required this.placeReference,
    required this.precision,
    required this.from,
    this.to,
    this.time,
  });

  final String id;
  final String placeReference;
  final MemoryDatePrecision precision;

  /// yyyy-MM-dd.
  final String from;

  /// yyyy-MM-dd; only for a period.
  final String? to;

  /// HH:mm, when the person knows the hour; only for a day. Orders memories within a day (D-303).
  final String? time;
}

/// What S05 shows.
class MemoryDetails {
  const MemoryDetails({
    required this.id,
    required this.placeName,
    required this.dateFrom,
    required this.dateTo,
    this.time,
    required this.isMine,
    required this.description,
    required this.descriptionIsAi,
    required this.photoUrls,
  });

  final String id;
  final String placeName;

  /// yyyy-MM-dd, the range the date stands for.
  final String dateFrom;
  final String dateTo;

  /// HH:mm, when the person gave the hour.
  final String? time;
  final bool isMine;
  final String? description;
  final bool descriptionIsAi;

  /// Signed for 5 minutes.
  final List<String> photoUrls;
}

/// MEM-E12-PLAT-01: what the map shows on hovering a point.
class MemoryPreview {
  const MemoryPreview({
    required this.id,
    required this.placeName,
    required this.datePrecision,
    required this.dateFrom,
    required this.dateTo,
    this.time,
    this.excerpt,
    this.photos = 0,
    this.photoUrl,
  });

  final String id;
  final String placeName;

  /// Day, Month, Year or Period.
  final String datePrecision;
  final String dateFrom;
  final String dateTo;
  final String? time;

  /// The start of the description, cut at a word.
  final String? excerpt;
  final int photos;

  /// The first photo added, signed for 5 minutes.
  final String? photoUrl;
}

/// One row of S06.
class TrashedMemory {
  const TrashedMemory({
    required this.id,
    required this.placeName,
    required this.dateFrom,
    required this.description,
    required this.photos,
    required this.trashedAt,
    required this.daysLeft,
  });

  final String id;
  final String placeName;
  final String dateFrom;
  final String? description;
  final int photos;
  final DateTime trashedAt;
  final int daysLeft;
}

/// A place to choose in S04.
class PlaceOption {
  const PlaceOption({
    required this.reference,
    required this.name,
    required this.latitude,
    required this.longitude,
    required this.isOwn,
  });

  final String reference;
  final String name;
  final double latitude;
  final double longitude;
  final bool isOwn;
}

/// A draft from the AI; [id] goes back with the approved description, for its marker.
class AiDraft {
  const AiDraft({required this.id, required this.text});

  final String id;
  final String text;
}

/// A photo the person picked, not sent yet. [path] exists on devices, so it can wait in the offline queue.
class PickedPhoto {
  const PickedPhoto({required this.name, required this.contentType, required this.read, this.path});

  final String name;
  final String contentType;
  final Future<List<int>> Function() read;
  final String? path;
}
