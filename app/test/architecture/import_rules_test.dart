import 'dart:io';

import 'package:flutter_test/flutter_test.dart';

import 'import_rules.dart';

/// A correct layout: two features, the composition root and core.
const _validLayout = {
  'lib/main.dart': "import 'app/memorio_app.dart';",
  'lib/app/memorio_app.dart': '''
import 'package:flutter/material.dart';
import 'package:memorio/features/album/album.dart';
import '../features/account/account.dart';
''',
  'lib/core/result.dart': 'sealed class Result {}',
  'lib/features/album/album.dart': '''
export 'presentation/album_page.dart';
export 'data/http_album_repository.dart';
''',
  'lib/features/album/domain/album.dart': '''
import 'package:meta/meta.dart';
import 'package:memorio/core/result.dart';
import 'album_id.dart';
''',
  'lib/features/album/domain/album_id.dart': 'class AlbumId {}',
  'lib/features/album/data/http_album_repository.dart': '''
import 'package:dio/dio.dart';
import '../domain/album.dart';
''',
  'lib/features/album/presentation/album_page.dart': '''
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../domain/album.dart';
import '../../account/account.dart';
''',
  'lib/features/account/account.dart': "export 'presentation/account_page.dart';",
  'lib/features/account/presentation/account_page.dart': "import 'package:flutter/material.dart';",
};

void main() {
  test('a correct layout has no violations', () {
    expect(checkImports(_validLayout), isEmpty);
  });

  final violations = <String, (Map<String, String>, String)>{
    'feature imports another feature inside': (
      {'lib/features/album/presentation/page.dart': "import '../../account/domain/account.dart';"},
      featureUsesAnotherFeatureInside,
    ),
    'feature imports another feature inside by package uri': (
      {'lib/features/album/data/repo.dart': "import 'package:memorio/features/account/data/api.dart';"},
      featureUsesAnotherFeatureInside,
    ),
    'app imports a feature inside': (
      {'lib/app/router.dart': "import '../features/album/presentation/album_page.dart';"},
      appUsesFeatureInside,
    ),
    'core imports a feature': (
      {'lib/core/ui/button.dart': "import '../../features/album/album.dart';"},
      coreImportsFeaturesOrApp,
    ),
    'core imports app': (
      {'lib/core/ui/button.dart': "import 'package:memorio/app/router.dart';"},
      coreImportsFeaturesOrApp,
    ),
    'feature imports app': (
      {'lib/features/album/presentation/page.dart': "import 'package:memorio/app/router.dart';"},
      featureImportsApp,
    ),
    'domain imports data': (
      {'lib/features/album/domain/album.dart': "import '../data/http_album_repository.dart';"},
      domainImportsOtherLayers,
    ),
    'domain exports presentation': (
      {'lib/features/album/domain/album.dart': "export '../presentation/album_page.dart';"},
      domainImportsOtherLayers,
    ),
    'presentation imports data': (
      {'lib/features/album/presentation/page.dart': "import '../data/http_album_repository.dart';"},
      presentationImportsData,
    ),
    'data imports presentation': (
      {'lib/features/album/data/repo.dart': "import '../presentation/album_page.dart';"},
      dataImportsPresentation,
    ),
    'domain imports Flutter': (
      {'lib/features/album/domain/album.dart': "import 'package:flutter/foundation.dart';"},
      domainImportsFramework,
    ),
    'domain imports Riverpod': (
      {'lib/features/album/domain/album.dart': 'import "package:flutter_riverpod/flutter_riverpod.dart";'},
      domainImportsFramework,
    ),
    'domain imports dart:ui': ({'lib/features/album/domain/album.dart': "import 'dart:ui';"}, domainImportsFramework),
  };

  for (final MapEntry(key: name, value: (sources, rule)) in violations.entries) {
    test('$name is caught by its rule only', () {
      expect(checkImports(sources).map((v) => v.rule), [rule]);
    });
  }

  test('the app follows the rules', () {
    final lib = Directory('lib');
    final sources = {
      for (final file in lib.listSync(recursive: true).whereType<File>().where((f) => f.path.endsWith('.dart')))
        file.path.replaceAll(r'\', '/'): file.readAsStringSync(),
    };
    // Guard: an empty scan (wrong working directory) must not pass.
    expect(sources.keys, containsAll(['lib/main.dart', 'lib/app/router.dart']));

    expect(checkImports(sources), isEmpty);
  });
}
