// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:dio/dio.dart';
import 'package:retrofit/retrofit.dart';

import '../models/keep_material_request.dart';
import '../models/plan_response.dart';

part 'plans_client.g.dart';

@RestApi()
abstract class PlansClient {
  factory PlansClient(Dio dio, {String? baseUrl}) = _PlansClient;

  /// The plan of the person's account: mode, dates and what is used.
  @GET('/v1/plan')
  Future<PlanResponse> getPlan();

  /// Asks the payment source now instead of waiting for its notification.
  @POST('/v1/plan/refresh')
  Future<PlanResponse> refreshPlan();

  /// In the window before the trim: the memories to keep first; a new choice replaces the earlier one.
  @PUT('/v1/plan/kept-material')
  Future<void> keepMaterial({@Body() required KeepMaterialRequest body});

  /// For the owner: the plan of the album.
  @GET('/v1/albums/{id}/plan')
  Future<PlanResponse> getAlbumPlan({@Path('id') required String id});

  /// For the owner: asks the payment source now.
  @POST('/v1/albums/{id}/plan/refresh')
  Future<PlanResponse> refreshAlbumPlan({@Path('id') required String id});

  /// For the owner, in the window before the trim: the memories to keep first.
  @PUT('/v1/albums/{id}/plan/kept-material')
  Future<void> keepAlbumMaterial({@Path('id') required String id, @Body() required KeepMaterialRequest body});
}
