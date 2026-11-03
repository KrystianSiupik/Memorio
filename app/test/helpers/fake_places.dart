import 'package:memorio/core/result.dart';
import 'package:memorio/features/memories/memories.dart';

const giewont = PlaceOption(reference: 'osm:N:1', name: 'Giewont', latitude: 49.25, longitude: 19.93, isOwn: false);

class FakePlacesRepository implements PlacesRepository {
  List<PlaceOption> found = const [giewont];
  final searched = <String>[];
  final addedOwn = <String>[];

  @override
  Future<Result<List<PlaceOption>, MemoryFailure>> suggestions(String text) async {
    searched.add(text);
    return Ok(found);
  }

  @override
  Future<Result<PlaceOption, MemoryFailure>> choose(String reference) async =>
      Ok(found.firstWhere((place) => place.reference == reference));

  @override
  Future<Result<PlaceOption, MemoryFailure>> addOwn({
    required String id,
    required String name,
    required double latitude,
    required double longitude,
  }) async {
    addedOwn.add(name);
    return Ok(PlaceOption(reference: 'own:$id', name: name, latitude: latitude, longitude: longitude, isOwn: true));
  }
}

class FakePhotoPicker implements PhotoPicker {
  @override
  Future<List<PickedPhoto>> pick() async => [
    PickedPhoto(name: 'a.jpg', contentType: 'image/jpeg', read: () async => [1, 2], path: '/tmp/a.jpg'),
  ];
}
