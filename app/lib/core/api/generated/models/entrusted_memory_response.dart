// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'entrusted_memory_response.g.dart';

@JsonSerializable()
class EntrustedMemoryResponse {
  const EntrustedMemoryResponse({
    required this.albumId,
    required this.albumName,
    required this.memoryId,
    required this.placeName,
    required this.dateFrom,
  });

  factory EntrustedMemoryResponse.fromJson(Map<String, Object?> json) => _$EntrustedMemoryResponseFromJson(json);

  final String albumId;
  final String albumName;
  final String memoryId;
  final String placeName;
  final String dateFrom;

  Map<String, Object?> toJson() => _$EntrustedMemoryResponseToJson(this);
}
