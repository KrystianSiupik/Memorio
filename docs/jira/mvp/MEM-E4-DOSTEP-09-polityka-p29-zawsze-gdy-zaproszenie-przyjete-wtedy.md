# MEM-E4-DOSTEP-09: Polityka P29: Zawsze gdy zaproszenie przyjęte, wtedy przyznaj odczyt albumu

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E4](epiki/E4-udostepnianie-linkiem-i-odbieranie-dostepu.md) Udostępnianie linkiem i odbieranie dostępu |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, dostep, MEM-E4-DOSTEP-09 |
| Moduł | dostep |
| Warstwa | aplikacja (handler zdarzenia) |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-DOSTEP-01](MEM-E0-DOSTEP-01-szkielet-modulu-dostep-struktura-publiczny-kontrak.md); [MEM-E4-DOSTEP-06](MEM-E4-DOSTEP-06-przyznaj-odczyt-zakresu-c65.md); [MEM-E3-ALBUM-24](MEM-E3-ALBUM-24-zdarzenie-integracyjne-zaproszenie-przyjete-e47-w-.md) |
| Blokuje | - |
| Kod roboczy | MEM-E4-DOSTEP-09 |
| Źródło | ES:P29 — 2026-10-16, kontrakty jako zdarzenia |

## Cel

Polityka P29: Zawsze gdy zaproszenie przyjęte, wtedy przyznaj odczyt albumu — część epiku E4 „Udostępnianie linkiem i odbieranie dostępu”.

## Kontekst

- Moduł **dostep** — kluczowe pytanie: Czy ta osoba może teraz zobaczyć tę treść?
- Czego moduł nie wie (nie wolno tego wymagać): dlaczego ktoś ma lub stracił dostęp; zawartość treści
- Projekt modułu: model procesów modułu dostep
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Polityka P29: Zawsze gdy zaproszenie przyjęte, wtedy przyznaj odczyt albumu (źródło: ES:P29 — 2026-10-16, kontrakty jako zdarzenia)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Given zdarzenie E47, when handler je przetwarza, then wywołana jest komenda C65. *Sprawdzenie: test integracyjny handlera*
- [ ] Podwójne dostarczenie zdarzenia nie daje drugiego skutku (inbox, ADR 0006). *Sprawdzenie: test idempotentności*
- [ ] Zdarzenie starsze niż ostatnio zastosowane dla tej samej pary (zasób, osoba) jest pomijane (D-013). *Sprawdzenie: test jednostkowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
