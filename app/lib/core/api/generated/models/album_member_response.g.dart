// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'album_member_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AlbumMemberResponse _$AlbumMemberResponseFromJson(Map<String, dynamic> json) => AlbumMemberResponse(
  accountId: json['accountId'] as String,
  email: json['email'] as String?,
  isOwner: json['isOwner'] as bool,
);

Map<String, dynamic> _$AlbumMemberResponseToJson(AlbumMemberResponse instance) => <String, dynamic>{
  'accountId': instance.accountId,
  'email': instance.email,
  'isOwner': instance.isOwner,
};
