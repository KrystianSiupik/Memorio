# MEM-E8-DOSTEP-03: Polityka P32: Zawsze gdy treść ukryta decyzją moderacji, wtedy ukryj zakres w Dostępie

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E8](epiki/E8-zgloszenia-moderacja-i-nakazy-organow.md) Zgłoszenia, moderacja i nakazy organów |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, dostep, MEM-E8-DOSTEP-03 |
| Moduł | dostep |
| Warstwa | aplikacja (handler zdarzenia) |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-DOSTEP-01](MEM-E0-DOSTEP-01-szkielet-modulu-dostep-struktura-publiczny-kontrak.md); [MEM-E8-DOSTEP-01](MEM-E8-DOSTEP-01-ukryj-zakres-c67.md); [MEM-E8-MODERACJA-11](MEM-E8-MODERACJA-11-zdarzenie-integracyjne-tresc-ukryta-e113-w-kontrak.md) |
| Blokuje | - |
| Kod roboczy | MEM-E8-DOSTEP-03 |
| Źródło | ES:P32 — 2026-10-16; wyjątek: nakaz (C113) woła Dostęp poleceniem z wynikiem |

## Cel

Polityka P32: Zawsze gdy treść ukryta decyzją moderacji, wtedy ukryj zakres w Dostępie — część epiku E8 „Zgłoszenia, moderacja i nakazy organów”.

## Kontekst

- Moduł **dostep** — kluczowe pytanie: Czy ta osoba może teraz zobaczyć tę treść?
- Czego moduł nie wie (nie wolno tego wymagać): dlaczego ktoś ma lub stracił dostęp; zawartość treści
- Projekt modułu: model procesów modułu dostep
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Polityka P32: Zawsze gdy treść ukryta decyzją moderacji, wtedy ukryj zakres w Dostępie (źródło: ES:P32 — 2026-10-16; wyjątek: nakaz (C113) woła Dostęp poleceniem z wynikiem)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Given zdarzenie E113, when handler je przetwarza, then wywołana jest komenda C67. *Sprawdzenie: test integracyjny handlera*
- [ ] Podwójne dostarczenie zdarzenia nie daje drugiego skutku (inbox, ADR 0006). *Sprawdzenie: test idempotentności*
- [ ] Zdarzenie starsze niż ostatnio zastosowane dla tej samej pary (zasób, osoba) jest pomijane (D-013). *Sprawdzenie: test jednostkowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
