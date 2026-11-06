// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'takeover_offer_response.g.dart';

@JsonSerializable()
class TakeoverOfferResponse {
  const TakeoverOfferResponse({
    required this.id,
    required this.albumId,
    required this.albumName,
    required this.answerBy,
    required this.founderMemories,
  });

  factory TakeoverOfferResponse.fromJson(Map<String, Object?> json) => _$TakeoverOfferResponseFromJson(json);

  final String id;
  final String albumId;
  final String albumName;
  final DateTime answerBy;
  final dynamic founderMemories;

  Map<String, Object?> toJson() => _$TakeoverOfferResponseToJson(this);
}
