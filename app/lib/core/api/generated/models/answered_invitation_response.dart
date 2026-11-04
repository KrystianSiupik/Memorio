// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'answered_invitation_response.g.dart';

@JsonSerializable()
class AnsweredInvitationResponse {
  const AnsweredInvitationResponse({required this.albumId});

  factory AnsweredInvitationResponse.fromJson(Map<String, Object?> json) => _$AnsweredInvitationResponseFromJson(json);

  final String albumId;

  Map<String, Object?> toJson() => _$AnsweredInvitationResponseToJson(this);
}
