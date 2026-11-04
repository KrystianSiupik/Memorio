// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'name_successor_request.g.dart';

@JsonSerializable()
class NameSuccessorRequest {
  const NameSuccessorRequest({required this.accountId});

  factory NameSuccessorRequest.fromJson(Map<String, Object?> json) => _$NameSuccessorRequestFromJson(json);

  final String accountId;

  Map<String, Object?> toJson() => _$NameSuccessorRequestToJson(this);
}
