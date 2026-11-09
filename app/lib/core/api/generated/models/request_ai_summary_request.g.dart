// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'request_ai_summary_request.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

RequestAiSummaryRequest _$RequestAiSummaryRequestFromJson(Map<String, dynamic> json) => RequestAiSummaryRequest(
  id: json['id'] as String,
  albumId: json['albumId'] as String,
  from: json['from'] as String?,
  to: json['to'] as String?,
);

Map<String, dynamic> _$RequestAiSummaryRequestToJson(RequestAiSummaryRequest instance) => <String, dynamic>{
  'id': instance.id,
  'albumId': instance.albumId,
  'from': instance.from,
  'to': instance.to,
};
