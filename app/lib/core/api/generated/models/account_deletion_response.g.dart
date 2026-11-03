// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'account_deletion_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AccountDeletionResponse _$AccountDeletionResponseFromJson(Map<String, dynamic> json) => AccountDeletionResponse(
  windowEndsAt: json['windowEndsAt'] == null ? null : DateTime.parse(json['windowEndsAt'] as String),
);

Map<String, dynamic> _$AccountDeletionResponseToJson(AccountDeletionResponse instance) => <String, dynamic>{
  'windowEndsAt': instance.windowEndsAt?.toIso8601String(),
};
