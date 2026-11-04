// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'invite_request.g.dart';

@JsonSerializable()
class InviteRequest {
  const InviteRequest({required this.id, required this.email});

  factory InviteRequest.fromJson(Map<String, Object?> json) => _$InviteRequestFromJson(json);

  final String id;
  final String? email;

  Map<String, Object?> toJson() => _$InviteRequestToJson(this);
}
