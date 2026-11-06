// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'period_memory_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PeriodMemoryResponse _$PeriodMemoryResponseFromJson(Map<String, dynamic> json) => PeriodMemoryResponse(
  id: json['id'] as String,
  placeName: json['placeName'] as String,
  latitude: json['latitude'],
  longitude: json['longitude'],
  datePrecision: json['datePrecision'] as String,
  dateFrom: json['dateFrom'] as String,
  dateTo: json['dateTo'] as String,
  description: json['description'] as String?,
  isMine: json['isMine'] as bool,
  photos: json['photos'],
  photoUrls: (json['photoUrls'] as List<dynamic>).map((e) => e as String).toList(),
);

Map<String, dynamic> _$PeriodMemoryResponseToJson(PeriodMemoryResponse instance) => <String, dynamic>{
  'id': instance.id,
  'placeName': instance.placeName,
  'latitude': instance.latitude,
  'longitude': instance.longitude,
  'datePrecision': instance.datePrecision,
  'dateFrom': instance.dateFrom,
  'dateTo': instance.dateTo,
  'description': instance.description,
  'isMine': instance.isMine,
  'photos': instance.photos,
  'photoUrls': instance.photoUrls,
};
