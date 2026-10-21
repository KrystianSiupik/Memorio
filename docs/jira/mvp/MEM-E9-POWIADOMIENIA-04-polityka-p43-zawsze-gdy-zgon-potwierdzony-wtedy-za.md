# MEM-E9-POWIADOMIENIA-04: Polityka P43: Zawsze gdy zgon potwierdzony, wtedy zaplanuj przypomnienia dla następcy (7. i 21. dzień)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E9](epiki/E9-sukcesja-i-przejecie-albumu-nastepca-albo-inny-ucz.md) Sukcesja i przejęcie albumu (następca albo inny uczestnik) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, powiadomienia, MEM-E9-POWIADOMIENIA-04 |
| Moduł | powiadomienia |
| Warstwa | aplikacja (handler zdarzenia) |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-POWIADOMIENIA-01](MEM-E0-POWIADOMIENIA-01-szkielet-modulu-powiadomienia-struktura-publiczny-.md); [MEM-E9-POWIADOMIENIA-02](MEM-E9-POWIADOMIENIA-02-przypomnij-nastepcy-o-decyzji-7-i-21-dzien-c167.md); [MEM-E9-SUKCESJA-13](MEM-E9-SUKCESJA-13-zdarzenie-integracyjne-zgon-potwierdzony-e121-w-ko.md) |
| Blokuje | - |
| Kod roboczy | MEM-E9-POWIADOMIENIA-04 |
| Źródło | ES:P43 — przegląd decyzji 2026-10-19 (HS-021) |

## Cel

Polityka P43: Zawsze gdy zgon potwierdzony, wtedy zaplanuj przypomnienia dla następcy (7. i 21. dzień) — część epiku E9 „Sukcesja i przejęcie albumu (następca albo inny uczestnik)”.

## Kontekst

- Moduł **powiadomienia** — kluczowe pytanie: Czy wiadomość dotarła i do kogo nie wolno pisać?
- Czego moduł nie wie (nie wolno tego wymagać): sens biznesowy wiadomości poza kategorią
- Projekt modułu: model procesów modułu powiadomienia
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Polityka P43: Zawsze gdy zgon potwierdzony, wtedy zaplanuj przypomnienia dla następcy (7. i 21. dzień) (źródło: ES:P43 — przegląd decyzji 2026-10-19 (HS-021))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Given zdarzenie E121, when handler je przetwarza, then wywołana jest komenda C167. *Sprawdzenie: test integracyjny handlera*
- [ ] Podwójne dostarczenie zdarzenia nie daje drugiego skutku (inbox, ADR 0006). *Sprawdzenie: test idempotentności*
- [ ] Zdarzenie starsze niż ostatnio zastosowane dla tej samej pary (zasób, osoba) jest pomijane (D-013). *Sprawdzenie: test jednostkowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
