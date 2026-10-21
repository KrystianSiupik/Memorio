# MEM-E3-POWIADOMIENIA-07: Polityka P6: Zawsze gdy minie 7 dni od nieprzyjętego zaproszenia, wtedy wyślij jedno przypomnienie

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E3](epiki/E3-album-wspoldzielony-sklad-wklady-zgody.md) Album współdzielony: skład, wkłady, zgody |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, powiadomienia, MEM-E3-POWIADOMIENIA-07 |
| Moduł | powiadomienia |
| Warstwa | aplikacja (handler zdarzenia) |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-POWIADOMIENIA-01](MEM-E0-POWIADOMIENIA-01-szkielet-modulu-powiadomienia-struktura-publiczny-.md); [MEM-E3-POWIADOMIENIA-01](MEM-E3-POWIADOMIENIA-01-wyslij-przypomnienie-o-zaproszeniu-c43.md); [MEM-E3-ALBUM-20](MEM-E3-ALBUM-20-zdarzenie-integracyjne-minelo-7-dni-od-zaproszenia.md) |
| Blokuje | - |
| Kod roboczy | MEM-E3-POWIADOMIENIA-07 |
| Źródło | ES:P6 — S2:C10 |

## Cel

Polityka P6: Zawsze gdy minie 7 dni od nieprzyjętego zaproszenia, wtedy wyślij jedno przypomnienie — część epiku E3 „Album współdzielony: skład, wkłady, zgody”.

## Kontekst

- Moduł **powiadomienia** — kluczowe pytanie: Czy wiadomość dotarła i do kogo nie wolno pisać?
- Czego moduł nie wie (nie wolno tego wymagać): sens biznesowy wiadomości poza kategorią
- Projekt modułu: model procesów modułu powiadomienia
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Polityka P6: Zawsze gdy minie 7 dni od nieprzyjętego zaproszenia, wtedy wyślij jedno przypomnienie (źródło: ES:P6 — S2:C10)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Given zdarzenie E45, when handler je przetwarza, then wywołana jest komenda C43. *Sprawdzenie: test integracyjny handlera*
- [ ] Podwójne dostarczenie zdarzenia nie daje drugiego skutku (inbox, ADR 0006). *Sprawdzenie: test idempotentności*
- [ ] Zdarzenie starsze niż ostatnio zastosowane dla tej samej pary (zasób, osoba) jest pomijane (D-013). *Sprawdzenie: test jednostkowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
