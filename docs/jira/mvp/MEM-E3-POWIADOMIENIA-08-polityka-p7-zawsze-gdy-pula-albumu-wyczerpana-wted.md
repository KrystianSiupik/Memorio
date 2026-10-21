# MEM-E3-POWIADOMIENIA-08: Polityka P7: Zawsze gdy pula albumu wyczerpana, wtedy powiadom właściciela z prośbą o większy plan

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E3](epiki/E3-album-wspoldzielony-sklad-wklady-zgody.md) Album współdzielony: skład, wkłady, zgody |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, powiadomienia, MEM-E3-POWIADOMIENIA-08 |
| Moduł | powiadomienia |
| Warstwa | aplikacja (handler zdarzenia) |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-POWIADOMIENIA-01](MEM-E0-POWIADOMIENIA-01-szkielet-modulu-powiadomienia-struktura-publiczny-.md); [MEM-E3-POWIADOMIENIA-03](MEM-E3-POWIADOMIENIA-03-powiadom-wlasciciela-o-pelnej-puli-c48.md); [MEM-E3-ALBUM-21](MEM-E3-ALBUM-21-zdarzenie-integracyjne-pula-albumu-wyczerpana-e51-.md) |
| Blokuje | - |
| Kod roboczy | MEM-E3-POWIADOMIENIA-08 |
| Źródło | ES:P7 — S2:B1 „WŁAŚCICIEL dostaje powiadomienie” |

## Cel

Polityka P7: Zawsze gdy pula albumu wyczerpana, wtedy powiadom właściciela z prośbą o większy plan — część epiku E3 „Album współdzielony: skład, wkłady, zgody”.

## Kontekst

- Moduł **powiadomienia** — kluczowe pytanie: Czy wiadomość dotarła i do kogo nie wolno pisać?
- Czego moduł nie wie (nie wolno tego wymagać): sens biznesowy wiadomości poza kategorią
- Projekt modułu: model procesów modułu powiadomienia
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Polityka P7: Zawsze gdy pula albumu wyczerpana, wtedy powiadom właściciela z prośbą o większy plan (źródło: ES:P7 — S2:B1 „WŁAŚCICIEL dostaje powiadomienie”)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Given zdarzenie E51, when handler je przetwarza, then wywołana jest komenda C48. *Sprawdzenie: test integracyjny handlera*
- [ ] Podwójne dostarczenie zdarzenia nie daje drugiego skutku (inbox, ADR 0006). *Sprawdzenie: test idempotentności*
- [ ] Zdarzenie starsze niż ostatnio zastosowane dla tej samej pary (zasób, osoba) jest pomijane (D-013). *Sprawdzenie: test jednostkowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
