// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:dio/dio.dart';
import 'package:retrofit/retrofit.dart';

import '../models/access_token_response.dart';
import '../models/external_sign_in_request.dart';
import '../models/login_request.dart';
import '../models/refresh_request.dart';
import '../models/register_request.dart';

part 'auth_client.g.dart';

@RestApi()
abstract class AuthClient {
  factory AuthClient(Dio dio, {String? baseUrl}) = _AuthClient;

  /// Creates an account with email and password (16+ declaration required) and signs in.
  @POST('/v1/auth/register')
  Future<AccessTokenResponse> register({@Body() required RegisterRequest body});

  /// Signs in with email and password.
  @POST('/v1/auth/login')
  Future<AccessTokenResponse> login({@Body() required LoginRequest body});

  /// Exchanges a refresh token for new tokens.
  @POST('/v1/auth/refresh')
  Future<AccessTokenResponse> refreshToken({@Body() required RefreshRequest body});

  /// Signs in with a Google or Apple ID token; a new account needs the 16+ declaration.
  @POST('/v1/auth/external/{provider}')
  Future<AccessTokenResponse> signInWithProvider({
    @Path('provider') required String provider,
    @Body() required ExternalSignInRequest body,
  });
}
