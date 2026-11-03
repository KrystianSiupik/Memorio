// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'created_memory_response.g.dart';

@JsonSerializable()
class CreatedMemoryResponse {
  const CreatedMemoryResponse({required this.id});

  factory CreatedMemoryResponse.fromJson(Map<String, Object?> json) => _$CreatedMemoryResponseFromJson(json);

  final String id;

  Map<String, Object?> toJson() => _$CreatedMemoryResponseToJson(this);
}
