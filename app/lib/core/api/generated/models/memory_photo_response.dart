// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'memory_photo_response.g.dart';

@JsonSerializable()
class MemoryPhotoResponse {
  const MemoryPhotoResponse({required this.id, required this.url});

  factory MemoryPhotoResponse.fromJson(Map<String, Object?> json) => _$MemoryPhotoResponseFromJson(json);

  final String id;
  final String url;

  Map<String, Object?> toJson() => _$MemoryPhotoResponseToJson(this);
}
