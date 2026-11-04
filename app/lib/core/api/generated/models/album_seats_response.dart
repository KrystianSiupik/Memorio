// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

import 'album_invited_response.dart';

part 'album_seats_response.g.dart';

@JsonSerializable()
class AlbumSeatsResponse {
  const AlbumSeatsResponse({required this.taken, required this.invited, required this.free, required this.invitations});

  factory AlbumSeatsResponse.fromJson(Map<String, Object?> json) => _$AlbumSeatsResponseFromJson(json);

  final dynamic taken;
  final dynamic invited;
  final dynamic free;
  final List<AlbumInvitedResponse> invitations;

  Map<String, Object?> toJson() => _$AlbumSeatsResponseToJson(this);
}
