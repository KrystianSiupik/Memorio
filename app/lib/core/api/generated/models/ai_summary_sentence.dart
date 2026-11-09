// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'ai_summary_sentence.g.dart';

@JsonSerializable()
class AiSummarySentence {
  const AiSummarySentence({required this.text, required this.memoryIds});

  factory AiSummarySentence.fromJson(Map<String, Object?> json) => _$AiSummarySentenceFromJson(json);

  final String text;
  final List<String> memoryIds;

  Map<String, Object?> toJson() => _$AiSummarySentenceToJson(this);
}
