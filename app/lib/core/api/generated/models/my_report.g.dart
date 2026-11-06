// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'my_report.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MyReport _$MyReportFromJson(Map<String, dynamic> json) => MyReport(
  id: json['id'] as String,
  target: json['target'] as String,
  targetId: json['targetId'] as String?,
  reason: json['reason'] as String,
  state: json['state'] as String,
  verdict: json['verdict'] as String?,
  statement: json['statement'] as String?,
  reportedAt: DateTime.parse(json['reportedAt'] as String),
  decidedAt: json['decidedAt'] == null ? null : DateTime.parse(json['decidedAt'] as String),
);

Map<String, dynamic> _$MyReportToJson(MyReport instance) => <String, dynamic>{
  'id': instance.id,
  'target': instance.target,
  'targetId': instance.targetId,
  'reason': instance.reason,
  'state': instance.state,
  'verdict': instance.verdict,
  'statement': instance.statement,
  'reportedAt': instance.reportedAt.toIso8601String(),
  'decidedAt': instance.decidedAt?.toIso8601String(),
};
