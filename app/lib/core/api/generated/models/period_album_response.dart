// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'period_album_response.g.dart';

@JsonSerializable()
class PeriodAlbumResponse {
  const PeriodAlbumResponse({required this.added, required this.notFitting});

  factory PeriodAlbumResponse.fromJson(Map<String, Object?> json) => _$PeriodAlbumResponseFromJson(json);

  final dynamic added;
  final dynamic notFitting;

  Map<String, Object?> toJson() => _$PeriodAlbumResponseToJson(this);
}
