// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'approve_description_request.g.dart';

@JsonSerializable()
class ApproveDescriptionRequest {
  const ApproveDescriptionRequest({required this.text, required this.aiDraftId});

  factory ApproveDescriptionRequest.fromJson(Map<String, Object?> json) => _$ApproveDescriptionRequestFromJson(json);

  final String? text;
  final String? aiDraftId;

  Map<String, Object?> toJson() => _$ApproveDescriptionRequestToJson(this);
}
