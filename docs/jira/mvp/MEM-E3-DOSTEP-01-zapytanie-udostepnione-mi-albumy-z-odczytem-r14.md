# MEM-E3-DOSTEP-01: Zapytanie: Udostępnione mi (albumy z odczytem) (R14)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E3](epiki/E3-album-wspoldzielony-sklad-wklady-zgody.md) Album współdzielony: skład, wkłady, zgody |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, dostep, MEM-E3-DOSTEP-01 |
| Moduł | dostep |
| Warstwa | infrastruktura (query handler) |
| Rodzaj | zwykły |
| Klasa problemu | T&P |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-DOSTEP-01](MEM-E0-DOSTEP-01-szkielet-modulu-dostep-struktura-publiczny-kontrak.md) |
| Blokuje | [MEM-E3-PLAT-01](MEM-E3-PLAT-01-ekran-s09-albumy-lista-i-nowy-album.md); [MEM-E4-PLAT-02](MEM-E4-PLAT-02-ekran-s16-udostepnione-mi.md) |
| Kod roboczy | MEM-E3-DOSTEP-01 |
| Źródło | ES:R14 — S2:B2 „Osoba widzi album w »Udostępnione mi«”; dopisany przy modelowaniu ekranów 2026-10-17 (D-042) |

## Cel

Zapytanie: Udostępnione mi (albumy z odczytem) (R14) — część epiku E3 „Album współdzielony: skład, wkłady, zgody”.

## Kontekst

- Moduł **dostep** — kluczowe pytanie: Czy ta osoba może teraz zobaczyć tę treść?
- Czego moduł nie wie (nie wolno tego wymagać): dlaczego ktoś ma lub stracił dostęp; zawartość treści
- Projekt modułu: model procesów modułu dostep
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zapytanie: Udostępnione mi (albumy z odczytem) (R14) (źródło: ES:R14 — S2:B2 „Osoba widzi album w »Udostępnione mi«”; dopisany przy modelowaniu ekranów 2026-10-17 (D-042))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E3-PLAT-01, MEM-E4-PLAT-02)

## Kryteria akceptacji

- [ ] Given dane źródłowe, when zapytanie „Udostępnione mi (albumy z odczytem)”, then wynik zawiera tylko treści dostępne dla pytającego (filtr Dostępu, ADR 0011). *Sprawdzenie: test integracyjny macierzy użytkownik × zasób*
- [ ] Projekcja nie ma logiki biznesowej i daje się odbudować z danych źródłowych. *Sprawdzenie: review + test przebudowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
