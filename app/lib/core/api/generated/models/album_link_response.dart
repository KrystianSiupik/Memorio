// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'album_link_response.g.dart';

@JsonSerializable()
class AlbumLinkResponse {
  const AlbumLinkResponse({required this.token});

  factory AlbumLinkResponse.fromJson(Map<String, Object?> json) => _$AlbumLinkResponseFromJson(json);

  final String? token;

  Map<String, Object?> toJson() => _$AlbumLinkResponseToJson(this);
}
