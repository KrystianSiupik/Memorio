// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'offer_handover_request.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

OfferHandoverRequest _$OfferHandoverRequestFromJson(Map<String, dynamic> json) => OfferHandoverRequest(
  toAccountId: json['toAccountId'] as String,
  keepContributions: json['keepContributions'] as bool,
);

Map<String, dynamic> _$OfferHandoverRequestToJson(OfferHandoverRequest instance) => <String, dynamic>{
  'toAccountId': instance.toAccountId,
  'keepContributions': instance.keepContributions,
};
