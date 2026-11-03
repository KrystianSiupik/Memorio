import 'package:dio/dio.dart';

import '../../../core/api/generated/clients/places_client.dart';
import '../../../core/api/generated/models/add_own_place_request.dart';
import '../../../core/api/generated/models/choose_place_request.dart';
import '../../../core/api/generated/models/place_response.dart';
import '../../../core/result.dart';
import '../domain/memories_repository.dart';
import '../domain/memory.dart';
import '../domain/memory_failure.dart';
import 'http_memories_repository.dart';

/// [PlacesRepository] on API v1.
class HttpPlacesRepository implements PlacesRepository {
  const HttpPlacesRepository({required this.places});

  final PlacesClient places;

  @override
  Future<Result<List<PlaceOption>, MemoryFailure>> suggestions(String text) => _attempt(() async {
    final found = await places.getPlaceSuggestions(text: text);
    return [for (final place in found) _optionOf(place)];
  });

  @override
  Future<Result<PlaceOption, MemoryFailure>> choose(String reference) =>
      _attempt(() async => _optionOf(await places.choosePlace(body: ChoosePlaceRequest(placeReference: reference))));

  @override
  Future<Result<PlaceOption, MemoryFailure>> addOwn({
    required String id,
    required String name,
    required double latitude,
    required double longitude,
  }) => _attempt(
    () async => _optionOf(
      await places.addOwnPlace(
        body: AddOwnPlaceRequest(id: id, name: name, latitude: latitude, longitude: longitude),
      ),
    ),
  );

  static PlaceOption _optionOf(PlaceResponse place) => PlaceOption(
    reference: place.placeReference,
    name: place.name,
    latitude: (place.latitude as num).toDouble(),
    longitude: (place.longitude as num).toDouble(),
    isOwn: place.isOwn,
  );

  static Future<Result<T, MemoryFailure>> _attempt<T>(Future<T> Function() request) async {
    try {
      return Ok(await request());
    } on DioException catch (error) {
      return Err(memoryFailureOf(error));
    }
  }
}
