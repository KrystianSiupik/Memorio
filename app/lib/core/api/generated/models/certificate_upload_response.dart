// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'certificate_upload_response.g.dart';

@JsonSerializable()
class CertificateUploadResponse {
  const CertificateUploadResponse({required this.key, required this.url, required this.contentType});

  factory CertificateUploadResponse.fromJson(Map<String, Object?> json) => _$CertificateUploadResponseFromJson(json);

  final String key;
  final String url;
  final String contentType;

  Map<String, Object?> toJson() => _$CertificateUploadResponseToJson(this);
}
