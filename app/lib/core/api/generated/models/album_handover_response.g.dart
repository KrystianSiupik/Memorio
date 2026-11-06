// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'album_handover_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AlbumHandoverResponse _$AlbumHandoverResponseFromJson(Map<String, dynamic> json) => AlbumHandoverResponse(
  toAccountId: json['toAccountId'] as String?,
  forMe: json['forMe'] as bool,
  keepsContributions: json['keepsContributions'] as bool,
  until: json['until'] == null ? null : DateTime.parse(json['until'] as String),
);

Map<String, dynamic> _$AlbumHandoverResponseToJson(AlbumHandoverResponse instance) => <String, dynamic>{
  'toAccountId': instance.toAccountId,
  'forMe': instance.forMe,
  'keepsContributions': instance.keepsContributions,
  'until': instance.until?.toIso8601String(),
};
