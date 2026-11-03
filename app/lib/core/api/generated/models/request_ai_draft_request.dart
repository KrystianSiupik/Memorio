// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'request_ai_draft_request.g.dart';

@JsonSerializable()
class RequestAiDraftRequest {
  const RequestAiDraftRequest({
    required this.id,
    required this.material,
    required this.consentsToAi,
    required this.peopleNames,
    required this.placeNames,
  });

  factory RequestAiDraftRequest.fromJson(Map<String, Object?> json) => _$RequestAiDraftRequestFromJson(json);

  final String id;
  final String? material;
  final bool consentsToAi;
  final List<String>? peopleNames;
  final List<String>? placeNames;

  Map<String, Object?> toJson() => _$RequestAiDraftRequestToJson(this);
}
