// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'entrust_request.g.dart';

@JsonSerializable()
class EntrustRequest {
  const EntrustRequest({required this.curatorAccountId});

  factory EntrustRequest.fromJson(Map<String, Object?> json) => _$EntrustRequestFromJson(json);

  final String curatorAccountId;

  Map<String, Object?> toJson() => _$EntrustRequestToJson(this);
}
