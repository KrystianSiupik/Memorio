// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'report_request.g.dart';

@JsonSerializable()
class ReportRequest {
  const ReportRequest({
    required this.id,
    required this.target,
    required this.targetId,
    required this.reason,
    required this.explanation,
  });

  factory ReportRequest.fromJson(Map<String, Object?> json) => _$ReportRequestFromJson(json);

  final String id;
  final String? target;
  final String targetId;
  final String? reason;
  final String? explanation;

  Map<String, Object?> toJson() => _$ReportRequestToJson(this);
}
