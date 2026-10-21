# MEM-E3-ALBUM-06: Powierz treści kuratorowi (C49)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E3](epiki/E3-album-wspoldzielony-sklad-wklady-zgody.md) Album współdzielony: skład, wkłady, zgody |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, album, MEM-E3-ALBUM-06 |
| Moduł | album |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-ALBUM-01](MEM-E0-ALBUM-01-szkielet-modulu-album-struktura-publiczny-kontrakt.md) |
| Blokuje | [MEM-E3-PLAT-04](MEM-E3-PLAT-04-ekran-s12-moje-zgody-i-powierzenia-w-albumie.md) |
| Kod roboczy | MEM-E3-ALBUM-06 |
| Źródło | ES:C49 — S2:B3 ROLA Kurator |

## Cel

Powierz treści kuratorowi (C49) — część epiku E3 „Album współdzielony: skład, wkłady, zgody”.

## Kontekst

- Moduł **album** — kluczowe pytanie: Kto współtworzy ten wspólny zbiór i co do niego wniósł?
- Czego moduł nie wie (nie wolno tego wymagać): jak działa wydawanie mediów i link; pieniądze (tylko limit miejsc); treść wspomnień; dlaczego album stracił właściciela
- Projekt modułu: model procesów modułu album
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Powierz treści kuratorowi (C49) (źródło: ES:C49 — S2:B3 ROLA Kurator)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E3-PLAT-04)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when Uczestnik albumu wykonuje „Powierz treści kuratorowi”, then powstaje „Treści powierzone kuratorowi” (E53). *Sprawdzenie: test jednostkowy handlera*
- [ ] Niepoprawne dane wejściowe odrzucone walidacją z czytelnym komunikatem, bez zapisu. *Sprawdzenie: test jednostkowy schematu wejścia*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
