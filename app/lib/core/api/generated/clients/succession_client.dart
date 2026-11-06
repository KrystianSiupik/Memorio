// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, unused_import, invalid_annotation_target, unnecessary_import

import 'package:dio/dio.dart';
import 'package:retrofit/retrofit.dart';

import '../models/certificate_upload_response.dart';
import '../models/legacy_contact_response.dart';
import '../models/name_legacy_contact_request.dart';
import '../models/report_death_request.dart';
import '../models/start_certificate_upload_request.dart';
import '../models/takeover_offer_response.dart';

part 'succession_client.g.dart';

@RestApi()
abstract class SuccessionClient {
  factory SuccessionClient(Dio dio, {String? baseUrl}) = _SuccessionClient;

  /// Someone in the album reports the death of its owner, with the certificate; the service checks it.
  @POST('/v1/albums/{id}/death-reports')
  Future<void> reportOwnerDeath({@Path('id') required String id, @Body() required ReportDeathRequest body});

  /// An address to send one certificate to, valid for 5 minutes.
  @POST('/v1/succession/uploads')
  Future<CertificateUploadResponse> startCertificateUpload({@Body() required StartCertificateUploadRequest body});

  /// Albums waiting for the person's answer as their successor.
  @GET('/v1/succession/offers')
  Future<List<TakeoverOfferResponse>> listTakeoverOffers();

  /// For the successor named: the album, the time to answer and what the founder brought.
  @GET('/v1/succession/offers/{id}')
  Future<TakeoverOfferResponse> getTakeoverOffer({@Path('id') required String id});

  /// The successor takes the album over.
  @POST('/v1/succession/offers/{id}/acceptance')
  Future<void> acceptTakeover({@Path('id') required String id});

  /// The successor refuses; the album is left without an owner.
  @POST('/v1/succession/offers/{id}/refusal')
  Future<void> refuseTakeover({@Path('id') required String id});

  /// Who may read the person's private memories after their death.
  @GET('/v1/legacy-contact')
  Future<LegacyContactResponse> getLegacyContact();

  /// Names that person, or replaces them.
  @PUT('/v1/legacy-contact')
  Future<void> nameLegacyContact({@Body() required NameLegacyContactRequest body});

  /// Nobody will get it; asking again changes nothing.
  @DELETE('/v1/legacy-contact')
  Future<void> removeLegacyContact();
}
