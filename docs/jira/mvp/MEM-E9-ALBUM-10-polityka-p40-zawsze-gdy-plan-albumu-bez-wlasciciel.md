# MEM-E9-ALBUM-10: Polityka P40: Zawsze gdy plan albumu bez właściciela opłacony przez uczestnika, wtedy przejmij album (jeśli to jego aktywna rezerwacja)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E9](epiki/E9-sukcesja-i-przejecie-albumu-nastepca-albo-inny-ucz.md) Sukcesja i przejęcie albumu (następca albo inny uczestnik) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, album, MEM-E9-ALBUM-10 |
| Moduł | album |
| Warstwa | aplikacja (handler zdarzenia) |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-ALBUM-01](MEM-E0-ALBUM-01-szkielet-modulu-album-struktura-publiczny-kontrakt.md); [MEM-E9-ALBUM-03](MEM-E9-ALBUM-03-przejmij-album-bez-wlasciciela-c156.md); [MEM-E7-PLANY-24](MEM-E7-PLANY-24-zdarzenie-integracyjne-plan-albumu-bez-wlasciciela.md) |
| Blokuje | - |
| Kod roboczy | MEM-E9-ALBUM-10 |
| Źródło | ES:P40 — przegląd decyzji 2026-10-19 (D-004) |

## Cel

Polityka P40: Zawsze gdy plan albumu bez właściciela opłacony przez uczestnika, wtedy przejmij album (jeśli to jego aktywna rezerwacja) — część epiku E9 „Sukcesja i przejęcie albumu (następca albo inny uczestnik)”.

## Kontekst

- Moduł **album** — kluczowe pytanie: Kto współtworzy ten wspólny zbiór i co do niego wniósł?
- Czego moduł nie wie (nie wolno tego wymagać): jak działa wydawanie mediów i link; pieniądze (tylko limit miejsc); treść wspomnień; dlaczego album stracił właściciela
- Projekt modułu: model procesów modułu album
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Polityka P40: Zawsze gdy plan albumu bez właściciela opłacony przez uczestnika, wtedy przejmij album (jeśli to jego aktywna rezerwacja) (źródło: ES:P40 — przegląd decyzji 2026-10-19 (D-004))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Given zdarzenie E171, when handler je przetwarza, then wywołana jest komenda C156. *Sprawdzenie: test integracyjny handlera*
- [ ] Podwójne dostarczenie zdarzenia nie daje drugiego skutku (inbox, ADR 0006). *Sprawdzenie: test idempotentności*
- [ ] Zdarzenie starsze niż ostatnio zastosowane dla tej samej pary (zasób, osoba) jest pomijane (D-013). *Sprawdzenie: test jednostkowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
