// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:dio/dio.dart';
import 'package:retrofit/retrofit.dart';

import '../models/add_own_place_request.dart';
import '../models/choose_place_request.dart';
import '../models/place_response.dart';

part 'places_client.g.dart';

@RestApi()
abstract class PlacesClient {
  factory PlacesClient(Dio dio, {String? baseUrl}) = _PlacesClient;

  /// Places whose name starts with the text: the person's own first, then OpenStreetMap.
  @GET('/v1/places/suggestions')
  Future<List<PlaceResponse>> getPlaceSuggestions({@Query('text') String? text, @Query('limit') dynamic limit});

  /// Checks the place a person chose from the suggestions and answers with it.
  @POST('/v1/places/choice')
  Future<PlaceResponse> choosePlace({@Body() required ChoosePlaceRequest body});

  /// Adds a place of the signed-in person's own; the id comes from the client, so a retry changes nothing.
  @POST('/v1/places/own')
  Future<PlaceResponse> addOwnPlace({@Body() required AddOwnPlaceRequest body});
}
