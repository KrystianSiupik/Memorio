// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_memory_request.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CreateMemoryRequest _$CreateMemoryRequestFromJson(Map<String, dynamic> json) => CreateMemoryRequest(
  id: json['id'] as String,
  placeReference: json['placeReference'] as String?,
  datePrecision: json['datePrecision'] as String?,
  dateFrom: json['dateFrom'] as String?,
  dateTo: json['dateTo'] as String?,
  time: json['time'] as String?,
);

Map<String, dynamic> _$CreateMemoryRequestToJson(CreateMemoryRequest instance) => <String, dynamic>{
  'id': instance.id,
  'placeReference': instance.placeReference,
  'datePrecision': instance.datePrecision,
  'dateFrom': instance.dateFrom,
  'dateTo': instance.dateTo,
  'time': instance.time,
};
