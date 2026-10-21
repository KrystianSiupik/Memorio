# MEM-E7-PLANY-23: Polityka P36: Zawsze gdy album bez właściciela, wtedy przełącz plan albumu w tryb tylko do odczytu po końcu opłaconego okresu

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E7](epiki/E7-plany-limity-i-retencja-24-miesiace.md) Plany, limity i retencja 24 miesiące |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, plany, MEM-E7-PLANY-23 |
| Moduł | plany |
| Warstwa | aplikacja (handler zdarzenia) |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-PLANY-01](MEM-E0-PLANY-01-szkielet-modulu-plany-struktura-publiczny-kontrakt.md); [MEM-E7-PLANY-02](MEM-E7-PLANY-02-przelacz-w-tryb-tylko-do-odczytu-c91.md); [MEM-E9-ALBUM-07](MEM-E9-ALBUM-07-zdarzenie-integracyjne-album-bez-wlasciciela-e162-.md) |
| Blokuje | - |
| Kod roboczy | MEM-E7-PLANY-23 |
| Źródło | ES:P36 — S2:B3 „ten sam co w B1”; S5 sonda HS-017 2026-10-17 (D-001) |

## Cel

Polityka P36: Zawsze gdy album bez właściciela, wtedy przełącz plan albumu w tryb tylko do odczytu po końcu opłaconego okresu — część epiku E7 „Plany, limity i retencja 24 miesiące”.

## Kontekst

- Moduł **plany** — kluczowe pytanie: W jakim stanie jest teraz plan tego konta lub albumu i co z tego wynika?
- Czego moduł nie wie (nie wolno tego wymagać): co jest w archiwum; kto co wniósł; zgony, następcy, odmowy przejęcia (zna tylko „album bez właściciela”)
- Projekt modułu: model procesów modułu plany
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Polityka P36: Zawsze gdy album bez właściciela, wtedy przełącz plan albumu w tryb tylko do odczytu po końcu opłaconego okresu (źródło: ES:P36 — S2:B3 „ten sam co w B1”; S5 sonda HS-017 2026-10-17 (D-001))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Given zdarzenie E162, when handler je przetwarza, then wywołana jest komenda C91. *Sprawdzenie: test integracyjny handlera*
- [ ] Podwójne dostarczenie zdarzenia nie daje drugiego skutku (inbox, ADR 0006). *Sprawdzenie: test idempotentności*
- [ ] Zdarzenie starsze niż ostatnio zastosowane dla tej samej pary (zasób, osoba) jest pomijane (D-013). *Sprawdzenie: test jednostkowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
