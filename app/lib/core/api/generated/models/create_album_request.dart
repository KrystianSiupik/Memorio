// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'create_album_request.g.dart';

@JsonSerializable()
class CreateAlbumRequest {
  const CreateAlbumRequest({required this.id, required this.name});

  factory CreateAlbumRequest.fromJson(Map<String, Object?> json) => _$CreateAlbumRequestFromJson(json);

  final String id;
  final String? name;

  Map<String, Object?> toJson() => _$CreateAlbumRequestToJson(this);
}
