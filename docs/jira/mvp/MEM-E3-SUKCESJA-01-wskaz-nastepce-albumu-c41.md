# MEM-E3-SUKCESJA-01: Wskaż następcę albumu (C41)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E3](epiki/E3-album-wspoldzielony-sklad-wklady-zgody.md) Album współdzielony: skład, wkłady, zgody |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, sukcesja, MEM-E3-SUKCESJA-01 |
| Moduł | sukcesja |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-SUKCESJA-01](MEM-E0-SUKCESJA-01-szkielet-modulu-sukcesja-struktura-publiczny-kontr.md) |
| Blokuje | [MEM-E3-PLAT-03](MEM-E3-PLAT-03-ekran-s11-zarzadzanie-albumem-wlasciciel.md) |
| Kod roboczy | MEM-E3-SUKCESJA-01 |
| Źródło | ES:C41 — S2:B3 „Właściciel wyznacza następcę… przy zakładaniu albumu Premium” |

## Cel

Wskaż następcę albumu (C41) — część epiku E3 „Album współdzielony: skład, wkłady, zgody”.

## Kontekst

- Moduł **sukcesja** — kluczowe pytanie: Kto przejmuje album lub odczyt, gdy właściciela już nie ma?
- Czego moduł nie wie (nie wolno tego wymagać): budowa albumu; mechanika dostępu
- Projekt modułu: model procesów modułu sukcesja
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Wskaż następcę albumu (C41) (źródło: ES:C41 — S2:B3 „Właściciel wyznacza następcę… przy zakładaniu albumu Premium”)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E3-PLAT-03)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when Właściciel albumu wykonuje „Wskaż następcę albumu”, then powstaje „Następca wskazany” (E41). *Sprawdzenie: test jednostkowy handlera*
- [ ] Niepoprawne dane wejściowe odrzucone walidacją z czytelnym komunikatem, bez zapisu. *Sprawdzenie: test jednostkowy schematu wejścia*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
