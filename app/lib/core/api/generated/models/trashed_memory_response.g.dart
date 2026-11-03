// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'trashed_memory_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

TrashedMemoryResponse _$TrashedMemoryResponseFromJson(Map<String, dynamic> json) => TrashedMemoryResponse(
  id: json['id'] as String,
  placeName: json['placeName'] as String,
  dateFrom: json['dateFrom'] as String,
  description: json['description'] as String?,
  photos: json['photos'],
  trashedAt: DateTime.parse(json['trashedAt'] as String),
  daysLeft: json['daysLeft'],
);

Map<String, dynamic> _$TrashedMemoryResponseToJson(TrashedMemoryResponse instance) => <String, dynamic>{
  'id': instance.id,
  'placeName': instance.placeName,
  'dateFrom': instance.dateFrom,
  'description': instance.description,
  'photos': instance.photos,
  'trashedAt': instance.trashedAt.toIso8601String(),
  'daysLeft': instance.daysLeft,
};
