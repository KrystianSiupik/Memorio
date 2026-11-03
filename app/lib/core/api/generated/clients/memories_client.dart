// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:dio/dio.dart';
import 'package:retrofit/retrofit.dart';

import '../models/add_photo_request.dart';
import '../models/approve_description_request.dart';
import '../models/create_memory_request.dart';
import '../models/created_memory_response.dart';
import '../models/memory_details_response.dart';
import '../models/memory_point_response.dart';
import '../models/request_data_deletion_request.dart';
import '../models/start_upload_request.dart';
import '../models/trashed_memory_response.dart';
import '../models/upload_response.dart';

part 'memories_client.g.dart';

@RestApi()
abstract class MemoriesClient {
  factory MemoriesClient(Dio dio, {String? baseUrl}) = _MemoriesClient;

  /// Records a memory; the id comes from the client, so a retry changes nothing.
  @POST('/v1/memories')
  Future<CreatedMemoryResponse> createMemory({@Body() required CreateMemoryRequest body});

  /// One point per memory the person may see; from and to (yyyy-MM-dd) filter by overlap.
  @GET('/v1/memories/map')
  Future<List<MemoryPointResponse>> getMemoryMap({@Query('from') String? from, @Query('to') String? to});

  /// Takes the person's content back from the trash while its 30 days last.
  @POST('/v1/memories/{id}/restore')
  Future<void> restoreMemory({@Path('id') required String id});

  /// The person's own content in the trash, while its 30 days last.
  @GET('/v1/memories/trash')
  Future<List<TrashedMemoryResponse>> getTrash();

  /// The author's formal request: the memory goes for good at once, without the trash.
  @POST('/v1/memories/{id}/permanent-deletion')
  Future<void> requestDataDeletion({@Path('id') required String id, @Body() required RequestDataDeletionRequest body});

  /// Moves the person's content to the trash for 30 days.
  @DELETE('/v1/memories/{id}')
  Future<void> trashMemory({@Path('id') required String id});

  /// Everything about one memory the person may see.
  @GET('/v1/memories/{id}')
  Future<MemoryDetailsResponse> getMemory({@Path('id') required String id});

  /// The author approves the description of a memory, naming the AI draft it started from, if any.
  @PUT('/v1/memories/{id}/description')
  Future<void> approveDescription({@Path('id') required String id, @Body() required ApproveDescriptionRequest body});

  /// An address to send one photo to, valid for 5 minutes.
  @POST('/v1/memories/uploads')
  Future<UploadResponse> startPhotoUpload({@Body() required StartUploadRequest body});

  /// Adds an uploaded photo to a memory; the photo id comes from the client, so a retry changes nothing.
  @POST('/v1/memories/{id}/photos')
  Future<void> addPhoto({@Path('id') required String id, @Body() required AddPhotoRequest body});
}
