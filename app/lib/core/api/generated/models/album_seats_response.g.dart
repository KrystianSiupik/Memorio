// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'album_seats_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AlbumSeatsResponse _$AlbumSeatsResponseFromJson(Map<String, dynamic> json) => AlbumSeatsResponse(
  taken: json['taken'],
  invited: json['invited'],
  free: json['free'],
  invitations: (json['invitations'] as List<dynamic>)
      .map((e) => AlbumInvitedResponse.fromJson(e as Map<String, dynamic>))
      .toList(),
);

Map<String, dynamic> _$AlbumSeatsResponseToJson(AlbumSeatsResponse instance) => <String, dynamic>{
  'taken': instance.taken,
  'invited': instance.invited,
  'free': instance.free,
  'invitations': instance.invitations,
};
