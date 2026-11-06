// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'legacy_contact_response.g.dart';

@JsonSerializable()
class LegacyContactResponse {
  const LegacyContactResponse({required this.email});

  factory LegacyContactResponse.fromJson(Map<String, Object?> json) => _$LegacyContactResponseFromJson(json);

  final String? email;

  Map<String, Object?> toJson() => _$LegacyContactResponseToJson(this);
}
