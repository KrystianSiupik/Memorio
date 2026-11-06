import 'package:dio/dio.dart';

import '../../../core/api/generated/clients/access_client.dart';
import '../../../core/api/generated/clients/albums_client.dart';
import '../../../core/api/generated/clients/memories_client.dart';
import '../../../core/api/generated/clients/succession_client.dart';
import '../../../core/api/generated/models/name_legacy_contact_request.dart';
import '../../../core/api/generated/models/report_death_request.dart';
import '../../../core/api/generated/models/start_certificate_upload_request.dart';
import '../../../core/api/generated/models/takeover_offer_response.dart';
import '../../../core/result.dart';
import '../../memories/memories.dart' show PickedPhoto;
import '../domain/succession.dart';

/// [SuccessionRepository] on API v1. The contract describes numbers as "integer or string", so the generated models
/// have them dynamic; the backend always sends numbers (D-212).
class HttpSuccessionRepository implements SuccessionRepository {
  const HttpSuccessionRepository({
    required this.succession,
    required this.albums,
    required this.access,
    required this.memories,
    required this.uploads,
  });

  final SuccessionClient succession;
  final AlbumsClient albums;
  final AccessClient access;
  final MemoriesClient memories;

  /// Sends files straight to the storage: its address is signed, so no sign-in and no API headers.
  final Dio uploads;

  @override
  Future<Result<List<TakeoverOffer>, SuccessionFailure>> loadOffers() =>
      _attempt(() async => [for (final offer in await succession.listTakeoverOffers()) _offerOf(offer)]);

  @override
  Future<Result<TakeoverOffer, SuccessionFailure>> loadOffer(String id) =>
      _attempt(() async => _offerOf(await succession.getTakeoverOffer(id: id)));

  @override
  Future<Result<void, SuccessionFailure>> answer(String id, {required bool accepted}) =>
      _attempt(() => accepted ? succession.acceptTakeover(id: id) : succession.refuseTakeover(id: id));

  @override
  Future<Result<void, SuccessionFailure>> report(String reportId, String albumId, PickedPhoto certificate) =>
      _attempt(() async {
        final upload = await succession.startCertificateUpload(
          body: StartCertificateUploadRequest(contentType: certificate.contentType),
        );
        await uploads.put<void>(
          upload.url,
          data: await certificate.read(),
          options: Options(contentType: upload.contentType),
        );
        await succession.reportOwnerDeath(
          id: albumId,
          body: ReportDeathRequest(id: reportId, uploadKey: upload.key),
        );
      });

  @override
  Future<Result<AlbumTakeover, SuccessionFailure>> loadTakeover(String albumId) => _attempt(() async {
    final takeover = await albums.getAlbumTakeover(id: albumId);
    return AlbumTakeover(
      canReserve: takeover.canReserve,
      reservedByMe: takeover.reservedByMe,
      reservedUntil: takeover.reservedUntil,
    );
  });

  @override
  Future<Result<void, SuccessionFailure>> reserve(String albumId) =>
      _attempt(() => albums.reserveAlbumTakeover(id: albumId));

  @override
  Future<Result<String?, SuccessionFailure>> loadContact() =>
      _attempt(() async => (await succession.getLegacyContact()).email);

  @override
  Future<Result<void, SuccessionFailure>> nameContact(String email) =>
      _attempt(() => succession.nameLegacyContact(body: NameLegacyContactRequest(email: email)));

  @override
  Future<Result<void, SuccessionFailure>> removeContact() => _attempt(succession.removeLegacyContact);

  @override
  Future<Result<List<LegacyArchive>, SuccessionFailure>> loadArchives() => _attempt(() async {
    return [
      for (final archive in await access.listLegacyArchives())
        LegacyArchive(ownerAccountId: archive.ownerAccountId, email: archive.email),
    ];
  });

  @override
  Future<Result<List<LegacyMemory>, SuccessionFailure>> loadArchive(String ownerAccountId) => _attempt(() async {
    return [
      for (final memory in await memories.listLegacyMemories(ownerAccountId: ownerAccountId))
        LegacyMemory(id: memory.id, placeName: memory.placeName, dateFrom: memory.dateFrom),
    ];
  });

  static TakeoverOffer _offerOf(TakeoverOfferResponse offer) => TakeoverOffer(
    id: offer.id,
    albumId: offer.albumId,
    albumName: offer.albumName,
    answerBy: offer.answerBy,
    founderMemories: offer.founderMemories as int,
  );

  Future<Result<T, SuccessionFailure>> _attempt<T>(Future<T> Function() call) async {
    try {
      return Ok(await call());
    } on DioException catch (error) {
      return Err(_failureOf(error));
    }
  }

  static SuccessionFailure _failureOf(DioException error) {
    final response = error.response;
    if (response == null) {
      return const SuccessionOffline();
    }
    final body = response.data;
    final code = body is Map ? body['code'] : null;
    return switch (code) {
      'Succession.NotSuccessor' => const SuccessionNotSuccessor(),
      'Succession.AlreadyDecided' || 'Album.HasOwner' => const SuccessionAlreadyDecided(),
      'Succession.AlreadyReported' => const SuccessionAlreadyReported(),
      'Album.TakeoverReserved' => const SuccessionReservedByOther(),
      'Succession.InvalidEmail' || 'Succession.CannotNameYourself' => const SuccessionInvalidEmail(),
      _ => switch (response.statusCode) {
        403 || 404 => const SuccessionNoAccess(),
        409 => const SuccessionAlreadyDecided(),
        _ => const SuccessionServerError(),
      },
    };
  }
}
