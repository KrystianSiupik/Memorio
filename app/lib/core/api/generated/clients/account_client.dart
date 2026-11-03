// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:dio/dio.dart';
import 'package:retrofit/retrofit.dart';

import '../models/accept_terms_request.dart';
import '../models/account_response.dart';

part 'account_client.g.dart';

@RestApi()
abstract class AccountClient {
  factory AccountClient(Dio dio, {String? baseUrl}) = _AccountClient;

  /// The signed-in account.
  @GET('/v1/account/me')
  Future<AccountResponse> getMe();

  /// Records that the signed-in person accepted the current version of the terms.
  @POST('/v1/account/terms-acceptance')
  Future<void> acceptTerms({@Body() required AcceptTermsRequest body});
}
