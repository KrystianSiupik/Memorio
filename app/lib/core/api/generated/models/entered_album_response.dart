// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'entered_album_response.g.dart';

@JsonSerializable()
class EnteredAlbumResponse {
  const EnteredAlbumResponse({required this.albumId});

  factory EnteredAlbumResponse.fromJson(Map<String, Object?> json) => _$EnteredAlbumResponseFromJson(json);

  final String albumId;

  Map<String, Object?> toJson() => _$EnteredAlbumResponseToJson(this);
}
