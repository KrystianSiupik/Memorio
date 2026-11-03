// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'memory_point_response.g.dart';

@JsonSerializable()
class MemoryPointResponse {
  const MemoryPointResponse({required this.id, required this.latitude, required this.longitude});

  factory MemoryPointResponse.fromJson(Map<String, Object?> json) => _$MemoryPointResponseFromJson(json);

  final String id;
  final dynamic latitude;
  final dynamic longitude;

  Map<String, Object?> toJson() => _$MemoryPointResponseToJson(this);
}
