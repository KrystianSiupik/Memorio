// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'access_token_response.g.dart';

@JsonSerializable()
class AccessTokenResponse {
  const AccessTokenResponse({
    required this.accessToken,
    required this.expiresIn,
    required this.refreshToken,
    this.tokenType,
  });

  factory AccessTokenResponse.fromJson(Map<String, Object?> json) => _$AccessTokenResponseFromJson(json);

  final String? tokenType;
  final String accessToken;
  final dynamic expiresIn;
  final String refreshToken;

  Map<String, Object?> toJson() => _$AccessTokenResponseToJson(this);
}
