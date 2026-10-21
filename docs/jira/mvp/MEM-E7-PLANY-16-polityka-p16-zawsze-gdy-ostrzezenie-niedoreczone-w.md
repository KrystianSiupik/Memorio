# MEM-E7-PLANY-16: Polityka P16: Zawsze gdy ostrzeżenie niedoręczone, wtedy wstrzymaj przycięcie

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E7](epiki/E7-plany-limity-i-retencja-24-miesiace.md) Plany, limity i retencja 24 miesiące |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, plany, MEM-E7-PLANY-16 |
| Moduł | plany |
| Warstwa | aplikacja (handler zdarzenia) |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-PLANY-01](MEM-E0-PLANY-01-szkielet-modulu-plany-struktura-publiczny-kontrakt.md); [MEM-E7-PLANY-04](MEM-E7-PLANY-04-wstrzymaj-przyciecie-archiwum-c93.md); [MEM-E7-POWIADOMIENIA-03](MEM-E7-POWIADOMIENIA-03-zdarzenie-integracyjne-ostrzezenie-niedoreczone-e9.md) |
| Blokuje | - |
| Kod roboczy | MEM-E7-PLANY-16 |
| Źródło | ES:P16 — S2:C10 „NIEODWRACALNA operacja jest WSTRZYMANA” |

## Cel

Polityka P16: Zawsze gdy ostrzeżenie niedoręczone, wtedy wstrzymaj przycięcie — część epiku E7 „Plany, limity i retencja 24 miesiące”.

## Kontekst

- Moduł **plany** — kluczowe pytanie: W jakim stanie jest teraz plan tego konta lub albumu i co z tego wynika?
- Czego moduł nie wie (nie wolno tego wymagać): co jest w archiwum; kto co wniósł; zgony, następcy, odmowy przejęcia (zna tylko „album bez właściciela”)
- Projekt modułu: model procesów modułu plany
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Polityka P16: Zawsze gdy ostrzeżenie niedoręczone, wtedy wstrzymaj przycięcie (źródło: ES:P16 — S2:C10 „NIEODWRACALNA operacja jest WSTRZYMANA”)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Given zdarzenie E99, when handler je przetwarza, then wywołana jest komenda C93. *Sprawdzenie: test integracyjny handlera*
- [ ] Podwójne dostarczenie zdarzenia nie daje drugiego skutku (inbox, ADR 0006). *Sprawdzenie: test idempotentności*
- [ ] Zdarzenie starsze niż ostatnio zastosowane dla tej samej pary (zasób, osoba) jest pomijane (D-013). *Sprawdzenie: test jednostkowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
