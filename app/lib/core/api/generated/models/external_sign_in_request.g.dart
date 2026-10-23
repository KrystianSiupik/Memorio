// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'external_sign_in_request.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ExternalSignInRequest _$ExternalSignInRequestFromJson(Map<String, dynamic> json) =>
    ExternalSignInRequest(idToken: json['idToken'] as String?, declaresMinimumAge: json['declaresMinimumAge'] as bool);

Map<String, dynamic> _$ExternalSignInRequestToJson(ExternalSignInRequest instance) => <String, dynamic>{
  'idToken': instance.idToken,
  'declaresMinimumAge': instance.declaresMinimumAge,
};
