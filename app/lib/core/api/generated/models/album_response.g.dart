// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'album_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AlbumResponse _$AlbumResponseFromJson(Map<String, dynamic> json) => AlbumResponse(
  id: json['id'] as String,
  name: json['name'] as String,
  isOwner: json['isOwner'] as bool,
  isMember: json['isMember'] as bool,
  successorAccountId: json['successorAccountId'] as String?,
  members: (json['members'] as List<dynamic>)
      .map((e) => AlbumMemberResponse.fromJson(e as Map<String, dynamic>))
      .toList(),
  memories: (json['memories'] as List<dynamic>)
      .map((e) => AlbumMemoryResponse.fromJson(e as Map<String, dynamic>))
      .toList(),
  myMemories: json['myMemories'],
  myPhotos: json['myPhotos'],
  freeBytes: json['freeBytes'],
  limitBytes: json['limitBytes'],
  myCuratorAccountId: json['myCuratorAccountId'] as String?,
  isReadOnly: json['isReadOnly'] as bool,
);

Map<String, dynamic> _$AlbumResponseToJson(AlbumResponse instance) => <String, dynamic>{
  'id': instance.id,
  'name': instance.name,
  'isOwner': instance.isOwner,
  'isMember': instance.isMember,
  'successorAccountId': instance.successorAccountId,
  'members': instance.members,
  'memories': instance.memories,
  'myMemories': instance.myMemories,
  'myPhotos': instance.myPhotos,
  'freeBytes': instance.freeBytes,
  'limitBytes': instance.limitBytes,
  'myCuratorAccountId': instance.myCuratorAccountId,
  'isReadOnly': instance.isReadOnly,
};
