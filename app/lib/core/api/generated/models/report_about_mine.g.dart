// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'report_about_mine.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ReportAboutMine _$ReportAboutMineFromJson(Map<String, dynamic> json) => ReportAboutMine(
  id: json['id'] as String,
  target: json['target'] as String,
  targetId: json['targetId'] as String,
  reason: json['reason'] as String,
  category: json['category'] as String,
  state: json['state'] as String,
  hidden: json['hidden'] as bool,
  verdict: json['verdict'] as String?,
  statement: json['statement'] as String?,
  reportedAt: DateTime.parse(json['reportedAt'] as String),
  decidedAt: json['decidedAt'] == null ? null : DateTime.parse(json['decidedAt'] as String),
  appealUntil: json['appealUntil'] == null ? null : DateTime.parse(json['appealUntil'] as String),
  appealReversed: json['appealReversed'] as bool?,
  appealStatement: json['appealStatement'] as String?,
);

Map<String, dynamic> _$ReportAboutMineToJson(ReportAboutMine instance) => <String, dynamic>{
  'id': instance.id,
  'target': instance.target,
  'targetId': instance.targetId,
  'reason': instance.reason,
  'category': instance.category,
  'state': instance.state,
  'hidden': instance.hidden,
  'verdict': instance.verdict,
  'statement': instance.statement,
  'reportedAt': instance.reportedAt.toIso8601String(),
  'decidedAt': instance.decidedAt?.toIso8601String(),
  'appealUntil': instance.appealUntil?.toIso8601String(),
  'appealReversed': instance.appealReversed,
  'appealStatement': instance.appealStatement,
};
