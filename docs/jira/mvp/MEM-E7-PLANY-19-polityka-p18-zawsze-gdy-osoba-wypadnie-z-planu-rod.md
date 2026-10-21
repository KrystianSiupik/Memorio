# MEM-E7-PLANY-19: Polityka P18: Zawsze gdy osoba wypadnie z planu rodzinnego, wtedy przełącz jej konto w tryb „nic nowego ponad limit”

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E7](epiki/E7-plany-limity-i-retencja-24-miesiace.md) Plany, limity i retencja 24 miesiące |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, plany, MEM-E7-PLANY-19 |
| Moduł | plany |
| Warstwa | aplikacja (handler zdarzenia) |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P2 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-PLANY-01](MEM-E0-PLANY-01-szkielet-modulu-plany-struktura-publiczny-kontrakt.md); [MEM-E7-PLANY-10](MEM-E7-PLANY-10-przelacz-konto-w-tryb-ponad-limitem-c100.md); [MEM-E7-PLANY-09](MEM-E7-PLANY-09-usun-osobe-z-planu-rodzinnego-c99.md) |
| Blokuje | - |
| Kod roboczy | MEM-E7-PLANY-19 |
| Źródło | ES:P18 — S2:C7 „jak w B1” |

## Cel

Polityka P18: Zawsze gdy osoba wypadnie z planu rodzinnego, wtedy przełącz jej konto w tryb „nic nowego ponad limit” — część epiku E7 „Plany, limity i retencja 24 miesiące”.

## Kontekst

- Moduł **plany** — kluczowe pytanie: W jakim stanie jest teraz plan tego konta lub albumu i co z tego wynika?
- Czego moduł nie wie (nie wolno tego wymagać): co jest w archiwum; kto co wniósł; zgony, następcy, odmowy przejęcia (zna tylko „album bez właściciela”)
- Projekt modułu: model procesów modułu plany
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Polityka P18: Zawsze gdy osoba wypadnie z planu rodzinnego, wtedy przełącz jej konto w tryb „nic nowego ponad limit” (źródło: ES:P18 — S2:C7 „jak w B1”)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Given zdarzenie E107, when handler je przetwarza, then wywołana jest komenda C100. *Sprawdzenie: test integracyjny handlera*
- [ ] Podwójne dostarczenie zdarzenia nie daje drugiego skutku (inbox, ADR 0006). *Sprawdzenie: test idempotentności*
- [ ] Zdarzenie starsze niż ostatnio zastosowane dla tej samej pary (zasób, osoba) jest pomijane (D-013). *Sprawdzenie: test jednostkowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
