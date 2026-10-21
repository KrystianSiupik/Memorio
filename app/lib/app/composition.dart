import 'package:flutter_riverpod/misc.dart';

import '../core/api/api.dart';
import '../features/account/account.dart';

/// Wires implementations to the interfaces features depend on. Tests build their own overrides.
List<Override> appOverrides({required String appVersion}) => [
  appVersionProvider.overrideWithValue(appVersion),
  sessionRepositoryProvider.overrideWithValue(const NoSessionRepository()),
];
