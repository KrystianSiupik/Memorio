// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'album_handover_response.g.dart';

@JsonSerializable()
class AlbumHandoverResponse {
  const AlbumHandoverResponse({
    required this.toAccountId,
    required this.forMe,
    required this.keepsContributions,
    required this.until,
  });

  factory AlbumHandoverResponse.fromJson(Map<String, Object?> json) => _$AlbumHandoverResponseFromJson(json);

  final String? toAccountId;
  final bool forMe;
  final bool keepsContributions;
  final DateTime? until;

  Map<String, Object?> toJson() => _$AlbumHandoverResponseToJson(this);
}
