// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'legacy_archive_response.g.dart';

@JsonSerializable()
class LegacyArchiveResponse {
  const LegacyArchiveResponse({required this.ownerAccountId, required this.email});

  factory LegacyArchiveResponse.fromJson(Map<String, Object?> json) => _$LegacyArchiveResponseFromJson(json);

  final String ownerAccountId;
  final String? email;

  Map<String, Object?> toJson() => _$LegacyArchiveResponseToJson(this);
}
