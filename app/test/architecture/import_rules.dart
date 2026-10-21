/// Dependency rules of the app (ADR 0019), checked on `import` and `export` directives.
///
/// Layout (paths relative to the package root):
/// - `lib/main.dart` and `lib/app/`: composition root, may use every feature through its public file;
/// - `lib/core/`: shared code used by 2+ features; never imports features or `app/`;
/// - `lib/features/<feature>/<feature>.dart`: the feature's public surface;
/// - `lib/features/<feature>/{domain,data,presentation}/`: its layers, with
///   presentation → domain ← data and a domain written in pure Dart.
///
/// No maintained free package expresses these rules (layers inside features plus public files),
/// so this small checker is tested against fixtures in `import_rules_test.dart`.
library;

const featureUsesAnotherFeatureInside = 'A feature uses another feature only through its public file';
const appUsesFeatureInside = 'app/ uses a feature only through its public file';
const coreImportsFeaturesOrApp = 'core/ does not import features or app/';
const featureImportsApp = 'A feature does not import app/';
const domainImportsOtherLayers = 'Domain does not import data or presentation';
const presentationImportsData = 'Presentation does not import data';
const dataImportsPresentation = 'Data does not import presentation';
const domainImportsFramework = 'Domain is pure Dart (no Flutter, Riverpod, router, database or HTTP)';

/// Packages the domain must not know.
const _frameworksForbiddenInDomain = [
  'dart:ui',
  'package:flutter/',
  'package:flutter_riverpod/',
  'package:riverpod/',
  'package:go_router/',
  'package:drift/',
  'package:dio/',
  'package:http/',
];

const _packagePrefix = 'package:memorio/';

final _directive = RegExp(r'''^\s*(?:import|export)\s+['"]([^'"]+)['"]''', multiLine: true);

class Violation {
  const Violation(this.file, this.target, this.rule);

  final String file;
  final String target;
  final String rule;

  @override
  String toString() => '$file -> $target: $rule';
}

/// Checks every source; [sources] maps a path like `lib/features/album/domain/album.dart` to its content.
List<Violation> checkImports(Map<String, String> sources) {
  final violations = <Violation>[];
  for (final MapEntry(key: file, value: content) in sources.entries) {
    final from = _Location.of(file);
    for (final match in _directive.allMatches(content)) {
      final uri = match.group(1)!;
      final target = _resolve(file, uri);
      final rule = target == null ? _checkExternal(from, uri) : _checkInternal(from, _Location.of(target));
      if (rule != null) {
        violations.add(Violation(file, target ?? uri, rule));
      }
    }
  }
  return violations;
}

/// The `lib/...` path of an import inside this package, or null for SDK and other packages.
String? _resolve(String file, String uri) {
  if (uri.startsWith(_packagePrefix)) {
    return 'lib/${uri.substring(_packagePrefix.length)}';
  }
  if (uri.startsWith('package:') || uri.startsWith('dart:')) {
    return null;
  }
  return Uri.parse(file).resolve(uri).path;
}

String? _checkExternal(_Location from, String uri) {
  if (from.layer == 'domain' && _frameworksForbiddenInDomain.any(uri.startsWith)) {
    return domainImportsFramework;
  }
  return null;
}

String? _checkInternal(_Location from, _Location to) {
  if (from.area == _Area.core && (to.area == _Area.feature || to.area == _Area.app)) {
    return coreImportsFeaturesOrApp;
  }
  if (from.area == _Area.feature && to.area == _Area.app) {
    return featureImportsApp;
  }
  if (to.area == _Area.feature && !to.isPublicFile) {
    if (from.area == _Area.app) {
      return appUsesFeatureInside;
    }
    if (from.area == _Area.feature && from.feature != to.feature) {
      return featureUsesAnotherFeatureInside;
    }
  }
  if (from.area == _Area.feature && from.feature == to.feature) {
    return switch ((from.layer, to.layer)) {
      ('domain', 'data' || 'presentation') => domainImportsOtherLayers,
      ('presentation', 'data') => presentationImportsData,
      ('data', 'presentation') => dataImportsPresentation,
      _ => null,
    };
  }
  return null;
}

enum _Area { app, core, feature, other }

class _Location {
  const _Location(this.area, {this.feature, this.layer, this.isPublicFile = false});

  factory _Location.of(String path) {
    final parts = path.split('/');
    if (parts.length < 2 || parts[0] != 'lib') {
      return const _Location(_Area.other);
    }
    if (parts.length == 2 && parts[1] == 'main.dart') {
      return const _Location(_Area.app);
    }
    switch (parts[1]) {
      case 'app':
        return const _Location(_Area.app);
      case 'core':
        return const _Location(_Area.core);
      case 'features' when parts.length >= 4:
        final feature = parts[2];
        return _Location(
          _Area.feature,
          feature: feature,
          layer: parts.length >= 5 ? parts[3] : null,
          isPublicFile: parts.length == 4 && parts[3] == '$feature.dart',
        );
      default:
        return const _Location(_Area.other);
    }
  }

  final _Area area;
  final String? feature;
  final String? layer;
  final bool isPublicFile;
}
