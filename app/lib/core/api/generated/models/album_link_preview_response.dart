// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'album_link_preview_response.g.dart';

@JsonSerializable()
class AlbumLinkPreviewResponse {
  const AlbumLinkPreviewResponse({required this.albumId, required this.albumName});

  factory AlbumLinkPreviewResponse.fromJson(Map<String, Object?> json) => _$AlbumLinkPreviewResponseFromJson(json);

  final String albumId;
  final String albumName;

  Map<String, Object?> toJson() => _$AlbumLinkPreviewResponseToJson(this);
}
