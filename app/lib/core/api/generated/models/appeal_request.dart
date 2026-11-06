// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'appeal_request.g.dart';

@JsonSerializable()
class AppealRequest {
  const AppealRequest({required this.text});

  factory AppealRequest.fromJson(Map<String, Object?> json) => _$AppealRequestFromJson(json);

  final String? text;

  Map<String, Object?> toJson() => _$AppealRequestToJson(this);
}
