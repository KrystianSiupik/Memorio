// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'account_deletion_response.g.dart';

@JsonSerializable()
class AccountDeletionResponse {
  const AccountDeletionResponse({required this.windowEndsAt});

  factory AccountDeletionResponse.fromJson(Map<String, Object?> json) => _$AccountDeletionResponseFromJson(json);

  final DateTime? windowEndsAt;

  Map<String, Object?> toJson() => _$AccountDeletionResponseToJson(this);
}
