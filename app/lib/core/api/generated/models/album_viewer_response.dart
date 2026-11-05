// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'album_viewer_response.g.dart';

@JsonSerializable()
class AlbumViewerResponse {
  const AlbumViewerResponse({required this.accountId, required this.email, required this.enteredAt});

  factory AlbumViewerResponse.fromJson(Map<String, Object?> json) => _$AlbumViewerResponseFromJson(json);

  final String accountId;
  final String? email;
  final DateTime enteredAt;

  Map<String, Object?> toJson() => _$AlbumViewerResponseToJson(this);
}
