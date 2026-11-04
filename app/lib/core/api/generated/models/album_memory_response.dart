// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'album_memory_response.g.dart';

@JsonSerializable()
class AlbumMemoryResponse {
  const AlbumMemoryResponse({
    required this.id,
    required this.authorAccountId,
    required this.authorEmail,
    required this.isMine,
    required this.placeName,
    required this.dateFrom,
    required this.photos,
    required this.taggedAccountIds,
  });

  factory AlbumMemoryResponse.fromJson(Map<String, Object?> json) => _$AlbumMemoryResponseFromJson(json);

  final String id;
  final String authorAccountId;
  final String? authorEmail;
  final bool isMine;
  final String placeName;
  final String dateFrom;
  final dynamic photos;
  final List<String> taggedAccountIds;

  Map<String, Object?> toJson() => _$AlbumMemoryResponseToJson(this);
}
