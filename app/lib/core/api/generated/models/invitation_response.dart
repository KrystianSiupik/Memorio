// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'invitation_response.g.dart';

@JsonSerializable()
class InvitationResponse {
  const InvitationResponse({required this.id, required this.albumId, required this.albumName, required this.invitedBy});

  factory InvitationResponse.fromJson(Map<String, Object?> json) => _$InvitationResponseFromJson(json);

  final String id;
  final String albumId;
  final String albumName;
  final String? invitedBy;

  Map<String, Object?> toJson() => _$InvitationResponseToJson(this);
}
