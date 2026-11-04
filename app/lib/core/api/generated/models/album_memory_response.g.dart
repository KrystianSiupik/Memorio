// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'album_memory_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AlbumMemoryResponse _$AlbumMemoryResponseFromJson(Map<String, dynamic> json) => AlbumMemoryResponse(
  id: json['id'] as String,
  authorAccountId: json['authorAccountId'] as String,
  authorEmail: json['authorEmail'] as String?,
  isMine: json['isMine'] as bool,
  placeName: json['placeName'] as String,
  dateFrom: json['dateFrom'] as String,
  photos: json['photos'],
  taggedAccountIds: (json['taggedAccountIds'] as List<dynamic>).map((e) => e as String).toList(),
);

Map<String, dynamic> _$AlbumMemoryResponseToJson(AlbumMemoryResponse instance) => <String, dynamic>{
  'id': instance.id,
  'authorAccountId': instance.authorAccountId,
  'authorEmail': instance.authorEmail,
  'isMine': instance.isMine,
  'placeName': instance.placeName,
  'dateFrom': instance.dateFrom,
  'photos': instance.photos,
  'taggedAccountIds': instance.taggedAccountIds,
};
