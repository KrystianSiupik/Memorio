# Features

One folder per feature (map, album, account, ...), created with the feature's first ticket.
Layers are nested inside the feature (ADR 0019); create only the layers a feature needs.

```
features/<feature>/
  <feature>.dart   public surface: the only file other features and app/ may import
  domain/          entities, value objects, repository interfaces, sealed failures; pure Dart
  data/            repository implementations: API client (OpenAPI, MEM-E0-PLAT-12), Drift (MEM-E0-PLAT-11)
  presentation/    pages, widgets, Riverpod notifiers and providers
```

Dependencies: presentation → domain ← data. Presentation never imports data, so it depends on
domain interfaces only:

```dart
// presentation/album_providers.dart
final albumRepositoryProvider = Provider<AlbumRepository>(
  (ref) => throw UnimplementedError('Overridden in app/ (composition root)'),
);

// album.dart (public surface)
export 'presentation/album_page.dart';
export 'presentation/album_providers.dart' show albumRepositoryProvider;
export 'data/http_album_repository.dart' show HttpAlbumRepository;

// app/: ProviderScope(overrides: [albumRepositoryProvider.overrideWith((ref) => HttpAlbumRepository(...))])
```

Tests override the same provider with a fake. Expected failures are returned as
`Result<T, Failure>` (`core/result.dart`) with a sealed failure type per feature.

The rules are checked by `test/architecture/import_rules_test.dart`.

Offline (ADR 0016): an operation that must work without network (creating a memory, adding photos)
is enqueued with `offlineQueueProvider` (`core/offline`); its id is the idempotency key the sender
passes to the API. The feature registers an `OperationSender` for its kind in `app/composition.dart`.
