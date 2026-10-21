# Memorio app

Flutter client for iOS, Android and web (ADR 0003). Riverpod for state and dependency injection,
go_router for navigation, dio + a client generated from `../api/openapi.v1.json` (swagger_parser). Layout and dependency rules: `lib/features/README.md`.

```bash
flutter pub get
flutter analyze
flutter test                                         # includes the architecture test of imports
dart format --output=none --set-exit-if-changed lib test
flutter run --dart-define=API_BASE_URL=http://10.0.2.2:5080   # backend URL; -d chrome for web
../tools/generate-api-client.sh                      # after a change of api/openapi.v1.json
```
