import 'dart:async';

import 'package:memorio/core/result.dart';
import 'package:memorio/features/ai_summaries/ai_summaries.dart';

PeriodSummary periodSummary({
  String id = 'summary-1',
  String accountId = 'account-1',
  SummaryState state = SummaryState.draft,
  List<String> sentences = const ['W lipcu byliście w górach.', 'Potem nad morzem.'],
  int usedMemories = 3,
  int leftOutPeople = 0,
  int droppedSentences = 0,
  bool addedToAlbum = false,
}) => PeriodSummary(
  id: id,
  accountId: accountId,
  albumId: 'album-1',
  from: '2026-07-01',
  to: '2026-07-31',
  state: state,
  sentences: sentences,
  usedMemories: usedMemories,
  leftOutMemories: leftOutPeople * 2,
  leftOutPeople: leftOutPeople,
  droppedSentences: droppedSentences,
  addedToAlbum: addedToAlbum,
);

/// Summaries from memory; tests set the next answers.
class FakeSummariesRepository implements SummariesRepository {
  bool given = true;
  PeriodSummary requested = periodSummary();
  List<PeriodSummary> album = [];
  SummaryFailure? loadFailure;
  SummaryFailure? actionFailure;

  /// While set, a request waits for it: the AI is still writing.
  Completer<void>? writing;

  /// Every action, as "name:arguments", in order.
  final actions = <String>[];

  Result<T, SummaryFailure> _answer<T>(T value) {
    final failure = actionFailure;
    return failure == null ? Ok(value) : Err(failure);
  }

  @override
  Future<Result<bool, SummaryFailure>> consent(String albumId) async {
    final failure = loadFailure;
    return failure == null ? Ok(given) : Err(failure);
  }

  @override
  Future<Result<void, SummaryFailure>> setConsent(String albumId, {required bool given}) async {
    actions.add('consent:$albumId:$given');
    final answer = _answer<void>(null);
    if (answer is Ok) {
      this.given = given;
    }
    return answer;
  }

  @override
  Future<Result<PeriodSummary, SummaryFailure>> request(String id, String albumId, String from, String to) async {
    actions.add('request:$id:$albumId');
    await writing?.future;
    return _answer(requested);
  }

  @override
  Future<Result<PeriodSummary, SummaryFailure>> save(String id) async {
    actions.add('save:$id');
    return _answer(periodSummary(id: id, state: SummaryState.saved));
  }

  @override
  Future<Result<PeriodSummary, SummaryFailure>> addToAlbum(String id) async {
    actions.add('add:$id');
    final added = periodSummary(id: id, state: SummaryState.saved, addedToAlbum: true);
    final answer = _answer(added);
    if (answer is Ok) {
      album = [added, ...album];
    }
    return answer;
  }

  @override
  Future<Result<List<PeriodSummary>, SummaryFailure>> inAlbum(String albumId) async {
    final failure = loadFailure;
    return failure == null ? Ok(album) : Err(failure);
  }
}
