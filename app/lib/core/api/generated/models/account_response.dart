// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'account_response.g.dart';

@JsonSerializable()
class AccountResponse {
  const AccountResponse({
    required this.accountId,
    required this.email,
    required this.acceptedTermsVersion,
    required this.currentTermsVersion,
  });

  factory AccountResponse.fromJson(Map<String, Object?> json) => _$AccountResponseFromJson(json);

  final String accountId;
  final String? email;
  final String? acceptedTermsVersion;
  final String currentTermsVersion;

  Map<String, Object?> toJson() => _$AccountResponseToJson(this);
}
