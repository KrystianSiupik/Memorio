// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'request_ai_summary_request.g.dart';

@JsonSerializable()
class RequestAiSummaryRequest {
  const RequestAiSummaryRequest({required this.id, required this.albumId, required this.from, required this.to});

  factory RequestAiSummaryRequest.fromJson(Map<String, Object?> json) => _$RequestAiSummaryRequestFromJson(json);

  final String id;
  final String albumId;
  final String? from;
  final String? to;

  Map<String, Object?> toJson() => _$RequestAiSummaryRequestToJson(this);
}
