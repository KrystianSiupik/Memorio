# MEM-E13-PLAT-10: Podgląd po najechaniu: zdjęcie w tle, biały tekst na szarym pasku

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E13](epiki/E13-wyglad-i-ustawienia.md) Wygląd i ustawienia |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, Web, MEM-E13-PLAT-10 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | Transformacja i prezentacja |
| Priorytet | P1 |
| Estymacja | 1–2 h |
| Pewność | średnia |
| Status | zrobiony |
| Zależy od | MEM-E12-PLAT-01 |
| Blokuje | - |
| Kod roboczy | MEM-E13-PLAT-10 |
| Źródło | prośba PO z 2026-11-08 |

## Cel

Podgląd po najechaniu na punkt pokazuje zdjęcie na całej karcie, a opis czyta się łatwo: biały tekst na szarym pasku.

## Zakres

- Pierwsze zdjęcie wypełnia kartę (min. 200 px wysokości), szary półprzezroczysty pasek u dołu z miejscem, datą, początkiem opisu i liczbą zdjęć, biały tekst
- Bez zdjęcia karta jest szara z białym tekstem

## Kryteria akceptacji

- [ ] Ze zdjęciem: zdjęcie wypełnia kartę, pasek leży u jej dołu, tekst jest biały. *Sprawdzenie: test widżetu*
- [ ] Dotychczasowe testy podglądu przechodzą. *Sprawdzenie: flutter test*
