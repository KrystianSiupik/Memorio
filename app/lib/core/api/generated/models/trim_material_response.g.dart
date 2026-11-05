// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'trim_material_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

TrimMaterialResponse _$TrimMaterialResponseFromJson(Map<String, dynamic> json) => TrimMaterialResponse(
  memoryId: json['memoryId'] as String,
  placeName: json['placeName'] as String,
  dateFrom: json['dateFrom'] as String,
  sizeBytes: json['sizeBytes'],
);

Map<String, dynamic> _$TrimMaterialResponseToJson(TrimMaterialResponse instance) => <String, dynamic>{
  'memoryId': instance.memoryId,
  'placeName': instance.placeName,
  'dateFrom': instance.dateFrom,
  'sizeBytes': instance.sizeBytes,
};
