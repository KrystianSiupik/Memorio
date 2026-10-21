# MEM-E9-ALBUM-08: Polityka P37: Zawsze gdy konto właściciela bez następcy usunięte (ze zgodą na wygaśnięcie albumu), wtedy zwolnij rolę właściciela

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E9](epiki/E9-sukcesja-i-przejecie-albumu-nastepca-albo-inny-ucz.md) Sukcesja i przejęcie albumu (następca albo inny uczestnik) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, album, MEM-E9-ALBUM-08 |
| Moduł | album |
| Warstwa | aplikacja (handler zdarzenia) |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-ALBUM-01](MEM-E0-ALBUM-01-szkielet-modulu-album-struktura-publiczny-kontrakt.md); [MEM-E9-ALBUM-01](MEM-E9-ALBUM-01-zwolnij-role-wlasciciela-albumu-c153.md); [MEM-E1-KONTO-08](MEM-E1-KONTO-08-zdarzenie-integracyjne-konto-usuniete-e7-w-kontrak.md) |
| Blokuje | - |
| Kod roboczy | MEM-E9-ALBUM-08 |
| Źródło | ES:P37 — S2:B3 KOMENDA B „świadomej zgody na wygaśnięcie albumu”; D-002 |

## Cel

Polityka P37: Zawsze gdy konto właściciela bez następcy usunięte (ze zgodą na wygaśnięcie albumu), wtedy zwolnij rolę właściciela — część epiku E9 „Sukcesja i przejęcie albumu (następca albo inny uczestnik)”.

## Kontekst

- Moduł **album** — kluczowe pytanie: Kto współtworzy ten wspólny zbiór i co do niego wniósł?
- Czego moduł nie wie (nie wolno tego wymagać): jak działa wydawanie mediów i link; pieniądze (tylko limit miejsc); treść wspomnień; dlaczego album stracił właściciela
- Projekt modułu: model procesów modułu album
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Polityka P37: Zawsze gdy konto właściciela bez następcy usunięte (ze zgodą na wygaśnięcie albumu), wtedy zwolnij rolę właściciela (źródło: ES:P37 — S2:B3 KOMENDA B „świadomej zgody na wygaśnięcie albumu”; D-002)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Given zdarzenie E7, when handler je przetwarza, then wywołana jest komenda C153. *Sprawdzenie: test integracyjny handlera*
- [ ] Podwójne dostarczenie zdarzenia nie daje drugiego skutku (inbox, ADR 0006). *Sprawdzenie: test idempotentności*
- [ ] Zdarzenie starsze niż ostatnio zastosowane dla tej samej pary (zasób, osoba) jest pomijane (D-013). *Sprawdzenie: test jednostkowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
