// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'memory_preview_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MemoryPreviewResponse _$MemoryPreviewResponseFromJson(Map<String, dynamic> json) => MemoryPreviewResponse(
  id: json['id'] as String,
  placeName: json['placeName'] as String,
  datePrecision: json['datePrecision'] as String,
  dateFrom: json['dateFrom'] as String,
  dateTo: json['dateTo'] as String,
  time: json['time'] as String?,
  excerpt: json['excerpt'] as String?,
  photos: json['photos'],
  photoUrl: json['photoUrl'] as String?,
);

Map<String, dynamic> _$MemoryPreviewResponseToJson(MemoryPreviewResponse instance) => <String, dynamic>{
  'id': instance.id,
  'placeName': instance.placeName,
  'datePrecision': instance.datePrecision,
  'dateFrom': instance.dateFrom,
  'dateTo': instance.dateTo,
  'time': instance.time,
  'excerpt': instance.excerpt,
  'photos': instance.photos,
  'photoUrl': instance.photoUrl,
};
