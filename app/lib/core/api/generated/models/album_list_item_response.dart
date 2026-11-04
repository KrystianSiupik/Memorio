// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'album_list_item_response.g.dart';

@JsonSerializable()
class AlbumListItemResponse {
  const AlbumListItemResponse({
    required this.id,
    required this.name,
    required this.isOwner,
    required this.members,
    required this.memories,
  });

  factory AlbumListItemResponse.fromJson(Map<String, Object?> json) => _$AlbumListItemResponseFromJson(json);

  final String id;
  final String name;
  final bool isOwner;
  final dynamic members;
  final dynamic memories;

  Map<String, Object?> toJson() => _$AlbumListItemResponseToJson(this);
}
