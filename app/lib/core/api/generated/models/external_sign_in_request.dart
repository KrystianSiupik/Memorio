// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'external_sign_in_request.g.dart';

@JsonSerializable()
class ExternalSignInRequest {
  const ExternalSignInRequest({required this.idToken, required this.declaresMinimumAge});

  factory ExternalSignInRequest.fromJson(Map<String, Object?> json) => _$ExternalSignInRequestFromJson(json);

  final String? idToken;
  final bool declaresMinimumAge;

  Map<String, Object?> toJson() => _$ExternalSignInRequestToJson(this);
}
