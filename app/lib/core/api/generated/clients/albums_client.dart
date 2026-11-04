// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:dio/dio.dart';
import 'package:retrofit/retrofit.dart';

import '../models/add_album_photo_request.dart';
import '../models/add_memory_request.dart';
import '../models/album_list_item_response.dart';
import '../models/album_response.dart';
import '../models/album_seats_response.dart';
import '../models/answered_invitation_response.dart';
import '../models/arrange_request.dart';
import '../models/create_album_request.dart';
import '../models/entrust_request.dart';
import '../models/entrusted_memory_response.dart';
import '../models/invitation_response.dart';
import '../models/invite_request.dart';
import '../models/kept_contribution_response.dart';
import '../models/leave_request.dart';
import '../models/name_successor_request.dart';

part 'albums_client.g.dart';

@RestApi()
abstract class AlbumsClient {
  factory AlbumsClient(Dio dio, {String? baseUrl}) = _AlbumsClient;

  /// The owner names someone in the album as its successor.
  @PUT('/v1/albums/{id}/successor')
  Future<void> nameSuccessor({@Path('id') required String id, @Body() required NameSuccessorRequest body});

  /// Creates an album; the id comes from the client, so a retry changes nothing.
  @POST('/v1/albums')
  Future<void> createAlbum({@Body() required CreateAlbumRequest body});

  /// The albums the person owns or takes part in.
  @GET('/v1/albums')
  Future<List<AlbumListItemResponse>> listAlbums();

  /// What stayed in albums the person is no longer in.
  @GET('/v1/albums/kept-contributions')
  Future<List<KeptContributionResponse>> listKeptContributions();

  /// Memories entrusted to the person as a curator.
  @GET('/v1/albums/entrusted')
  Future<List<EntrustedMemoryResponse>> listEntrusted();

  /// The album with its memories, members, the person's own contributions and the pool.
  @GET('/v1/albums/{id}')
  Future<AlbumResponse> getAlbum({@Path('id') required String id});

  /// For the owner: seats taken, invited and free.
  @GET('/v1/albums/{id}/seats')
  Future<AlbumSeatsResponse> getAlbumSeats({@Path('id') required String id});

  /// Invites an address; the same address again refreshes its invitation and takes no second seat.
  @POST('/v1/albums/{id}/invitations')
  Future<void> inviteToAlbum({@Path('id') required String id, @Body() required InviteRequest body});

  /// The owner takes an invitation back; the seat is free at once.
  @DELETE('/v1/albums/{id}/invitations/{invitationId}')
  Future<void> withdrawInvitation({@Path('id') required String id, @Path('invitationId') required String invitationId});

  /// The owner removes a person; what they brought stays until they choose.
  @DELETE('/v1/albums/{id}/participants/{accountId}')
  Future<void> removeParticipant({@Path('id') required String id, @Path('accountId') required String accountId});

  /// Leaves the album, keeping what the person brought in it or taking it along.
  @POST('/v1/albums/{id}/leave')
  Future<void> leaveAlbum({@Path('id') required String id, @Body() required LeaveRequest body});

  /// The author brings their memory; the album's pool pays for it.
  @POST('/v1/albums/{id}/memories')
  Future<void> addMemoryToAlbum({@Path('id') required String id, @Body() required AddMemoryRequest body});

  /// The author adds an uploaded photo to their memory in the album; the album's pool pays for it.
  @POST('/v1/albums/{id}/memories/{memoryId}/photos')
  Future<void> addPhotoInAlbum({
    @Path('id') required String id,
    @Path('memoryId') required String memoryId,
    @Body() required AddAlbumPhotoRequest body,
  });

  /// Tags someone in this album in the memory.
  @PUT('/v1/albums/{id}/memories/{memoryId}/tags/{accountId}')
  Future<void> tagParticipant({
    @Path('id') required String id,
    @Path('memoryId') required String memoryId,
    @Path('accountId') required String accountId,
  });

  /// Removes the person's own tag.
  @DELETE('/v1/albums/{id}/memories/{memoryId}/tags/me')
  Future<void> removeOwnTag({@Path('id') required String id, @Path('memoryId') required String memoryId});

  /// Takes back what stayed in the album after the person left or was removed.
  @POST('/v1/albums/{id}/contributions/withdrawal')
  Future<void> revokeKeptContributions({@Path('id') required String id});

  /// Lets someone in the album put the person's contributions in order.
  @PUT('/v1/albums/{id}/curator')
  Future<void> entrustToCurator({@Path('id') required String id, @Body() required EntrustRequest body});

  /// Ends the entrustment; asking again changes nothing.
  @DELETE('/v1/albums/{id}/curator')
  Future<void> revokeEntrustment({@Path('id') required String id});

  /// The curator orders the memories entrusted to them.
  @PUT('/v1/albums/{id}/arrangement')
  Future<void> arrangeEntrusted({@Path('id') required String id, @Body() required ArrangeRequest body});

  /// Invitations waiting for the person's address: who invites and to what, no content.
  @GET('/v1/invitations')
  Future<List<InvitationResponse>> listInvitations();

  /// Joins the album.
  @POST('/v1/invitations/{id}/acceptance')
  Future<AnsweredInvitationResponse> acceptInvitation({@Path('id') required String id});

  /// Refuses; the seat is free at once.
  @POST('/v1/invitations/{id}/refusal')
  Future<void> declineInvitation({@Path('id') required String id});
}
