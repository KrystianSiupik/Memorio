// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'create_album_from_period_request.g.dart';

@JsonSerializable()
class CreateAlbumFromPeriodRequest {
  const CreateAlbumFromPeriodRequest({required this.id, required this.name, required this.from, required this.to});

  factory CreateAlbumFromPeriodRequest.fromJson(Map<String, Object?> json) =>
      _$CreateAlbumFromPeriodRequestFromJson(json);

  final String id;
  final String? name;
  final String? from;
  final String? to;

  Map<String, Object?> toJson() => _$CreateAlbumFromPeriodRequestToJson(this);
}
