// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'album_takeover_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AlbumTakeoverResponse _$AlbumTakeoverResponseFromJson(Map<String, dynamic> json) => AlbumTakeoverResponse(
  isOwnerless: json['isOwnerless'] as bool,
  canReserve: json['canReserve'] as bool,
  reservedByMe: json['reservedByMe'] as bool,
  reservedUntil: json['reservedUntil'] == null ? null : DateTime.parse(json['reservedUntil'] as String),
);

Map<String, dynamic> _$AlbumTakeoverResponseToJson(AlbumTakeoverResponse instance) => <String, dynamic>{
  'isOwnerless': instance.isOwnerless,
  'canReserve': instance.canReserve,
  'reservedByMe': instance.reservedByMe,
  'reservedUntil': instance.reservedUntil?.toIso8601String(),
};
