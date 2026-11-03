// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'ai_draft_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AiDraftResponse _$AiDraftResponseFromJson(Map<String, dynamic> json) => AiDraftResponse(
  id: json['id'] as String,
  text: json['text'] as String,
  isAiGenerated: json['isAiGenerated'] as bool,
);

Map<String, dynamic> _$AiDraftResponseToJson(AiDraftResponse instance) => <String, dynamic>{
  'id': instance.id,
  'text': instance.text,
  'isAiGenerated': instance.isAiGenerated,
};
