# MEM-E2-MIEJSCA-01: Wybierz miejsce z podpowiedzi (C10)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E2](epiki/E2-wspomnienia-na-prywatnej-mapie-tworzenie-kosz-usuw.md) Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, miejsca, MEM-E2-MIEJSCA-01 |
| Moduł | miejsca |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-MIEJSCA-01](MEM-E0-MIEJSCA-01-szkielet-modulu-miejsca-struktura-publiczny-kontra.md); [MEM-E2-MIEJSCA-03](MEM-E2-MIEJSCA-03-zapytanie-podpowiedzi-miejsc-openstreetmap-miejsca.md) |
| Blokuje | [MEM-E2-MIEJSCA-04](MEM-E2-MIEJSCA-04-read-port-miejsca-dla-archiwum-odwolanie-do-miejsc.md); [MEM-E2-PLAT-02](MEM-E2-PLAT-02-ekran-s04-nowe-wspomnienie-kreator-miejsce-tresc.md) |
| Kod roboczy | MEM-E2-MIEJSCA-01 |
| Źródło | ES:C10 — S1:§4 „wyszukiwać je w bazie lokalizacji”; S2:C4 po zmianie (L17) |

## Cel

Wybierz miejsce z podpowiedzi (C10) — część epiku E2 „Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie”.

## Kontekst

- Moduł **miejsca** — kluczowe pytanie: Jakie to miejsce na świecie i jak autor je nazywa?
- Czego moduł nie wie (nie wolno tego wymagać): czyje wspomnienia wiszą pod miejscem i co zawierają
- Projekt modułu: model procesów modułu miejsca
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Wybierz miejsce z podpowiedzi (C10) (źródło: ES:C10 — S1:§4 „wyszukiwać je w bazie lokalizacji”; S2:C4 po zmianie (L17))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E2-MIEJSCA-04, MEM-E2-PLAT-02)

## Kryteria akceptacji

- [ ] Given aktor widzi: Podpowiedzi miejsc (OpenStreetMap + miejsca własne), when Użytkownik (autor wspomnień) wykonuje „Wybierz miejsce z podpowiedzi”, then powstaje „Miejsce wybrane” (E10). *Sprawdzenie: test jednostkowy handlera*
- [ ] Niepoprawne dane wejściowe odrzucone walidacją z czytelnym komunikatem, bez zapisu. *Sprawdzenie: test jednostkowy schematu wejścia*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
