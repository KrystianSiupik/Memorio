// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'invitation_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

InvitationResponse _$InvitationResponseFromJson(Map<String, dynamic> json) => InvitationResponse(
  id: json['id'] as String,
  albumId: json['albumId'] as String,
  albumName: json['albumName'] as String,
  invitedBy: json['invitedBy'] as String?,
);

Map<String, dynamic> _$InvitationResponseToJson(InvitationResponse instance) => <String, dynamic>{
  'id': instance.id,
  'albumId': instance.albumId,
  'albumName': instance.albumName,
  'invitedBy': instance.invitedBy,
};
