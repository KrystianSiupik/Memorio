# MEM-E2-MIEJSCA-04: Read port miejsca → dla archiwum: odwołanie do miejsca (wpis OSM albo miejsce własne) i jego istnienie

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E2](epiki/E2-wspomnienia-na-prywatnej-mapie-tworzenie-kosz-usuw.md) Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, miejsca, MEM-E2-MIEJSCA-04 |
| Moduł | miejsca |
| Warstwa | kontrakt |
| Rodzaj | kontrakt |
| Klasa problemu | T&P |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-MIEJSCA-01](MEM-E0-MIEJSCA-01-szkielet-modulu-miejsca-struktura-publiczny-kontra.md); [MEM-E2-MIEJSCA-01](MEM-E2-MIEJSCA-01-wybierz-miejsce-z-podpowiedzi-c10.md); [MEM-E2-MIEJSCA-02](MEM-E2-MIEJSCA-02-dodaj-miejsce-wlasne-c11.md) |
| Blokuje | [MEM-E2-ARCHIWUM-01](MEM-E2-ARCHIWUM-01-utworz-wspomnienie-c12.md) |
| Kod roboczy | MEM-E2-MIEJSCA-04 |
| Źródło | kontrakty modułów porty: archiwum → miejsca (read-port) C12 |

## Cel

Read port miejsca → dla archiwum: odwołanie do miejsca (wpis OSM albo miejsce własne) i jego istnienie — część epiku E2 „Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie”.

## Kontekst

- Moduł **miejsca** — kluczowe pytanie: Jakie to miejsce na świecie i jak autor je nazywa?
- Czego moduł nie wie (nie wolno tego wymagać): czyje wspomnienia wiszą pod miejscem i co zawierają
- Projekt modułu: model procesów modułu miejsca
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Read port miejsca → dla archiwum: odwołanie do miejsca (wpis OSM albo miejsce własne) i jego istnienie (źródło: kontrakty modułów porty: archiwum → miejsca (read-port) C12)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E2-ARCHIWUM-01)

## Kryteria akceptacji

- [ ] Read port miejsca → dla archiwum: odwołanie do miejsca (wpis OSM albo miejsce własne) i jego istnienie. *Sprawdzenie: test/skrypt w CI lub ręcznie na środowisku*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
