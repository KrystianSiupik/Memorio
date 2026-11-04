// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'kept_contribution_response.g.dart';

@JsonSerializable()
class KeptContributionResponse {
  const KeptContributionResponse({
    required this.albumId,
    required this.albumName,
    required this.chosenByAuthor,
    required this.memories,
  });

  factory KeptContributionResponse.fromJson(Map<String, Object?> json) => _$KeptContributionResponseFromJson(json);

  final String albumId;
  final String albumName;
  final bool chosenByAuthor;
  final dynamic memories;

  Map<String, Object?> toJson() => _$KeptContributionResponseToJson(this);
}
