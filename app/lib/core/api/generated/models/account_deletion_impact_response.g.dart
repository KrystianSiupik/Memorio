// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'account_deletion_impact_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AccountDeletionImpactResponse _$AccountDeletionImpactResponseFromJson(Map<String, dynamic> json) =>
    AccountDeletionImpactResponse(
      memories: json['memories'],
      photos: json['photos'],
      ownAlbums: json['ownAlbums'],
      ownAlbumsWithoutSuccessor: json['ownAlbumsWithoutSuccessor'],
      otherPeoplesAlbums: json['otherPeoplesAlbums'],
    );

Map<String, dynamic> _$AccountDeletionImpactResponseToJson(AccountDeletionImpactResponse instance) => <String, dynamic>{
  'memories': instance.memories,
  'photos': instance.photos,
  'ownAlbums': instance.ownAlbums,
  'ownAlbumsWithoutSuccessor': instance.ownAlbumsWithoutSuccessor,
  'otherPeoplesAlbums': instance.otherPeoplesAlbums,
};
