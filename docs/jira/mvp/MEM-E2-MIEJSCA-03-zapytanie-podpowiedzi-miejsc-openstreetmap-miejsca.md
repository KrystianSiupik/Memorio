# MEM-E2-MIEJSCA-03: Zapytanie: Podpowiedzi miejsc (OpenStreetMap + miejsca własne) (R3)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E2](epiki/E2-wspomnienia-na-prywatnej-mapie-tworzenie-kosz-usuw.md) Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, miejsca, MEM-E2-MIEJSCA-03 |
| Moduł | miejsca |
| Warstwa | infrastruktura (query handler) |
| Rodzaj | zwykły |
| Klasa problemu | T&P |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-MIEJSCA-01](MEM-E0-MIEJSCA-01-szkielet-modulu-miejsca-struktura-publiczny-kontra.md) |
| Blokuje | [MEM-E2-MIEJSCA-01](MEM-E2-MIEJSCA-01-wybierz-miejsce-z-podpowiedzi-c10.md); [MEM-E2-PLAT-02](MEM-E2-PLAT-02-ekran-s04-nowe-wspomnienie-kreator-miejsce-tresc.md) |
| Kod roboczy | MEM-E2-MIEJSCA-03 |
| Źródło | ES:R3 — S2:C4 po zmianie (L17) |

## Cel

Zapytanie: Podpowiedzi miejsc (OpenStreetMap + miejsca własne) (R3) — część epiku E2 „Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie”.

## Kontekst

- Moduł **miejsca** — kluczowe pytanie: Jakie to miejsce na świecie i jak autor je nazywa?
- Czego moduł nie wie (nie wolno tego wymagać): czyje wspomnienia wiszą pod miejscem i co zawierają
- Projekt modułu: model procesów modułu miejsca
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zapytanie: Podpowiedzi miejsc (OpenStreetMap + miejsca własne) (R3) (źródło: ES:R3 — S2:C4 po zmianie (L17))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E2-MIEJSCA-01, MEM-E2-PLAT-02)

## Kryteria akceptacji

- [ ] Given dane źródłowe, when zapytanie „Podpowiedzi miejsc (OpenStreetMap + miejsca własne)”, then wynik zawiera tylko treści dostępne dla pytającego (filtr Dostępu, ADR 0011). *Sprawdzenie: test integracyjny macierzy użytkownik × zasób*
- [ ] Projekcja nie ma logiki biznesowej i daje się odbudować z danych źródłowych. *Sprawdzenie: review + test przebudowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
