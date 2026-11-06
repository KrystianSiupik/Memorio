import 'package:memorio/core/result.dart';
import 'package:memorio/features/replay/replay.dart';

/// Periods from memory; tests set the next answers.
class FakeReplayRepository implements ReplayRepository {
  List<PeriodMemory> memories = [];
  ReplayFailure? loadFailure;
  ReplayFailure? actionFailure;
  PeriodAlbum created = const PeriodAlbum(added: 2, notFitting: 0);

  final periods = <String>[];
  final actions = <String>[];

  @override
  Future<Result<List<PeriodMemory>, ReplayFailure>> loadPeriod(String from, String to) async {
    periods.add('$from..$to');
    final failure = loadFailure;
    return failure == null ? Ok(memories) : Err(failure);
  }

  @override
  Future<Result<PeriodAlbum, ReplayFailure>> createAlbum(String id, String name, String from, String to) async {
    actions.add('album:$name:$from..$to');
    final failure = actionFailure;
    return failure == null ? Ok(created) : Err(failure);
  }
}

PeriodMemory periodMemory(
  String id,
  String place, {
  String precision = 'Day',
  String from = '2026-03-01',
  String? to,
  String? time,
  bool mine = true,
}) => PeriodMemory(
  id: id,
  placeName: place,
  latitude: 50,
  longitude: 19,
  datePrecision: precision,
  dateFrom: from,
  dateTo: to ?? from,
  time: time,
  description: 'Opis $place',
  isMine: mine,
  photos: 0,
);
