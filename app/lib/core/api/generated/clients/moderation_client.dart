// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:dio/dio.dart';
import 'package:retrofit/retrofit.dart';

import '../models/appeal_request.dart';
import '../models/my_moderation_response.dart';
import '../models/report_request.dart';
import '../models/reported_response.dart';

part 'moderation_client.g.dart';

@RestApi()
abstract class ModerationClient {
  factory ModerationClient(Dio dio, {String? baseUrl}) = _ModerationClient;

  /// What the person reported, and the decisions about their own content, with the reasons.
  @GET('/v1/moderation/mine')
  Future<MyModerationResponse> getMyModeration();

  /// Reports a memory or an album the person sees; the id comes from the client, and the same report again changes nothing.
  @POST('/v1/reports')
  Future<ReportedResponse> reportContent({@Body() required ReportRequest body});

  /// The author appeals against the removal of their content, with reasons.
  @POST('/v1/reports/{id}/appeal')
  Future<void> appealDecision({@Path('id') required String id, @Body() required AppealRequest body});
}
