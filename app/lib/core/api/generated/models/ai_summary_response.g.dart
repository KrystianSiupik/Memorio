// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'ai_summary_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AiSummaryResponse _$AiSummaryResponseFromJson(Map<String, dynamic> json) => AiSummaryResponse(
  id: json['id'] as String,
  accountId: json['accountId'] as String,
  albumId: json['albumId'] as String,
  from: json['from'] as String,
  to: json['to'] as String,
  state: json['state'] as String,
  text: json['text'] as String,
  sentences: (json['sentences'] as List<dynamic>)
      .map((e) => AiSummarySentence.fromJson(e as Map<String, dynamic>))
      .toList(),
  isAiGenerated: json['isAiGenerated'] as bool,
  model: json['model'] as String,
  promptVersion: json['promptVersion'] as String,
  generatedAt: DateTime.parse(json['generatedAt'] as String),
  usedMemories: json['usedMemories'],
  leftOutMemories: json['leftOutMemories'],
  leftOutPeople: json['leftOutPeople'],
  droppedSentences: json['droppedSentences'],
  addedToAlbum: json['addedToAlbum'] as bool,
  invalidReason: json['invalidReason'] as String?,
);

Map<String, dynamic> _$AiSummaryResponseToJson(AiSummaryResponse instance) => <String, dynamic>{
  'id': instance.id,
  'accountId': instance.accountId,
  'albumId': instance.albumId,
  'from': instance.from,
  'to': instance.to,
  'state': instance.state,
  'text': instance.text,
  'sentences': instance.sentences,
  'isAiGenerated': instance.isAiGenerated,
  'model': instance.model,
  'promptVersion': instance.promptVersion,
  'generatedAt': instance.generatedAt.toIso8601String(),
  'usedMemories': instance.usedMemories,
  'leftOutMemories': instance.leftOutMemories,
  'leftOutPeople': instance.leftOutPeople,
  'droppedSentences': instance.droppedSentences,
  'addedToAlbum': instance.addedToAlbum,
  'invalidReason': instance.invalidReason,
};
