// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'account_deletion_impact_response.g.dart';

@JsonSerializable()
class AccountDeletionImpactResponse {
  const AccountDeletionImpactResponse({
    required this.memories,
    required this.photos,
    required this.ownAlbums,
    required this.ownAlbumsWithoutSuccessor,
    required this.otherPeoplesAlbums,
  });

  factory AccountDeletionImpactResponse.fromJson(Map<String, Object?> json) =>
      _$AccountDeletionImpactResponseFromJson(json);

  final dynamic memories;
  final dynamic photos;
  final dynamic ownAlbums;
  final dynamic ownAlbumsWithoutSuccessor;
  final dynamic otherPeoplesAlbums;

  Map<String, Object?> toJson() => _$AccountDeletionImpactResponseToJson(this);
}
