# MEM-E9-POWIADOMIENIA-03: Polityka P42: Zawsze gdy album przejęty przez uczestnika, wtedy powiadom pozostałych (mogą wyjść z wyborem zostaw/zabierz)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E9](epiki/E9-sukcesja-i-przejecie-albumu-nastepca-albo-inny-ucz.md) Sukcesja i przejęcie albumu (następca albo inny uczestnik) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, powiadomienia, MEM-E9-POWIADOMIENIA-03 |
| Moduł | powiadomienia |
| Warstwa | aplikacja (handler zdarzenia) |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-POWIADOMIENIA-01](MEM-E0-POWIADOMIENIA-01-szkielet-modulu-powiadomienia-struktura-publiczny-.md); [MEM-E9-POWIADOMIENIA-01](MEM-E9-POWIADOMIENIA-01-powiadom-uczestnikow-o-nowym-wlascicielu-c166.md); [MEM-E9-ALBUM-11](MEM-E9-ALBUM-11-zdarzenie-integracyjne-album-przejety-przez-uczest.md) |
| Blokuje | - |
| Kod roboczy | MEM-E9-POWIADOMIENIA-03 |
| Źródło | ES:P42 — przegląd decyzji 2026-10-19 (D-004) |

## Cel

Polityka P42: Zawsze gdy album przejęty przez uczestnika, wtedy powiadom pozostałych (mogą wyjść z wyborem zostaw/zabierz) — część epiku E9 „Sukcesja i przejęcie albumu (następca albo inny uczestnik)”.

## Kontekst

- Moduł **powiadomienia** — kluczowe pytanie: Czy wiadomość dotarła i do kogo nie wolno pisać?
- Czego moduł nie wie (nie wolno tego wymagać): sens biznesowy wiadomości poza kategorią
- Projekt modułu: model procesów modułu powiadomienia
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Polityka P42: Zawsze gdy album przejęty przez uczestnika, wtedy powiadom pozostałych (mogą wyjść z wyborem zostaw/zabierz) (źródło: ES:P42 — przegląd decyzji 2026-10-19 (D-004))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Given zdarzenie E167, when handler je przetwarza, then wywołana jest komenda C166. *Sprawdzenie: test integracyjny handlera*
- [ ] Podwójne dostarczenie zdarzenia nie daje drugiego skutku (inbox, ADR 0006). *Sprawdzenie: test idempotentności*
- [ ] Zdarzenie starsze niż ostatnio zastosowane dla tej samej pary (zasób, osoba) jest pomijane (D-013). *Sprawdzenie: test jednostkowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
