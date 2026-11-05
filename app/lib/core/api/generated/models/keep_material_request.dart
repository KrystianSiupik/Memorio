// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'keep_material_request.g.dart';

@JsonSerializable()
class KeepMaterialRequest {
  const KeepMaterialRequest({required this.memoryIds});

  factory KeepMaterialRequest.fromJson(Map<String, Object?> json) => _$KeepMaterialRequestFromJson(json);

  final List<String>? memoryIds;

  Map<String, Object?> toJson() => _$KeepMaterialRequestToJson(this);
}
