// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:dio/dio.dart';

import 'clients/app_client.dart';
import 'clients/account_client.dart';
import 'clients/auth_client.dart';

/// Memorio API `vv1`
class MemorioApiClient {
  MemorioApiClient(Dio dio, {String? baseUrl}) : _dio = dio, _baseUrl = baseUrl;

  final Dio _dio;
  final String? _baseUrl;

  static String get version => 'v1';

  AppClient? _app;
  AccountClient? _account;
  AuthClient? _auth;

  AppClient get app => _app ??= AppClient(_dio, baseUrl: _baseUrl);

  AccountClient get account => _account ??= AccountClient(_dio, baseUrl: _baseUrl);

  AuthClient get auth => _auth ??= AuthClient(_dio, baseUrl: _baseUrl);
}
