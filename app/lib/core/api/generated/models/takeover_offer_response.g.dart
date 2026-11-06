// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'takeover_offer_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

TakeoverOfferResponse _$TakeoverOfferResponseFromJson(Map<String, dynamic> json) => TakeoverOfferResponse(
  id: json['id'] as String,
  albumId: json['albumId'] as String,
  albumName: json['albumName'] as String,
  answerBy: DateTime.parse(json['answerBy'] as String),
  founderMemories: json['founderMemories'],
);

Map<String, dynamic> _$TakeoverOfferResponseToJson(TakeoverOfferResponse instance) => <String, dynamic>{
  'id': instance.id,
  'albumId': instance.albumId,
  'albumName': instance.albumName,
  'answerBy': instance.answerBy.toIso8601String(),
  'founderMemories': instance.founderMemories,
};
