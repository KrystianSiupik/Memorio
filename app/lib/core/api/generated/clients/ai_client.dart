// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:dio/dio.dart';
import 'package:retrofit/retrofit.dart';

import '../models/ai_draft_response.dart';
import '../models/request_ai_draft_request.dart';

part 'ai_client.g.dart';

@RestApi()
abstract class AiClient {
  factory AiClient(Dio dio, {String? baseUrl}) = _AiClient;

  /// Writes a draft of a description with the AI; the id comes from the client, so a retry gets the same draft.
  @POST('/v1/ai/drafts')
  Future<AiDraftResponse> requestAiDraft({@Body() required RequestAiDraftRequest body});
}
