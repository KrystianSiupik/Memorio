# MEM-E3-ALBUM-14: Oznacz uczestnika albumu we wspomnieniu (C169)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E3](epiki/E3-album-wspoldzielony-sklad-wklady-zgody.md) Album współdzielony: skład, wkłady, zgody |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, album, MEM-E3-ALBUM-14 |
| Moduł | album |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-ALBUM-01](MEM-E0-ALBUM-01-szkielet-modulu-album-struktura-publiczny-kontrakt.md); [MEM-E3-ALBUM-17](MEM-E3-ALBUM-17-zapytanie-album-wspomnienia-sklad-i-moje-wklady-r1.md) |
| Blokuje | [MEM-E3-ALBUM-26](MEM-E3-ALBUM-26-zdarzenie-integracyjne-uczestnik-oznaczony-we-wspo.md); [MEM-E3-PLAT-02](MEM-E3-PLAT-02-ekran-s10-album.md) |
| Kod roboczy | MEM-E3-ALBUM-14 |
| Źródło | ES:C169 — przegląd decyzji 2026-10-19 (D-079) |

## Cel

Oznacz uczestnika albumu we wspomnieniu (C169) — część epiku E3 „Album współdzielony: skład, wkłady, zgody”.

## Kontekst

- Moduł **album** — kluczowe pytanie: Kto współtworzy ten wspólny zbiór i co do niego wniósł?
- Czego moduł nie wie (nie wolno tego wymagać): jak działa wydawanie mediów i link; pieniądze (tylko limit miejsc); treść wspomnień; dlaczego album stracił właściciela
- Projekt modułu: model procesów modułu album
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Oznacz uczestnika albumu we wspomnieniu (C169) (źródło: ES:C169 — przegląd decyzji 2026-10-19 (D-079))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E3-ALBUM-26, MEM-E3-PLAT-02)

## Kryteria akceptacji

- [ ] Given aktor widzi: Album: wspomnienia, skład i moje wkłady, when Uczestnik albumu wykonuje „Oznacz uczestnika albumu we wspomnieniu”, then powstaje „Uczestnik oznaczony we wspomnieniu” (E175). *Sprawdzenie: test jednostkowy handlera*
- [ ] Niepoprawne dane wejściowe odrzucone walidacją z czytelnym komunikatem, bez zapisu. *Sprawdzenie: test jednostkowy schematu wejścia*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
