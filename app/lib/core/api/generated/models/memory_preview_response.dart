// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'memory_preview_response.g.dart';

@JsonSerializable()
class MemoryPreviewResponse {
  const MemoryPreviewResponse({
    required this.id,
    required this.placeName,
    required this.datePrecision,
    required this.dateFrom,
    required this.dateTo,
    required this.time,
    required this.excerpt,
    required this.photos,
    required this.photoUrl,
  });

  factory MemoryPreviewResponse.fromJson(Map<String, Object?> json) => _$MemoryPreviewResponseFromJson(json);

  final String id;
  final String placeName;
  final String datePrecision;
  final String dateFrom;
  final String dateTo;
  final String? time;
  final String? excerpt;
  final dynamic photos;
  final String? photoUrl;

  Map<String, Object?> toJson() => _$MemoryPreviewResponseToJson(this);
}
