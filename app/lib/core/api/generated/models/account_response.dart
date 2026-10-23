// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'account_response.g.dart';

@JsonSerializable()
class AccountResponse {
  const AccountResponse({required this.accountId, required this.email});

  factory AccountResponse.fromJson(Map<String, Object?> json) => _$AccountResponseFromJson(json);

  final String accountId;
  final String? email;

  Map<String, Object?> toJson() => _$AccountResponseToJson(this);
}
