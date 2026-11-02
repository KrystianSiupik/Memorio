import 'package:flutter/widgets.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:package_info_plus/package_info_plus.dart';

import 'app/composition.dart';
import 'app/memorio_app.dart';
import 'features/map/map.dart';

Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();
  useSelfHostedMapLibre();
  final packageInfo = await PackageInfo.fromPlatform();

  runApp(
    ProviderScope(
      overrides: appOverrides(appVersion: packageInfo.version),
      child: const MemorioApp(),
    ),
  );
}
