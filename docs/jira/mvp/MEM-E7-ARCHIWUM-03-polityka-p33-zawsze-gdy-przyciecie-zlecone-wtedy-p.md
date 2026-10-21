# MEM-E7-ARCHIWUM-03: Polityka P33: Zawsze gdy przycięcie zlecone, wtedy przytnij archiwum według wyboru właściciela albo od najstarszego

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E7](epiki/E7-plany-limity-i-retencja-24-miesiace.md) Plany, limity i retencja 24 miesiące |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, archiwum, MEM-E7-ARCHIWUM-03 |
| Moduł | archiwum |
| Warstwa | aplikacja (handler zdarzenia) |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-ARCHIWUM-01](MEM-E0-ARCHIWUM-01-szkielet-modulu-archiwum-struktura-publiczny-kontr.md); [MEM-E7-ARCHIWUM-01](MEM-E7-ARCHIWUM-01-przytnij-archiwum-do-limitu-nadmiar-do-kosza-na-30.md); [MEM-E7-PLANY-22](MEM-E7-PLANY-22-zdarzenie-integracyjne-przyciecie-archiwum-zlecone.md) |
| Blokuje | - |
| Kod roboczy | MEM-E7-ARCHIWUM-03 |
| Źródło | ES:P33 — 2026-10-16, kontrakty jako zdarzenia |

## Cel

Polityka P33: Zawsze gdy przycięcie zlecone, wtedy przytnij archiwum według wyboru właściciela albo od najstarszego — część epiku E7 „Plany, limity i retencja 24 miesiące”.

## Kontekst

- Moduł **archiwum** — kluczowe pytanie: Co ta osoba zapisała, gdzie i kiedy?
- Czego moduł nie wie (nie wolno tego wymagać): kto poza autorem widzi treść; w których albumach jest wspomnienie; plany i pieniądze; czy treść poszła do AI
- Projekt modułu: model procesów modułu archiwum
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Polityka P33: Zawsze gdy przycięcie zlecone, wtedy przytnij archiwum według wyboru właściciela albo od najstarszego (źródło: ES:P33 — 2026-10-16, kontrakty jako zdarzenia)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Given zdarzenie E103, when handler je przetwarza, then wywołana jest komenda C24. *Sprawdzenie: test integracyjny handlera*
- [ ] Podwójne dostarczenie zdarzenia nie daje drugiego skutku (inbox, ADR 0006). *Sprawdzenie: test idempotentności*
- [ ] Zdarzenie starsze niż ostatnio zastosowane dla tej samej pary (zasób, osoba) jest pomijane (D-013). *Sprawdzenie: test jednostkowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
