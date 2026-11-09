// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'ai_summary_sentence.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AiSummarySentence _$AiSummarySentenceFromJson(Map<String, dynamic> json) => AiSummarySentence(
  text: json['text'] as String,
  memoryIds: (json['memoryIds'] as List<dynamic>).map((e) => e as String).toList(),
);

Map<String, dynamic> _$AiSummarySentenceToJson(AiSummarySentence instance) => <String, dynamic>{
  'text': instance.text,
  'memoryIds': instance.memoryIds,
};
