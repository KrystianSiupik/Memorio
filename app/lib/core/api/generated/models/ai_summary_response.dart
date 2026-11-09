// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

import 'ai_summary_sentence.dart';

part 'ai_summary_response.g.dart';

@JsonSerializable()
class AiSummaryResponse {
  const AiSummaryResponse({
    required this.id,
    required this.accountId,
    required this.albumId,
    required this.from,
    required this.to,
    required this.state,
    required this.text,
    required this.sentences,
    required this.isAiGenerated,
    required this.model,
    required this.promptVersion,
    required this.generatedAt,
    required this.usedMemories,
    required this.leftOutMemories,
    required this.leftOutPeople,
    required this.droppedSentences,
    required this.addedToAlbum,
    required this.invalidReason,
  });

  factory AiSummaryResponse.fromJson(Map<String, Object?> json) => _$AiSummaryResponseFromJson(json);

  final String id;
  final String accountId;
  final String albumId;
  final String from;
  final String to;
  final String state;
  final String text;
  final List<AiSummarySentence> sentences;
  final bool isAiGenerated;
  final String model;
  final String promptVersion;
  final DateTime generatedAt;
  final dynamic usedMemories;
  final dynamic leftOutMemories;
  final dynamic leftOutPeople;
  final dynamic droppedSentences;
  final bool addedToAlbum;
  final String? invalidReason;

  Map<String, Object?> toJson() => _$AiSummaryResponseToJson(this);
}
