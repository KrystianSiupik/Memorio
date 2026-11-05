// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'plan_response.g.dart';

@JsonSerializable()
class PlanResponse {
  const PlanResponse({
    required this.mode,
    required this.entitledUntil,
    required this.readOnlySince,
    required this.trimAt,
    required this.choiceOpensAt,
    required this.isChoiceOpen,
    required this.isHeld,
    required this.lastCheckedAt,
    required this.keptMemoryIds,
    required this.usedBytes,
    required this.limitBytes,
    required this.freeLimitBytes,
    required this.aiUsed,
    required this.aiLimit,
  });

  factory PlanResponse.fromJson(Map<String, Object?> json) => _$PlanResponseFromJson(json);

  final String mode;
  final DateTime? entitledUntil;
  final DateTime? readOnlySince;
  final DateTime? trimAt;
  final DateTime? choiceOpensAt;
  final bool isChoiceOpen;
  final bool isHeld;
  final DateTime? lastCheckedAt;
  final List<String> keptMemoryIds;
  final dynamic usedBytes;
  final dynamic limitBytes;
  final dynamic freeLimitBytes;
  final dynamic aiUsed;
  final dynamic aiLimit;

  Map<String, Object?> toJson() => _$PlanResponseToJson(this);
}
