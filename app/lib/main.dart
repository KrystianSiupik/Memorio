import 'package:flutter/widgets.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:package_info_plus/package_info_plus.dart';

import 'app/memorio_app.dart';
import 'core/api/api.dart';

Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();
  final packageInfo = await PackageInfo.fromPlatform();

  runApp(
    ProviderScope(overrides: [appVersionProvider.overrideWithValue(packageInfo.version)], child: const MemorioApp()),
  );
}
