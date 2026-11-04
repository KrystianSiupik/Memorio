// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'shared_album_response.g.dart';

@JsonSerializable()
class SharedAlbumResponse {
  const SharedAlbumResponse({required this.id, required this.name});

  factory SharedAlbumResponse.fromJson(Map<String, Object?> json) => _$SharedAlbumResponseFromJson(json);

  final String id;
  final String name;

  Map<String, Object?> toJson() => _$SharedAlbumResponseToJson(this);
}
