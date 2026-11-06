import 'package:memorio/core/result.dart';
import 'package:memorio/features/memories/memories.dart' show PickedPhoto;
import 'package:memorio/features/succession/succession.dart';

/// Succession from memory; tests set the next answers.
class FakeSuccessionRepository implements SuccessionRepository {
  List<TakeoverOffer> offers = [];
  TakeoverOffer? offer = takeoverOffer();
  AlbumTakeover takeover = const AlbumTakeover(canReserve: true, reservedByMe: false);
  String? contact;
  List<LegacyArchive> archives = [];
  List<LegacyMemory> memories = [];
  SuccessionFailure? loadFailure;
  SuccessionFailure? actionFailure;

  /// Every action, as "name:arguments", in order.
  final actions = <String>[];

  Result<T, SuccessionFailure> _load<T>(T value) {
    final failure = loadFailure;
    return failure == null ? Ok(value) : Err(failure);
  }

  Future<Result<void, SuccessionFailure>> _act(String action) async {
    actions.add(action);
    final failure = actionFailure;
    return failure == null ? const Ok(null) : Err(failure);
  }

  @override
  Future<Result<List<TakeoverOffer>, SuccessionFailure>> loadOffers() async => _load(offers);

  @override
  Future<Result<TakeoverOffer, SuccessionFailure>> loadOffer(String id) async {
    final known = offer;
    return known == null ? const Err(SuccessionNotSuccessor()) : _load(known);
  }

  @override
  Future<Result<void, SuccessionFailure>> answer(String id, {required bool accepted}) => _act('answer:$id:$accepted');

  @override
  Future<Result<void, SuccessionFailure>> report(String reportId, String albumId, PickedPhoto certificate) =>
      _act('report:$albumId:${certificate.name}');

  @override
  Future<Result<AlbumTakeover, SuccessionFailure>> loadTakeover(String albumId) async => _load(takeover);

  @override
  Future<Result<void, SuccessionFailure>> reserve(String albumId) => _act('reserve:$albumId');

  @override
  Future<Result<String?, SuccessionFailure>> loadContact() async => _load(contact);

  @override
  Future<Result<void, SuccessionFailure>> nameContact(String email) => _act('name:$email');

  @override
  Future<Result<void, SuccessionFailure>> removeContact() => _act('remove');

  @override
  Future<Result<List<LegacyArchive>, SuccessionFailure>> loadArchives() async => _load(archives);

  @override
  Future<Result<List<LegacyMemory>, SuccessionFailure>> loadArchive(String ownerAccountId) async => _load(memories);
}

TakeoverOffer takeoverOffer() => TakeoverOffer(
  id: 'case-1',
  albumId: 'album-1',
  albumName: 'Rodzina',
  answerBy: DateTime.utc(2030, 2, 1, 12),
  founderMemories: 3,
);
