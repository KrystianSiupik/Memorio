// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'plan_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PlanResponse _$PlanResponseFromJson(Map<String, dynamic> json) => PlanResponse(
  mode: json['mode'] as String,
  entitledUntil: json['entitledUntil'] == null ? null : DateTime.parse(json['entitledUntil'] as String),
  readOnlySince: json['readOnlySince'] == null ? null : DateTime.parse(json['readOnlySince'] as String),
  trimAt: json['trimAt'] == null ? null : DateTime.parse(json['trimAt'] as String),
  choiceOpensAt: json['choiceOpensAt'] == null ? null : DateTime.parse(json['choiceOpensAt'] as String),
  isChoiceOpen: json['isChoiceOpen'] as bool,
  isHeld: json['isHeld'] as bool,
  lastCheckedAt: json['lastCheckedAt'] == null ? null : DateTime.parse(json['lastCheckedAt'] as String),
  keptMemoryIds: (json['keptMemoryIds'] as List<dynamic>).map((e) => e as String).toList(),
  usedBytes: json['usedBytes'],
  limitBytes: json['limitBytes'],
  freeLimitBytes: json['freeLimitBytes'],
  aiUsed: json['aiUsed'],
  aiLimit: json['aiLimit'],
);

Map<String, dynamic> _$PlanResponseToJson(PlanResponse instance) => <String, dynamic>{
  'mode': instance.mode,
  'entitledUntil': instance.entitledUntil?.toIso8601String(),
  'readOnlySince': instance.readOnlySince?.toIso8601String(),
  'trimAt': instance.trimAt?.toIso8601String(),
  'choiceOpensAt': instance.choiceOpensAt?.toIso8601String(),
  'isChoiceOpen': instance.isChoiceOpen,
  'isHeld': instance.isHeld,
  'lastCheckedAt': instance.lastCheckedAt?.toIso8601String(),
  'keptMemoryIds': instance.keptMemoryIds,
  'usedBytes': instance.usedBytes,
  'limitBytes': instance.limitBytes,
  'freeLimitBytes': instance.freeLimitBytes,
  'aiUsed': instance.aiUsed,
  'aiLimit': instance.aiLimit,
};
