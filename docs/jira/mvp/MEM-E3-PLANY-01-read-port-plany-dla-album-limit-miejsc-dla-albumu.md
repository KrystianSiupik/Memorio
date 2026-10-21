# MEM-E3-PLANY-01: Read port plany → dla album: limit miejsc dla albumu

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E3](epiki/E3-album-wspoldzielony-sklad-wklady-zgody.md) Album współdzielony: skład, wkłady, zgody |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, plany, MEM-E3-PLANY-01 |
| Moduł | plany |
| Warstwa | kontrakt |
| Rodzaj | kontrakt |
| Klasa problemu | T&P |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-PLANY-01](MEM-E0-PLANY-01-szkielet-modulu-plany-struktura-publiczny-kontrakt.md); [MEM-E7-PLANY-01](MEM-E7-PLANY-01-nadaj-uprawnienia-planu-c90.md) |
| Blokuje | [MEM-E3-ALBUM-02](MEM-E3-ALBUM-02-zapros-osobe-do-albumu-c42.md) |
| Kod roboczy | MEM-E3-PLANY-01 |
| Źródło | kontrakty modułów porty: album → plany (read-port) C42 |

## Cel

Read port plany → dla album: limit miejsc dla albumu — część epiku E3 „Album współdzielony: skład, wkłady, zgody”.

## Kontekst

- Moduł **plany** — kluczowe pytanie: W jakim stanie jest teraz plan tego konta lub albumu i co z tego wynika?
- Czego moduł nie wie (nie wolno tego wymagać): co jest w archiwum; kto co wniósł; zgony, następcy, odmowy przejęcia (zna tylko „album bez właściciela”)
- Projekt modułu: model procesów modułu plany
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Read port plany → dla album: limit miejsc dla albumu (źródło: kontrakty modułów porty: album → plany (read-port) C42)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E3-ALBUM-02)

## Kryteria akceptacji

- [ ] Read port plany → dla album: limit miejsc dla albumu. *Sprawdzenie: test/skrypt w CI lub ręcznie na środowisku*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
