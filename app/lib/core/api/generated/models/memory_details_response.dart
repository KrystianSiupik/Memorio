// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

import 'memory_photo_response.dart';

part 'memory_details_response.g.dart';

@JsonSerializable()
class MemoryDetailsResponse {
  const MemoryDetailsResponse({
    required this.id,
    required this.placeName,
    required this.latitude,
    required this.longitude,
    required this.datePrecision,
    required this.dateFrom,
    required this.dateTo,
    required this.isMine,
    required this.description,
    required this.descriptionIsAi,
    required this.photos,
  });

  factory MemoryDetailsResponse.fromJson(Map<String, Object?> json) => _$MemoryDetailsResponseFromJson(json);

  final String id;
  final String placeName;
  final dynamic latitude;
  final dynamic longitude;
  final String datePrecision;
  final String dateFrom;
  final String dateTo;
  final bool isMine;
  final String? description;
  final bool descriptionIsAi;
  final List<MemoryPhotoResponse> photos;

  Map<String, Object?> toJson() => _$MemoryDetailsResponseToJson(this);
}
