// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'entrusted_memory_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

EntrustedMemoryResponse _$EntrustedMemoryResponseFromJson(Map<String, dynamic> json) => EntrustedMemoryResponse(
  albumId: json['albumId'] as String,
  albumName: json['albumName'] as String,
  memoryId: json['memoryId'] as String,
  placeName: json['placeName'] as String,
  dateFrom: json['dateFrom'] as String,
);

Map<String, dynamic> _$EntrustedMemoryResponseToJson(EntrustedMemoryResponse instance) => <String, dynamic>{
  'albumId': instance.albumId,
  'albumName': instance.albumName,
  'memoryId': instance.memoryId,
  'placeName': instance.placeName,
  'dateFrom': instance.dateFrom,
};
