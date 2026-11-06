// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'start_certificate_upload_request.g.dart';

@JsonSerializable()
class StartCertificateUploadRequest {
  const StartCertificateUploadRequest({required this.contentType});

  factory StartCertificateUploadRequest.fromJson(Map<String, Object?> json) =>
      _$StartCertificateUploadRequestFromJson(json);

  final String? contentType;

  Map<String, Object?> toJson() => _$StartCertificateUploadRequestToJson(this);
}
