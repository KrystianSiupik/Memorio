// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'place_response.g.dart';

@JsonSerializable()
class PlaceResponse {
  const PlaceResponse({
    required this.placeReference,
    required this.name,
    required this.latitude,
    required this.longitude,
    required this.isOwn,
  });

  factory PlaceResponse.fromJson(Map<String, Object?> json) => _$PlaceResponseFromJson(json);

  final String placeReference;
  final String name;
  final dynamic latitude;
  final dynamic longitude;
  final bool isOwn;

  Map<String, Object?> toJson() => _$PlaceResponseToJson(this);
}
