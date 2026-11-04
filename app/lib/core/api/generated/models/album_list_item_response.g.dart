// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'album_list_item_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AlbumListItemResponse _$AlbumListItemResponseFromJson(Map<String, dynamic> json) => AlbumListItemResponse(
  id: json['id'] as String,
  name: json['name'] as String,
  isOwner: json['isOwner'] as bool,
  members: json['members'],
  memories: json['memories'],
);

Map<String, dynamic> _$AlbumListItemResponseToJson(AlbumListItemResponse instance) => <String, dynamic>{
  'id': instance.id,
  'name': instance.name,
  'isOwner': instance.isOwner,
  'members': instance.members,
  'memories': instance.memories,
};
