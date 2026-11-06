// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'memory_details_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MemoryDetailsResponse _$MemoryDetailsResponseFromJson(Map<String, dynamic> json) => MemoryDetailsResponse(
  id: json['id'] as String,
  placeName: json['placeName'] as String,
  latitude: json['latitude'],
  longitude: json['longitude'],
  datePrecision: json['datePrecision'] as String,
  dateFrom: json['dateFrom'] as String,
  dateTo: json['dateTo'] as String,
  time: json['time'] as String?,
  isMine: json['isMine'] as bool,
  description: json['description'] as String?,
  descriptionIsAi: json['descriptionIsAi'] as bool,
  photos: (json['photos'] as List<dynamic>)
      .map((e) => MemoryPhotoResponse.fromJson(e as Map<String, dynamic>))
      .toList(),
);

Map<String, dynamic> _$MemoryDetailsResponseToJson(MemoryDetailsResponse instance) => <String, dynamic>{
  'id': instance.id,
  'placeName': instance.placeName,
  'latitude': instance.latitude,
  'longitude': instance.longitude,
  'datePrecision': instance.datePrecision,
  'dateFrom': instance.dateFrom,
  'dateTo': instance.dateTo,
  'time': instance.time,
  'isMine': instance.isMine,
  'description': instance.description,
  'descriptionIsAi': instance.descriptionIsAi,
  'photos': instance.photos,
};
