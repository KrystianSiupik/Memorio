// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

import 'album_member_response.dart';
import 'album_memory_response.dart';

part 'album_response.g.dart';

@JsonSerializable()
class AlbumResponse {
  const AlbumResponse({
    required this.id,
    required this.name,
    required this.isOwner,
    required this.isMember,
    required this.successorAccountId,
    required this.members,
    required this.memories,
    required this.myMemories,
    required this.myPhotos,
    required this.freeBytes,
    required this.limitBytes,
    required this.myCuratorAccountId,
    required this.isReadOnly,
  });

  factory AlbumResponse.fromJson(Map<String, Object?> json) => _$AlbumResponseFromJson(json);

  final String id;
  final String name;
  final bool isOwner;
  final bool isMember;
  final String? successorAccountId;
  final List<AlbumMemberResponse> members;
  final List<AlbumMemoryResponse> memories;
  final dynamic myMemories;
  final dynamic myPhotos;
  final dynamic freeBytes;
  final dynamic limitBytes;
  final String? myCuratorAccountId;
  final bool isReadOnly;

  Map<String, Object?> toJson() => _$AlbumResponseToJson(this);
}
