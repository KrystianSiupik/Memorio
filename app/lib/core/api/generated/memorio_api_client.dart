// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:dio/dio.dart';

import 'clients/app_client.dart';
import 'clients/access_client.dart';
import 'clients/ai_client.dart';
import 'clients/account_client.dart';
import 'clients/albums_client.dart';
import 'clients/succession_client.dart';
import 'clients/notifications_client.dart';
import 'clients/memories_client.dart';
import 'clients/places_client.dart';
import 'clients/plans_client.dart';
import 'clients/auth_client.dart';

/// Memorio API `vv1`
class MemorioApiClient {
  MemorioApiClient(Dio dio, {String? baseUrl}) : _dio = dio, _baseUrl = baseUrl;

  final Dio _dio;
  final String? _baseUrl;

  static String get version => 'v1';

  AppClient? _app;
  AccessClient? _access;
  AiClient? _ai;
  AccountClient? _account;
  AlbumsClient? _albums;
  SuccessionClient? _succession;
  NotificationsClient? _notifications;
  MemoriesClient? _memories;
  PlacesClient? _places;
  PlansClient? _plans;
  AuthClient? _auth;

  AppClient get app => _app ??= AppClient(_dio, baseUrl: _baseUrl);

  AccessClient get access => _access ??= AccessClient(_dio, baseUrl: _baseUrl);

  AiClient get ai => _ai ??= AiClient(_dio, baseUrl: _baseUrl);

  AccountClient get account => _account ??= AccountClient(_dio, baseUrl: _baseUrl);

  AlbumsClient get albums => _albums ??= AlbumsClient(_dio, baseUrl: _baseUrl);

  SuccessionClient get succession => _succession ??= SuccessionClient(_dio, baseUrl: _baseUrl);

  NotificationsClient get notifications => _notifications ??= NotificationsClient(_dio, baseUrl: _baseUrl);

  MemoriesClient get memories => _memories ??= MemoriesClient(_dio, baseUrl: _baseUrl);

  PlacesClient get places => _places ??= PlacesClient(_dio, baseUrl: _baseUrl);

  PlansClient get plans => _plans ??= PlansClient(_dio, baseUrl: _baseUrl);

  AuthClient get auth => _auth ??= AuthClient(_dio, baseUrl: _baseUrl);
}
