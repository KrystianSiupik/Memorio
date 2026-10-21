# MEM-E4-DOSTEP-04: Odbierz dostęp osobie (C63)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E4](epiki/E4-udostepnianie-linkiem-i-odbieranie-dostepu.md) Udostępnianie linkiem i odbieranie dostępu |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, dostep, MEM-E4-DOSTEP-04 |
| Moduł | dostep |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-DOSTEP-01](MEM-E0-DOSTEP-01-szkielet-modulu-dostep-struktura-publiczny-kontrak.md); [MEM-E4-DOSTEP-07](MEM-E4-DOSTEP-07-zapytanie-imienna-lista-ogladajacych-albumu-r10.md) |
| Blokuje | [MEM-E4-DOSTEP-08](MEM-E4-DOSTEP-08-zdarzenie-integracyjne-dostep-odebrany-e66-w-kontr.md); [MEM-E4-DOSTEP-10](MEM-E4-DOSTEP-10-polityka-p30-zawsze-gdy-uczestnik-opuscil-album-al.md); [MEM-E3-PLAT-03](MEM-E3-PLAT-03-ekran-s11-zarzadzanie-albumem-wlasciciel.md) |
| Kod roboczy | MEM-E4-DOSTEP-04 |
| Źródło | ES:C63 — S2:B2 KOMENDA C |

## Cel

Odbierz dostęp osobie (C63) — część epiku E4 „Udostępnianie linkiem i odbieranie dostępu”.

## Kontekst

- Moduł **dostep** — kluczowe pytanie: Czy ta osoba może teraz zobaczyć tę treść?
- Czego moduł nie wie (nie wolno tego wymagać): dlaczego ktoś ma lub stracił dostęp; zawartość treści
- Projekt modułu: model procesów modułu dostep
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Odbierz dostęp osobie (C63) (źródło: ES:C63 — S2:B2 KOMENDA C)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E4-DOSTEP-08, MEM-E4-DOSTEP-10, MEM-E3-PLAT-03)

## Kryteria akceptacji

- [ ] Given aktor widzi: Imienna lista oglądających albumu, when Właściciel albumu wykonuje „Odbierz dostęp osobie”, then powstaje „Dostęp odebrany” (E66). *Sprawdzenie: test jednostkowy handlera*
- [ ] Niepoprawne dane wejściowe odrzucone walidacją z czytelnym komunikatem, bez zapisu. *Sprawdzenie: test jednostkowy schematu wejścia*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
