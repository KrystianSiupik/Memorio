// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'album_member_response.g.dart';

@JsonSerializable()
class AlbumMemberResponse {
  const AlbumMemberResponse({required this.accountId, required this.email, required this.isOwner});

  factory AlbumMemberResponse.fromJson(Map<String, Object?> json) => _$AlbumMemberResponseFromJson(json);

  final String accountId;
  final String? email;
  final bool isOwner;

  Map<String, Object?> toJson() => _$AlbumMemberResponseToJson(this);
}
