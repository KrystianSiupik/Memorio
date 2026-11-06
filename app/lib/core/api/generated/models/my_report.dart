// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'my_report.g.dart';

@JsonSerializable()
class MyReport {
  const MyReport({
    required this.id,
    required this.target,
    required this.targetId,
    required this.reason,
    required this.state,
    required this.verdict,
    required this.statement,
    required this.reportedAt,
    required this.decidedAt,
    required this.appealUntil,
    required this.appealReversed,
    required this.appealStatement,
  });

  factory MyReport.fromJson(Map<String, Object?> json) => _$MyReportFromJson(json);

  final String id;
  final String target;
  final String? targetId;
  final String reason;
  final String state;
  final String? verdict;
  final String? statement;
  final DateTime reportedAt;
  final DateTime? decidedAt;
  final DateTime? appealUntil;
  final bool? appealReversed;
  final String? appealStatement;

  Map<String, Object?> toJson() => _$MyReportToJson(this);
}
