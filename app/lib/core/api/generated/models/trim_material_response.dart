// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'trim_material_response.g.dart';

@JsonSerializable()
class TrimMaterialResponse {
  const TrimMaterialResponse({
    required this.memoryId,
    required this.placeName,
    required this.dateFrom,
    required this.sizeBytes,
  });

  factory TrimMaterialResponse.fromJson(Map<String, Object?> json) => _$TrimMaterialResponseFromJson(json);

  final String memoryId;
  final String placeName;
  final String dateFrom;
  final dynamic sizeBytes;

  Map<String, Object?> toJson() => _$TrimMaterialResponseToJson(this);
}
