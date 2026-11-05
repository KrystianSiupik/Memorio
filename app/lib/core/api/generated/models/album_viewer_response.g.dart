// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'album_viewer_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AlbumViewerResponse _$AlbumViewerResponseFromJson(Map<String, dynamic> json) => AlbumViewerResponse(
  accountId: json['accountId'] as String,
  email: json['email'] as String?,
  enteredAt: DateTime.parse(json['enteredAt'] as String),
);

Map<String, dynamic> _$AlbumViewerResponseToJson(AlbumViewerResponse instance) => <String, dynamic>{
  'accountId': instance.accountId,
  'email': instance.email,
  'enteredAt': instance.enteredAt.toIso8601String(),
};
