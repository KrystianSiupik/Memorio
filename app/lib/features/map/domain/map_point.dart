/// One memory on the map.
class MapPoint {
  const MapPoint({required this.id, required this.latitude, required this.longitude, this.photoUrl});

  final String id;
  final double latitude;
  final double longitude;

  /// MEM-E13-PLAT-05: drawn as a round photo when photo points are on.
  final String? photoUrl;
}
