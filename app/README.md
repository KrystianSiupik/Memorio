# Memorio app

Flutter client for iOS, Android and web (ADR 0003). Riverpod for state and dependency injection,
go_router for navigation. Layout and dependency rules: `lib/features/README.md`.

```bash
flutter pub get
flutter analyze
flutter test                                         # includes the architecture test of imports
dart format --output=none --set-exit-if-changed lib test
flutter run                                          # -d chrome for web
```
