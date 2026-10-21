# MEM-E7-LIMITY-02: Polityka P27: Zawsze gdy plan aktywny albo przełączony w tryb tylko do odczytu, wtedy ustaw limit puli

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E7](epiki/E7-plany-limity-i-retencja-24-miesiace.md) Plany, limity i retencja 24 miesiące |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, limity, MEM-E7-LIMITY-02 |
| Moduł | limity |
| Warstwa | aplikacja (handler zdarzenia) |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-LIMITY-01](MEM-E0-LIMITY-01-szkielet-modulu-limity-struktura-publiczny-kontrak.md); [MEM-E7-LIMITY-01](MEM-E7-LIMITY-01-ustaw-limit-puli-c152.md); [MEM-E7-PLANY-20](MEM-E7-PLANY-20-zdarzenie-integracyjne-plan-aktywny-e94-w-kontrakc.md); [MEM-E7-PLANY-21](MEM-E7-PLANY-21-zdarzenie-integracyjne-album-lub-konto-w-trybie-ty.md) |
| Blokuje | - |
| Kod roboczy | MEM-E7-LIMITY-02 |
| Źródło | ES:P27 — 2026-10-16, kontrakty jako zdarzenia |

## Cel

Polityka P27: Zawsze gdy plan aktywny albo przełączony w tryb tylko do odczytu, wtedy ustaw limit puli — część epiku E7 „Plany, limity i retencja 24 miesiące”.

## Kontekst

- Moduł **limity** — kluczowe pytanie: Czy ta operacja zmieści się w puli i ile zostało?
- Czego moduł nie wie (nie wolno tego wymagać): skąd limit; kto płaci; czego dotyczy treść
- Projekt modułu: model procesów modułu limity
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Polityka P27: Zawsze gdy plan aktywny albo przełączony w tryb tylko do odczytu, wtedy ustaw limit puli (źródło: ES:P27 — 2026-10-16, kontrakty jako zdarzenia)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Given zdarzenie E94, E95, when handler je przetwarza, then wywołana jest komenda C152. *Sprawdzenie: test integracyjny handlera*
- [ ] Podwójne dostarczenie zdarzenia nie daje drugiego skutku (inbox, ADR 0006). *Sprawdzenie: test idempotentności*
- [ ] Zdarzenie starsze niż ostatnio zastosowane dla tej samej pary (zasób, osoba) jest pomijane (D-013). *Sprawdzenie: test jednostkowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
