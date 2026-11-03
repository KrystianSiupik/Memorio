/// One memory on the map.
class MemoryPin {
  const MemoryPin({required this.id, required this.latitude, required this.longitude});

  final String id;
  final double latitude;
  final double longitude;
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
  });

  final String id;
  final String placeReference;
  final MemoryDatePrecision precision;

  /// yyyy-MM-dd.
  final String from;

  /// yyyy-MM-dd; only for a period.
  final String? to;
}

/// What S05 shows.
class MemoryDetails {
  const MemoryDetails({
    required this.id,
    required this.placeName,
    required this.dateFrom,
    required this.dateTo,
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
  final bool isMine;
  final String? description;
  final bool descriptionIsAi;

  /// Signed for 5 minutes.
  final List<String> photoUrls;
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
