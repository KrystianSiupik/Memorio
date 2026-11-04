// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'add_memory_request.g.dart';

@JsonSerializable()
class AddMemoryRequest {
  const AddMemoryRequest({required this.memoryId});

  factory AddMemoryRequest.fromJson(Map<String, Object?> json) => _$AddMemoryRequestFromJson(json);

  final String memoryId;

  Map<String, Object?> toJson() => _$AddMemoryRequestToJson(this);
}
