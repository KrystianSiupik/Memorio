// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'report_death_request.g.dart';

@JsonSerializable()
class ReportDeathRequest {
  const ReportDeathRequest({required this.id, required this.uploadKey});

  factory ReportDeathRequest.fromJson(Map<String, Object?> json) => _$ReportDeathRequestFromJson(json);

  final String id;
  final String? uploadKey;

  Map<String, Object?> toJson() => _$ReportDeathRequestToJson(this);
}
