// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_album_from_period_request.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CreateAlbumFromPeriodRequest _$CreateAlbumFromPeriodRequestFromJson(Map<String, dynamic> json) =>
    CreateAlbumFromPeriodRequest(
      id: json['id'] as String,
      name: json['name'] as String?,
      from: json['from'] as String?,
      to: json['to'] as String?,
    );

Map<String, dynamic> _$CreateAlbumFromPeriodRequestToJson(CreateAlbumFromPeriodRequest instance) => <String, dynamic>{
  'id': instance.id,
  'name': instance.name,
  'from': instance.from,
  'to': instance.to,
};
