import 'package:memorio/core/result.dart';
import 'package:memorio/features/plans/plans.dart';

/// Plans from memory; tests set the next answers.
class FakePlansRepository implements PlansRepository {
  PlanState state = planState();
  List<TrimMaterial> material = [];
  PlanFailure? loadFailure;
  PlanFailure? actionFailure;

  /// Every action, as "name:arguments", in order.
  final actions = <String>[];

  @override
  Future<Result<PlanState, PlanFailure>> load(String? albumId) async {
    final failure = loadFailure;
    return failure == null ? Ok(state) : Err(failure);
  }

  @override
  Future<Result<PlanState, PlanFailure>> refresh(String? albumId) async {
    actions.add('refresh:$albumId');
    final failure = actionFailure;
    return failure == null ? Ok(state) : Err(failure);
  }

  @override
  Future<Result<List<TrimMaterial>, PlanFailure>> loadMaterial(String? albumId) async {
    final failure = loadFailure;
    return failure == null ? Ok(material) : Err(failure);
  }

  @override
  Future<Result<void, PlanFailure>> keep(String? albumId, List<String> memoryIds) async {
    actions.add('keep:$albumId:${memoryIds.join(',')}');
    final failure = actionFailure;
    return failure == null ? const Ok(null) : Err(failure);
  }
}

const gigabyte = 1024 * 1024 * 1024;

PlanState planState({
  PlanMode mode = PlanMode.free,
  DateTime? entitledUntil,
  DateTime? readOnlySince,
  DateTime? trimAt,
  bool isChoiceOpen = false,
  bool isHeld = false,
  List<String> kept = const [],
}) => PlanState(
  mode: mode,
  entitledUntil: entitledUntil,
  readOnlySince: readOnlySince,
  trimAt: trimAt,
  isChoiceOpen: isChoiceOpen,
  isHeld: isHeld,
  keptMemoryIds: kept,
  usedBytes: gigabyte,
  limitBytes: 2 * gigabyte,
  freeLimitBytes: 2 * gigabyte,
  aiUsed: 1,
  aiLimit: 20,
);

const oldest = TrimMaterial(memoryId: 'm1', placeName: 'Giewont', dateFrom: '1990-07-01', sizeBytes: gigabyte);
const middle = TrimMaterial(memoryId: 'm2', placeName: 'Sopot', dateFrom: '2000-07-01', sizeBytes: gigabyte);
const newest = TrimMaterial(memoryId: 'm3', placeName: 'Kraków', dateFrom: '2020-07-01', sizeBytes: gigabyte);
