# MEM-E3-POWIADOMIENIA-09: Polityka P8: Zawsze gdy uczestnik usunięty z albumu, wtedy poproś go o wybór zostaw/zabierz z terminem

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E3](epiki/E3-album-wspoldzielony-sklad-wklady-zgody.md) Album współdzielony: skład, wkłady, zgody |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, powiadomienia, MEM-E3-POWIADOMIENIA-09 |
| Moduł | powiadomienia |
| Warstwa | aplikacja (handler zdarzenia) |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-POWIADOMIENIA-01](MEM-E0-POWIADOMIENIA-01-szkielet-modulu-powiadomienia-struktura-publiczny-.md); [MEM-E3-POWIADOMIENIA-04](MEM-E3-POWIADOMIENIA-04-popros-o-wybor-zostaw-zabierz-c54.md); [MEM-E3-ALBUM-22](MEM-E3-ALBUM-22-zdarzenie-integracyjne-uczestnik-usuniety-z-albumu.md) |
| Blokuje | - |
| Kod roboczy | MEM-E3-POWIADOMIENIA-09 |
| Źródło | ES:P8 — S2:B1 „odchodzący dostaje powiadomienie z tym samym wyborem i terminem” |

## Cel

Polityka P8: Zawsze gdy uczestnik usunięty z albumu, wtedy poproś go o wybór zostaw/zabierz z terminem — część epiku E3 „Album współdzielony: skład, wkłady, zgody”.

## Kontekst

- Moduł **powiadomienia** — kluczowe pytanie: Czy wiadomość dotarła i do kogo nie wolno pisać?
- Czego moduł nie wie (nie wolno tego wymagać): sens biznesowy wiadomości poza kategorią
- Projekt modułu: model procesów modułu powiadomienia
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Polityka P8: Zawsze gdy uczestnik usunięty z albumu, wtedy poproś go o wybór zostaw/zabierz z terminem (źródło: ES:P8 — S2:B1 „odchodzący dostaje powiadomienie z tym samym wyborem i terminem”)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Given zdarzenie E59, when handler je przetwarza, then wywołana jest komenda C54. *Sprawdzenie: test integracyjny handlera*
- [ ] Podwójne dostarczenie zdarzenia nie daje drugiego skutku (inbox, ADR 0006). *Sprawdzenie: test idempotentności*
- [ ] Zdarzenie starsze niż ostatnio zastosowane dla tej samej pary (zasób, osoba) jest pomijane (D-013). *Sprawdzenie: test jednostkowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
