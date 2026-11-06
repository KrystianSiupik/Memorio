// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'report_request.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ReportRequest _$ReportRequestFromJson(Map<String, dynamic> json) => ReportRequest(
  id: json['id'] as String,
  target: json['target'] as String?,
  targetId: json['targetId'] as String,
  reason: json['reason'] as String?,
  explanation: json['explanation'] as String?,
);

Map<String, dynamic> _$ReportRequestToJson(ReportRequest instance) => <String, dynamic>{
  'id': instance.id,
  'target': instance.target,
  'targetId': instance.targetId,
  'reason': instance.reason,
  'explanation': instance.explanation,
};
