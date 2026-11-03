// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'place_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PlaceResponse _$PlaceResponseFromJson(Map<String, dynamic> json) => PlaceResponse(
  placeReference: json['placeReference'] as String,
  name: json['name'] as String,
  latitude: json['latitude'],
  longitude: json['longitude'],
  isOwn: json['isOwn'] as bool,
);

Map<String, dynamic> _$PlaceResponseToJson(PlaceResponse instance) => <String, dynamic>{
  'placeReference': instance.placeReference,
  'name': instance.name,
  'latitude': instance.latitude,
  'longitude': instance.longitude,
  'isOwn': instance.isOwn,
};
