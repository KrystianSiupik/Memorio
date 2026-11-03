// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'ai_draft_response.g.dart';

@JsonSerializable()
class AiDraftResponse {
  const AiDraftResponse({required this.id, required this.text, required this.isAiGenerated});

  factory AiDraftResponse.fromJson(Map<String, Object?> json) => _$AiDraftResponseFromJson(json);

  final String id;
  final String text;
  final bool isAiGenerated;

  Map<String, Object?> toJson() => _$AiDraftResponseToJson(this);
}
