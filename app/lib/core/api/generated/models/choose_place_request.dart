// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'choose_place_request.g.dart';

@JsonSerializable()
class ChoosePlaceRequest {
  const ChoosePlaceRequest({required this.placeReference});

  factory ChoosePlaceRequest.fromJson(Map<String, Object?> json) => _$ChoosePlaceRequestFromJson(json);

  final String? placeReference;

  Map<String, Object?> toJson() => _$ChoosePlaceRequestToJson(this);
}
