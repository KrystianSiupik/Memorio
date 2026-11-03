// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'create_memory_request.g.dart';

@JsonSerializable()
class CreateMemoryRequest {
  const CreateMemoryRequest({
    required this.id,
    required this.placeReference,
    required this.datePrecision,
    required this.dateFrom,
    required this.dateTo,
  });

  factory CreateMemoryRequest.fromJson(Map<String, Object?> json) => _$CreateMemoryRequestFromJson(json);

  final String id;
  final String? placeReference;
  final String? datePrecision;
  final String? dateFrom;
  final String? dateTo;

  Map<String, Object?> toJson() => _$CreateMemoryRequestToJson(this);
}
