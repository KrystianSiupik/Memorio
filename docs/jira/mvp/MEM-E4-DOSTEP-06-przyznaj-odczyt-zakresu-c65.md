# MEM-E4-DOSTEP-06: Przyznaj odczyt zakresu (C65)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E4](epiki/E4-udostepnianie-linkiem-i-odbieranie-dostepu.md) Udostępnianie linkiem i odbieranie dostępu |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, dostep, MEM-E4-DOSTEP-06 |
| Moduł | dostep |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-DOSTEP-01](MEM-E0-DOSTEP-01-szkielet-modulu-dostep-struktura-publiczny-kontrak.md) |
| Blokuje | [MEM-E6-DOSTEP-01](MEM-E6-DOSTEP-01-read-port-dostep-dla-narracja-ai-ktore-tresci-osob.md); [MEM-E4-DOSTEP-09](MEM-E4-DOSTEP-09-polityka-p29-zawsze-gdy-zaproszenie-przyjete-wtedy.md); [MEM-E4-DOSTEP-11](MEM-E4-DOSTEP-11-polityka-p31-zawsze-gdy-album-prywatny-udostepnion.md) |
| Kod roboczy | MEM-E4-DOSTEP-06 |
| Źródło | ES:C65 — przejście 2: Dostęp przyjmuje skutki z Albumu i Sukcesji (S5 A8) |

## Cel

Przyznaj odczyt zakresu (C65) — część epiku E4 „Udostępnianie linkiem i odbieranie dostępu”.

## Kontekst

- Moduł **dostep** — kluczowe pytanie: Czy ta osoba może teraz zobaczyć tę treść?
- Czego moduł nie wie (nie wolno tego wymagać): dlaczego ktoś ma lub stracił dostęp; zawartość treści
- Projekt modułu: model procesów modułu dostep
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Przyznaj odczyt zakresu (C65) (źródło: ES:C65 — przejście 2: Dostęp przyjmuje skutki z Albumu i Sukcesji (S5 A8))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E6-DOSTEP-01, MEM-E4-DOSTEP-09, MEM-E4-DOSTEP-11)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when polityka/port wykonuje „Przyznaj odczyt zakresu”, then powstaje „Odczyt przyznany” (E160). *Sprawdzenie: test jednostkowy handlera*
- [ ] Niepoprawne dane wejściowe odrzucone walidacją z czytelnym komunikatem, bez zapisu. *Sprawdzenie: test jednostkowy schematu wejścia*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
