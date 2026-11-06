// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'name_legacy_contact_request.g.dart';

@JsonSerializable()
class NameLegacyContactRequest {
  const NameLegacyContactRequest({required this.email});

  factory NameLegacyContactRequest.fromJson(Map<String, Object?> json) => _$NameLegacyContactRequestFromJson(json);

  final String? email;

  Map<String, Object?> toJson() => _$NameLegacyContactRequestToJson(this);
}
