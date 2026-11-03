// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'account_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AccountResponse _$AccountResponseFromJson(Map<String, dynamic> json) => AccountResponse(
  accountId: json['accountId'] as String,
  email: json['email'] as String?,
  acceptedTermsVersion: json['acceptedTermsVersion'] as String?,
  currentTermsVersion: json['currentTermsVersion'] as String,
);

Map<String, dynamic> _$AccountResponseToJson(AccountResponse instance) => <String, dynamic>{
  'accountId': instance.accountId,
  'email': instance.email,
  'acceptedTermsVersion': instance.acceptedTermsVersion,
  'currentTermsVersion': instance.currentTermsVersion,
};
