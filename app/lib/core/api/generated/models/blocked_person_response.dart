// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'blocked_person_response.g.dart';

@JsonSerializable()
class BlockedPersonResponse {
  const BlockedPersonResponse({required this.accountId, required this.email});

  factory BlockedPersonResponse.fromJson(Map<String, Object?> json) => _$BlockedPersonResponseFromJson(json);

  final String accountId;
  final String? email;

  Map<String, Object?> toJson() => _$BlockedPersonResponseToJson(this);
}
