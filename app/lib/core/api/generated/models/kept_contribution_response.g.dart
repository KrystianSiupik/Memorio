// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'kept_contribution_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

KeptContributionResponse _$KeptContributionResponseFromJson(Map<String, dynamic> json) => KeptContributionResponse(
  albumId: json['albumId'] as String,
  albumName: json['albumName'] as String,
  chosenByAuthor: json['chosenByAuthor'] as bool,
  memories: json['memories'],
);

Map<String, dynamic> _$KeptContributionResponseToJson(KeptContributionResponse instance) => <String, dynamic>{
  'albumId': instance.albumId,
  'albumName': instance.albumName,
  'chosenByAuthor': instance.chosenByAuthor,
  'memories': instance.memories,
};
