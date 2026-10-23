// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'access_token_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AccessTokenResponse _$AccessTokenResponseFromJson(Map<String, dynamic> json) => AccessTokenResponse(
  accessToken: json['accessToken'] as String,
  expiresIn: json['expiresIn'],
  refreshToken: json['refreshToken'] as String,
  tokenType: json['tokenType'] as String?,
);

Map<String, dynamic> _$AccessTokenResponseToJson(AccessTokenResponse instance) => <String, dynamic>{
  'tokenType': instance.tokenType,
  'accessToken': instance.accessToken,
  'expiresIn': instance.expiresIn,
  'refreshToken': instance.refreshToken,
};
