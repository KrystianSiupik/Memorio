// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:dio/dio.dart';
import 'package:retrofit/retrofit.dart';

import '../models/accept_terms_request.dart';
import '../models/account_deletion_impact_response.dart';
import '../models/account_deletion_response.dart';
import '../models/account_response.dart';
import '../models/request_account_deletion_request.dart';

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

  /// In numbers, what goes when the signed-in person deletes their account.
  @GET('/v1/account/deletion-impact')
  Future<AccountDeletionImpactResponse> getAccountDeletionImpact();

  /// Opens the 30-day window for deleting the account; asking again changes nothing.
  @POST('/v1/account/deletion')
  Future<AccountDeletionResponse> requestAccountDeletion({@Body() required RequestAccountDeletionRequest body});

  /// Whether a window for deleting the account is open, and until when.
  @GET('/v1/account/deletion')
  Future<AccountDeletionResponse> getAccountDeletion();
}
