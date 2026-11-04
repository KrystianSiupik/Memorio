// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:dio/dio.dart';
import 'package:retrofit/retrofit.dart';

import '../models/shared_album_response.dart';

part 'access_client.g.dart';

@RestApi()
abstract class AccessClient {
  factory AccessClient(Dio dio, {String? baseUrl}) = _AccessClient;

  /// Albums the person may read without being in them. Empty until links exist (E4).
  @GET('/v1/shared-with-me')
  Future<List<SharedAlbumResponse>> getSharedWithMe();
}
