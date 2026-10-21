# MEM-E3-LIMITY-02: Read port: Stan puli albumu (R6)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E3](epiki/E3-album-wspoldzielony-sklad-wklady-zgody.md) Album współdzielony: skład, wkłady, zgody |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, limity, MEM-E3-LIMITY-02 |
| Moduł | limity |
| Warstwa | kontrakt |
| Rodzaj | kontrakt |
| Klasa problemu | T&P |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-LIMITY-01](MEM-E0-LIMITY-01-szkielet-modulu-limity-struktura-publiczny-kontrak.md) |
| Blokuje | [MEM-E3-ALBUM-05](MEM-E3-ALBUM-05-dodaj-wspomnienie-do-albumu-c47.md); [MEM-E3-ALBUM-13](MEM-E3-ALBUM-13-dodaj-zdjecia-do-wspomnienia-w-albumie-c56.md); [MEM-E3-PLAT-02](MEM-E3-PLAT-02-ekran-s10-album.md) |
| Kod roboczy | MEM-E3-LIMITY-02 |
| Źródło | ES:R6 — S2:B1 „Uczestnik przed operacją widzi, ile zostało w puli” |

## Cel

Read port: Stan puli albumu (R6) — część epiku E3 „Album współdzielony: skład, wkłady, zgody”.

## Kontekst

- Moduł **limity** — kluczowe pytanie: Czy ta operacja zmieści się w puli i ile zostało?
- Czego moduł nie wie (nie wolno tego wymagać): skąd limit; kto płaci; czego dotyczy treść
- Projekt modułu: model procesów modułu limity
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Read port: Stan puli albumu (R6) (źródło: ES:R6 — S2:B1 „Uczestnik przed operacją widzi, ile zostało w puli”)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E3-ALBUM-05, MEM-E3-ALBUM-13, MEM-E3-PLAT-02)

## Kryteria akceptacji

- [ ] Given dane źródłowe, when zapytanie „Stan puli albumu”, then wynik zawiera tylko treści dostępne dla pytającego (filtr Dostępu, ADR 0011). *Sprawdzenie: test integracyjny macierzy użytkownik × zasób*
- [ ] Projekcja nie ma logiki biznesowej i daje się odbudować z danych źródłowych. *Sprawdzenie: review + test przebudowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
