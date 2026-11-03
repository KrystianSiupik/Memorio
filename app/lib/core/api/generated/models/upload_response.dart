// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'upload_response.g.dart';

@JsonSerializable()
class UploadResponse {
  const UploadResponse({required this.key, required this.url, required this.contentType});

  factory UploadResponse.fromJson(Map<String, Object?> json) => _$UploadResponseFromJson(json);

  final String key;
  final String url;
  final String contentType;

  Map<String, Object?> toJson() => _$UploadResponseToJson(this);
}
