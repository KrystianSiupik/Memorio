// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'ai_consent_response.g.dart';

@JsonSerializable()
class AiConsentResponse {
  const AiConsentResponse({required this.given});

  factory AiConsentResponse.fromJson(Map<String, Object?> json) => _$AiConsentResponseFromJson(json);

  final bool given;

  Map<String, Object?> toJson() => _$AiConsentResponseToJson(this);
}
