// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'add_own_place_request.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AddOwnPlaceRequest _$AddOwnPlaceRequestFromJson(Map<String, dynamic> json) => AddOwnPlaceRequest(
  id: json['id'] as String,
  name: json['name'] as String?,
  latitude: json['latitude'],
  longitude: json['longitude'],
);

Map<String, dynamic> _$AddOwnPlaceRequestToJson(AddOwnPlaceRequest instance) => <String, dynamic>{
  'id': instance.id,
  'name': instance.name,
  'latitude': instance.latitude,
  'longitude': instance.longitude,
};
