// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'offer_handover_request.g.dart';

@JsonSerializable()
class OfferHandoverRequest {
  const OfferHandoverRequest({required this.toAccountId, required this.keepContributions});

  factory OfferHandoverRequest.fromJson(Map<String, Object?> json) => _$OfferHandoverRequestFromJson(json);

  final String toAccountId;
  final bool keepContributions;

  Map<String, Object?> toJson() => _$OfferHandoverRequestToJson(this);
}
