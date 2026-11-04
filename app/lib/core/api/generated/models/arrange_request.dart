// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'arrange_request.g.dart';

@JsonSerializable()
class ArrangeRequest {
  const ArrangeRequest({required this.memoryIds});

  factory ArrangeRequest.fromJson(Map<String, Object?> json) => _$ArrangeRequestFromJson(json);

  final List<String>? memoryIds;

  Map<String, Object?> toJson() => _$ArrangeRequestToJson(this);
}
