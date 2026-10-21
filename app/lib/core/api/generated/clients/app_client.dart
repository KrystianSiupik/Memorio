// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:dio/dio.dart';
import 'package:retrofit/retrofit.dart';

import '../models/app_version_policy_response.dart';

part 'app_client.g.dart';

@RestApi()
abstract class AppClient {
  factory AppClient(Dio dio, {String? baseUrl}) = _AppClient;

  /// Minimum app version the API serves.
  @GET('/v1/app/version-policy')
  Future<AppVersionPolicyResponse> getAppVersionPolicy();
}
