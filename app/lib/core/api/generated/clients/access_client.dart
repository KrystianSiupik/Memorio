// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:dio/dio.dart';
import 'package:retrofit/retrofit.dart';

import '../models/album_link_preview_response.dart';
import '../models/album_link_response.dart';
import '../models/album_viewer_response.dart';
import '../models/blocked_person_response.dart';
import '../models/entered_album_response.dart';
import '../models/shared_album_response.dart';

part 'access_client.g.dart';

@RestApi()
abstract class AccessClient {
  factory AccessClient(Dio dio, {String? baseUrl}) = _AccessClient;

  /// Albums the person may read without being in them. Empty until links exist (E4).
  @GET('/v1/shared-with-me')
  Future<List<SharedAlbumResponse>> getSharedWithMe();

  /// The owner gets the link; a working link stays the same.
  @POST('/v1/albums/{id}/link')
  Future<AlbumLinkResponse> shareAlbumByLink({@Path('id') required String id});

  /// For the owner: the working link, if there is one.
  @GET('/v1/albums/{id}/link')
  Future<AlbumLinkResponse> getAlbumLink({@Path('id') required String id});

  /// Nobody new comes in; those let in keep their access.
  @DELETE('/v1/albums/{id}/link')
  Future<void> revokeAlbumLink({@Path('id') required String id});

  /// For people in the album: who reads it through the link, by name.
  @GET('/v1/albums/{id}/viewers')
  Future<List<AlbumViewerResponse>> getAlbumViewers({@Path('id') required String id});

  /// The owner takes the access of someone let in; they may come back through a working link.
  @DELETE('/v1/albums/{id}/viewers/{accountId}')
  Future<void> revokeAlbumAccess({@Path('id') required String id, @Path('accountId') required String accountId});

  /// The person may not come in by a link or an invitation, and loses access at once.
  @PUT('/v1/albums/{id}/blocks/{accountId}')
  Future<void> blockUser({@Path('id') required String id, @Path('accountId') required String accountId});

  /// The owner takes a block back; it gives no access by itself.
  @DELETE('/v1/albums/{id}/blocks/{accountId}')
  Future<void> unblockUser({@Path('id') required String id, @Path('accountId') required String accountId});

  /// For the owner: who is blocked in this album.
  @GET('/v1/albums/{id}/blocks')
  Future<List<BlockedPersonResponse>> getAlbumBlocks({@Path('id') required String id});

  /// Which album a working link leads to.
  @GET('/v1/album-links/{token}')
  Future<AlbumLinkPreviewResponse> previewAlbumLink({@Path('token') required String token});

  /// Comes in through the link, by name.
  @POST('/v1/album-links/{token}/entry')
  Future<EnteredAlbumResponse> enterAlbumByLink({@Path('token') required String token});
}
