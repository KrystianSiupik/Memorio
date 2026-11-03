// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:json_annotation/json_annotation.dart';

part 'request_account_deletion_request.g.dart';

@JsonSerializable()
class RequestAccountDeletionRequest {
  const RequestAccountDeletionRequest({required this.consentsToAlbumExpiry});

  factory RequestAccountDeletionRequest.fromJson(Map<String, Object?> json) =>
      _$RequestAccountDeletionRequestFromJson(json);

  final bool consentsToAlbumExpiry;

  Map<String, Object?> toJson() => _$RequestAccountDeletionRequestToJson(this);
}
