// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'period_memory_response.g.dart';

@JsonSerializable()
class PeriodMemoryResponse {
  const PeriodMemoryResponse({
    required this.id,
    required this.placeName,
    required this.latitude,
    required this.longitude,
    required this.datePrecision,
    required this.dateFrom,
    required this.dateTo,
    required this.time,
    required this.description,
    required this.isMine,
    required this.photos,
    required this.photoUrls,
  });

  factory PeriodMemoryResponse.fromJson(Map<String, Object?> json) => _$PeriodMemoryResponseFromJson(json);

  final String id;
  final String placeName;
  final dynamic latitude;
  final dynamic longitude;
  final String datePrecision;
  final String dateFrom;
  final String dateTo;
  final String? time;
  final String? description;
  final bool isMine;
  final dynamic photos;
  final List<String> photoUrls;

  Map<String, Object?> toJson() => _$PeriodMemoryResponseToJson(this);
}
