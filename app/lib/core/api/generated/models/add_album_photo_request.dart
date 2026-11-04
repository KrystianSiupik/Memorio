// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'add_album_photo_request.g.dart';

@JsonSerializable()
class AddAlbumPhotoRequest {
  const AddAlbumPhotoRequest({required this.photoId, required this.uploadKey});

  factory AddAlbumPhotoRequest.fromJson(Map<String, Object?> json) => _$AddAlbumPhotoRequestFromJson(json);

  final String photoId;
  final String? uploadKey;

  Map<String, Object?> toJson() => _$AddAlbumPhotoRequestToJson(this);
}
