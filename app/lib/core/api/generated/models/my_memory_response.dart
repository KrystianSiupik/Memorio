// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'my_memory_response.g.dart';

@JsonSerializable()
class MyMemoryResponse {
  const MyMemoryResponse({required this.id, required this.placeName, required this.dateFrom});

  factory MyMemoryResponse.fromJson(Map<String, Object?> json) => _$MyMemoryResponseFromJson(json);

  final String id;
  final String placeName;
  final String dateFrom;

  Map<String, Object?> toJson() => _$MyMemoryResponseToJson(this);
}
