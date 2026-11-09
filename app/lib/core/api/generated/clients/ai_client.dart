// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:dio/dio.dart';
import 'package:retrofit/retrofit.dart';

import '../models/ai_consent_response.dart';
import '../models/ai_draft_response.dart';
import '../models/ai_summary_response.dart';
import '../models/request_ai_draft_request.dart';
import '../models/request_ai_summary_request.dart';

part 'ai_client.g.dart';

@RestApi()
abstract class AiClient {
  factory AiClient(Dio dio, {String? baseUrl}) = _AiClient;

  /// Writes a draft of a description with the AI; the id comes from the client, so a retry gets the same draft.
  @POST('/v1/ai/drafts')
  Future<AiDraftResponse> requestAiDraft({@Body() required RequestAiDraftRequest body});

  /// Whether the signed-in person lets the AI use their content in this album (S2:C5).
  @GET('/v1/albums/{albumId}/ai-consent')
  Future<AiConsentResponse> getAiConsent({@Path('albumId') required String albumId});

  /// C80: the participant lets the AI use their content in this album; again changes nothing.
  @PUT('/v1/albums/{albumId}/ai-consent')
  Future<void> giveAiConsent({@Path('albumId') required String albumId});

  /// C81: the person takes the consent back; what was made from their content in this album is invalidated.
  @DELETE('/v1/albums/{albumId}/ai-consent')
  Future<void> revokeAiConsent({@Path('albumId') required String albumId});

  /// C82: the AI's story of a period of an album, from the materials the person may use; the id comes from the client.
  @POST('/v1/ai/summaries')
  Future<AiSummaryResponse> requestAiSummary({@Body() required RequestAiSummaryRequest body});

  /// One summary of the person who asked for it.
  @GET('/v1/ai/summaries/{id}')
  Future<AiSummaryResponse> getAiSummary({@Path('id') required String id});

  /// C83: the person approves the summary as it is and keeps it; the memories it tells of are checked again.
  @POST('/v1/ai/summaries/{id}/save')
  Future<AiSummaryResponse> saveAiSummary({@Path('id') required String id});

  /// C84: a saved summary shown to the participants of its album.
  @POST('/v1/ai/summaries/{id}/album')
  Future<AiSummaryResponse> addAiSummaryToAlbum({@Path('id') required String id});

  /// The summaries added to the album that this participant may read every memory of, the newest first.
  @GET('/v1/albums/{albumId}/ai-summaries')
  Future<List<AiSummaryResponse>> listAlbumAiSummaries({@Path('albumId') required String albumId});
}
