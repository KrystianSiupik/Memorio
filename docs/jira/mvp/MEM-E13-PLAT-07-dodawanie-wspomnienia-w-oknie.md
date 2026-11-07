# MEM-E13-PLAT-07: Dodawanie wspomnienia w oknie z etapami

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E13](epiki/E13-wyglad-i-ustawienia.md) Wygląd i ustawienia |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, Web, MEM-E13-PLAT-07 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | Transformacja i prezentacja |
| Priorytet | P1 |
| Estymacja | 4–6 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | MEM-E12-PLAT-05 |
| Blokuje | - |
| Kod roboczy | MEM-E13-PLAT-07 |
| Źródło | prośba PO z 2026-11-07 po pokazie aplikacji |

## Cel

Nowe wspomnienie dodaje się w oknie (modalu) nad mapą, krok po kroku, z widocznymi etapami. Okno nie znika przypadkiem.

## Zakres

- Okno nad bieżącym ekranem; na szerokim ekranie wyśrodkowane i węższe, na telefonie na cały ekran
- U góry tytuł, pasek etapów (1 Miejsce i data, 2 Treść) i przycisk X
- Na dole „Anuluj” oraz „Wstecz” / „Dalej” / „Zapisz wspomnienie”
- Kliknięcie w tło ani klawisz Esc nie zamykają okna; zamykają je tylko X i „Anuluj”
- Adres `/memories/new` (także z miejscem z mapy) dalej działa i otwiera okno
- Zachowanie kroków bez zmian (walidacja, zdjęcia, AI, tryb offline)

## Poza zakresem

- pytanie „czy na pewno porzucić” przy zamykaniu z wpisaną treścią (osobny ticket, jeśli PO zechce)

## Kryteria akceptacji

- [ ] „+” na mapie otwiera okno z paskiem etapów; mapa zostaje pod spodem. *Sprawdzenie: test widżetu*
- [ ] Kliknięcie w tło i Esc nie zamykają okna; X i „Anuluj” zamykają. *Sprawdzenie: test widżetu*
- [ ] Pasek etapów pokazuje bieżący krok po „Dalej” i „Wstecz”. *Sprawdzenie: test widżetu*
- [ ] Dotychczasowe testy S04 przechodzą. *Sprawdzenie: flutter test*
