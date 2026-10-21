# MEM-E3-ARCHIWUM-01: Write port archiwum → dla album: zapisz treść z numerem puli albumu

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E3](epiki/E3-album-wspoldzielony-sklad-wklady-zgody.md) Album współdzielony: skład, wkłady, zgody |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, archiwum, MEM-E3-ARCHIWUM-01 |
| Moduł | archiwum |
| Warstwa | kontrakt |
| Rodzaj | kontrakt |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-ARCHIWUM-01](MEM-E0-ARCHIWUM-01-szkielet-modulu-archiwum-struktura-publiczny-kontr.md); [MEM-E2-ARCHIWUM-02](MEM-E2-ARCHIWUM-02-dodaj-zdjecia-do-wspomnienia-c13.md) |
| Blokuje | [MEM-E3-ALBUM-13](MEM-E3-ALBUM-13-dodaj-zdjecia-do-wspomnienia-w-albumie-c56.md) |
| Kod roboczy | MEM-E3-ARCHIWUM-01 |
| Źródło | kontrakty modułów porty: album → archiwum (write-port) C56 |

## Cel

Write port archiwum → dla album: zapisz treść z numerem puli albumu — część epiku E3 „Album współdzielony: skład, wkłady, zgody”.

## Kontekst

- Moduł **archiwum** — kluczowe pytanie: Co ta osoba zapisała, gdzie i kiedy?
- Czego moduł nie wie (nie wolno tego wymagać): kto poza autorem widzi treść; w których albumach jest wspomnienie; plany i pieniądze; czy treść poszła do AI
- Projekt modułu: model procesów modułu archiwum
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Write port archiwum → dla album: zapisz treść z numerem puli albumu (źródło: kontrakty modułów porty: album → archiwum (write-port) C56)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E3-ALBUM-13)

## Kryteria akceptacji

- [ ] Write port archiwum → dla album: zapisz treść z numerem puli albumu. *Sprawdzenie: test/skrypt w CI lub ręcznie na środowisku*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
