# MEM-E0-PLAT-22: Odchudzenie pierwszej wizyty w przeglądarce: leniwe ładowanie ekranów

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E0](epiki/E0-szkielet-end-to-end-i-platforma-etap-1.md) Szkielet end-to-end i platforma (etap 1) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Platforma, Flutter, web, MEM-E0-PLAT-22 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | Transformacja i prezentacja |
| Priorytet | P2 |
| Estymacja | 4–8 h |
| Pewność | średnia |
| Status | do zrobienia |
| Zależy od | [MEM-E0-PLAT-05](MEM-E0-PLAT-05-spike-mapy-maplibre-pmtiles-w-flutterze-globus-10-.md) (benchmark mapy, SD3) |
| Blokuje | - |
| Kod roboczy | MEM-E0-PLAT-22 |
| Źródło | CI 2026-11-13: pierwsza wizyta 3,67 MB gzip przy limicie 3,5 MB; limit tymczasowo 4,0 MB (D-374) |

## Cel

Pierwsza wizyta w aplikacji webowej pobiera tylko to, czego potrzeba do mapy (S03) i logowania, a reszta ekranów dochodzi przy pierwszym wejściu na nie. Limit SD3 w benchmarku wraca do 3,5 MB albo niżej.

## Kontekst

- Benchmark mapy (`tools/map-benchmark/run.mjs`, D-192) mierzy, ile strona pobiera przed otwarciem mapy. Przy E2 było 3,49 MB przy limicie 3,5 MB, po E13 i E6 jest 3,67 MB.
- Wszystkie ekrany są dziś w jednym pliku `main.dart.js`, więc każdy nowy ekran powiększa pierwszą wizytę, choć użytkownik zaczyna od mapy.
- Dart na web dzieli kod przez importy odroczone (`import ... deferred as`, `loadLibrary()`), które `flutter build web` zamienia na osobne pliki ładowane na żądanie.

## Zakres

- Pomiar: które funkcje (`app/lib/features/*`) i pakiety ważą najwięcej w `main.dart.js` (np. `--dump-info` / `--analyze-size`).
- Importy odroczone dla ekranów, które nie są potrzebne na starcie, w pierwszej kolejności: podsumowania AI (`ai_summaries`), sukcesja (`succession`), moderacja (`moderation`), plany (`plans`), ustawienia (`settings`), „Prześledź” (`replay`).
- Trasa w routerze ładuje bibliotekę (`loadLibrary()`) przed pokazaniem ekranu, z widokiem ładowania i obsługą błędu sieci (ponów).
- Na telefonie (Android, iOS) bez zmian w zachowaniu: importy odroczone ładują się od razu.
- Limit `initialGzipMb` w `tools/map-benchmark/run.mjs` z powrotem na 3,5 MB (albo niżej, jeśli pomiar na to pozwala) i wpis w `docs/decisions.md`.

## Poza zakresem

- Zmiana renderera Fluttera (CanvasKit, skwasm) i ładowanie czcionek.
- Odchudzanie samej biblioteki mapy (`mapGzipMb`, mieści się w limicie).

## Kryteria akceptacji

- [ ] Pierwsza wizyta ≤ 3,5 MB gzip, a limit w benchmarku wrócił do 3,5 MB. *Sprawdzenie: CI — benchmark mapy*
- [ ] Każdy ekran ładowany leniwie otwiera się z widokiem ładowania, a bez sieci pokazuje błąd z ponowieniem. *Sprawdzenie: test widżetu*
- [ ] Na Androidzie i iOS ekrany działają jak wcześniej. *Sprawdzenie: test na urządzeniu*

## Ryzyka i pytania

- Ekran ładowany leniwie wymaga sieci przy pierwszym wejściu; tryb offline w przeglądarce może go nie otworzyć. Do sprawdzenia, czy service worker trzyma te pliki w pamięci podręcznej.
