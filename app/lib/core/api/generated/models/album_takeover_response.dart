// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'album_takeover_response.g.dart';

@JsonSerializable()
class AlbumTakeoverResponse {
  const AlbumTakeoverResponse({
    required this.isOwnerless,
    required this.canReserve,
    required this.reservedByMe,
    required this.reservedUntil,
  });

  factory AlbumTakeoverResponse.fromJson(Map<String, Object?> json) => _$AlbumTakeoverResponseFromJson(json);

  final bool isOwnerless;
  final bool canReserve;
  final bool reservedByMe;
  final DateTime? reservedUntil;

  Map<String, Object?> toJson() => _$AlbumTakeoverResponseToJson(this);
}
