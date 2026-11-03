// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'trashed_memory_response.g.dart';

@JsonSerializable()
class TrashedMemoryResponse {
  const TrashedMemoryResponse({
    required this.id,
    required this.placeName,
    required this.dateFrom,
    required this.description,
    required this.photos,
    required this.trashedAt,
    required this.daysLeft,
  });

  factory TrashedMemoryResponse.fromJson(Map<String, Object?> json) => _$TrashedMemoryResponseFromJson(json);

  final String id;
  final String placeName;
  final String dateFrom;
  final String? description;
  final dynamic photos;
  final DateTime trashedAt;
  final dynamic daysLeft;

  Map<String, Object?> toJson() => _$TrashedMemoryResponseToJson(this);
}
