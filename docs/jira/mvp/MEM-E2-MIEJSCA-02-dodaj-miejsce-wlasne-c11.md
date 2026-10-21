# MEM-E2-MIEJSCA-02: Dodaj miejsce własne (C11)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E2](epiki/E2-wspomnienia-na-prywatnej-mapie-tworzenie-kosz-usuw.md) Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, miejsca, MEM-E2-MIEJSCA-02 |
| Moduł | miejsca |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-MIEJSCA-01](MEM-E0-MIEJSCA-01-szkielet-modulu-miejsca-struktura-publiczny-kontra.md) |
| Blokuje | [MEM-E2-MIEJSCA-04](MEM-E2-MIEJSCA-04-read-port-miejsca-dla-archiwum-odwolanie-do-miejsc.md); [MEM-E2-PLAT-02](MEM-E2-PLAT-02-ekran-s04-nowe-wspomnienie-kreator-miejsce-tresc.md) |
| Kod roboczy | MEM-E2-MIEJSCA-02 |
| Źródło | ES:C11 — S1:§4 „samodzielnie dodawać miejsca”; S2:C4 MIEJSCE WŁASNE |

## Cel

Dodaj miejsce własne (C11) — część epiku E2 „Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie”.

## Kontekst

- Moduł **miejsca** — kluczowe pytanie: Jakie to miejsce na świecie i jak autor je nazywa?
- Czego moduł nie wie (nie wolno tego wymagać): czyje wspomnienia wiszą pod miejscem i co zawierają
- Projekt modułu: model procesów modułu miejsca
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Dodaj miejsce własne (C11) (źródło: ES:C11 — S1:§4 „samodzielnie dodawać miejsca”; S2:C4 MIEJSCE WŁASNE)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E2-MIEJSCA-04, MEM-E2-PLAT-02)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when Użytkownik (autor wspomnień) wykonuje „Dodaj miejsce własne”, then powstaje „Miejsce własne dodane” (E11). *Sprawdzenie: test jednostkowy handlera*
- [ ] Niepoprawne dane wejściowe odrzucone walidacją z czytelnym komunikatem, bez zapisu. *Sprawdzenie: test jednostkowy schematu wejścia*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
