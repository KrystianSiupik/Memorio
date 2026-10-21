# MEM-E7-POWIADOMIENIA-04: Polityka P26: Zawsze gdy przycięcie wstrzymane, wtedy ponawiaj ostrzeżenie co miesiąc aż do skutecznego doręczenia

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E7](epiki/E7-plany-limity-i-retencja-24-miesiace.md) Plany, limity i retencja 24 miesiące |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, powiadomienia, MEM-E7-POWIADOMIENIA-04 |
| Moduł | powiadomienia |
| Warstwa | aplikacja (handler zdarzenia) |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-POWIADOMIENIA-01](MEM-E0-POWIADOMIENIA-01-szkielet-modulu-powiadomienia-struktura-publiczny-.md); [MEM-E7-POWIADOMIENIA-01](MEM-E7-POWIADOMIENIA-01-wyslij-ostrzezenie-przed-przycieciem-c92.md); [MEM-E7-PLANY-17](MEM-E7-PLANY-17-zdarzenie-integracyjne-przyciecie-wstrzymane-e100-.md) |
| Blokuje | - |
| Kod roboczy | MEM-E7-POWIADOMIENIA-04 |
| Źródło | ES:P26 — 2026-10-16, HS-010 |

## Cel

Polityka P26: Zawsze gdy przycięcie wstrzymane, wtedy ponawiaj ostrzeżenie co miesiąc aż do skutecznego doręczenia — część epiku E7 „Plany, limity i retencja 24 miesiące”.

## Kontekst

- Moduł **powiadomienia** — kluczowe pytanie: Czy wiadomość dotarła i do kogo nie wolno pisać?
- Czego moduł nie wie (nie wolno tego wymagać): sens biznesowy wiadomości poza kategorią
- Projekt modułu: model procesów modułu powiadomienia
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Polityka P26: Zawsze gdy przycięcie wstrzymane, wtedy ponawiaj ostrzeżenie co miesiąc aż do skutecznego doręczenia (źródło: ES:P26 — 2026-10-16, HS-010)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Given zdarzenie E100, when handler je przetwarza, then wywołana jest komenda C92. *Sprawdzenie: test integracyjny handlera*
- [ ] Podwójne dostarczenie zdarzenia nie daje drugiego skutku (inbox, ADR 0006). *Sprawdzenie: test idempotentności*
- [ ] Zdarzenie starsze niż ostatnio zastosowane dla tej samej pary (zasób, osoba) jest pomijane (D-013). *Sprawdzenie: test jednostkowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
