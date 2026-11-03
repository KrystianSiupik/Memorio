// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'add_own_place_request.g.dart';

@JsonSerializable()
class AddOwnPlaceRequest {
  const AddOwnPlaceRequest({required this.id, required this.name, required this.latitude, required this.longitude});

  factory AddOwnPlaceRequest.fromJson(Map<String, Object?> json) => _$AddOwnPlaceRequestFromJson(json);

  final String id;
  final String? name;
  final dynamic latitude;
  final dynamic longitude;

  Map<String, Object?> toJson() => _$AddOwnPlaceRequestToJson(this);
}
