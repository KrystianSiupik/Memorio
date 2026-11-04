// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'album_invited_response.g.dart';

@JsonSerializable()
class AlbumInvitedResponse {
  const AlbumInvitedResponse({required this.id, required this.email});

  factory AlbumInvitedResponse.fromJson(Map<String, Object?> json) => _$AlbumInvitedResponseFromJson(json);

  final String id;
  final String email;

  Map<String, Object?> toJson() => _$AlbumInvitedResponseToJson(this);
}
