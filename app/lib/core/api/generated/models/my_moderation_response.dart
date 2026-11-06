// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

import 'my_report.dart';
import 'report_about_mine.dart';

part 'my_moderation_response.g.dart';

@JsonSerializable()
class MyModerationResponse {
  const MyModerationResponse({required this.reports, required this.aboutMine});

  factory MyModerationResponse.fromJson(Map<String, Object?> json) => _$MyModerationResponseFromJson(json);

  final List<MyReport> reports;
  final List<ReportAboutMine> aboutMine;

  Map<String, Object?> toJson() => _$MyModerationResponseToJson(this);
}
