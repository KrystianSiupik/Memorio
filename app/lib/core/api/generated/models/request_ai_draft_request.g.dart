// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'request_ai_draft_request.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

RequestAiDraftRequest _$RequestAiDraftRequestFromJson(Map<String, dynamic> json) => RequestAiDraftRequest(
  id: json['id'] as String,
  material: json['material'] as String?,
  consentsToAi: json['consentsToAi'] as bool,
  peopleNames: (json['peopleNames'] as List<dynamic>?)?.map((e) => e as String).toList(),
  placeNames: (json['placeNames'] as List<dynamic>?)?.map((e) => e as String).toList(),
);

Map<String, dynamic> _$RequestAiDraftRequestToJson(RequestAiDraftRequest instance) => <String, dynamic>{
  'id': instance.id,
  'material': instance.material,
  'consentsToAi': instance.consentsToAi,
  'peopleNames': instance.peopleNames,
  'placeNames': instance.placeNames,
};
