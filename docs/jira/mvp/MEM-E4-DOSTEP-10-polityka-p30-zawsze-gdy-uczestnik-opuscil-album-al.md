# MEM-E4-DOSTEP-10: Polityka P30: Zawsze gdy uczestnik opuścił album albo został usunięty, wtedy odbierz mu odczyt

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E4](epiki/E4-udostepnianie-linkiem-i-odbieranie-dostepu.md) Udostępnianie linkiem i odbieranie dostępu |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, dostep, MEM-E4-DOSTEP-10 |
| Moduł | dostep |
| Warstwa | aplikacja (handler zdarzenia) |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-DOSTEP-01](MEM-E0-DOSTEP-01-szkielet-modulu-dostep-struktura-publiczny-kontrak.md); [MEM-E4-DOSTEP-04](MEM-E4-DOSTEP-04-odbierz-dostep-osobie-c63.md); [MEM-E3-ALBUM-25](MEM-E3-ALBUM-25-zdarzenie-integracyjne-uczestnik-opuscil-album-mie.md); [MEM-E3-ALBUM-22](MEM-E3-ALBUM-22-zdarzenie-integracyjne-uczestnik-usuniety-z-albumu.md) |
| Blokuje | - |
| Kod roboczy | MEM-E4-DOSTEP-10 |
| Źródło | ES:P30 — 2026-10-16, kontrakty jako zdarzenia |

## Cel

Polityka P30: Zawsze gdy uczestnik opuścił album albo został usunięty, wtedy odbierz mu odczyt — część epiku E4 „Udostępnianie linkiem i odbieranie dostępu”.

## Kontekst

- Moduł **dostep** — kluczowe pytanie: Czy ta osoba może teraz zobaczyć tę treść?
- Czego moduł nie wie (nie wolno tego wymagać): dlaczego ktoś ma lub stracił dostęp; zawartość treści
- Projekt modułu: model procesów modułu dostep
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Polityka P30: Zawsze gdy uczestnik opuścił album albo został usunięty, wtedy odbierz mu odczyt (źródło: ES:P30 — 2026-10-16, kontrakty jako zdarzenia)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Given zdarzenie E56, E59, when handler je przetwarza, then wywołana jest komenda C63. *Sprawdzenie: test integracyjny handlera*
- [ ] Podwójne dostarczenie zdarzenia nie daje drugiego skutku (inbox, ADR 0006). *Sprawdzenie: test idempotentności*
- [ ] Zdarzenie starsze niż ostatnio zastosowane dla tej samej pary (zasób, osoba) jest pomijane (D-013). *Sprawdzenie: test jednostkowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
