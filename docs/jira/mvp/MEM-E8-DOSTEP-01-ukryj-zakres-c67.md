# MEM-E8-DOSTEP-01: Ukryj zakres (C67)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E8](epiki/E8-zgloszenia-moderacja-i-nakazy-organow.md) Zgłoszenia, moderacja i nakazy organów |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, dostep, MEM-E8-DOSTEP-01 |
| Moduł | dostep |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-DOSTEP-01](MEM-E0-DOSTEP-01-szkielet-modulu-dostep-struktura-publiczny-kontrak.md) |
| Blokuje | [MEM-E8-DOSTEP-02](MEM-E8-DOSTEP-02-write-port-dostep-dla-moderacja-ukryj-zakres-natyc.md); [MEM-E8-DOSTEP-03](MEM-E8-DOSTEP-03-polityka-p32-zawsze-gdy-tresc-ukryta-decyzja-moder.md) |
| Kod roboczy | MEM-E8-DOSTEP-01 |
| Źródło | ES:C67 — przejście 2: Moderacja → Dostęp: skutek „zakres ukryty” (S5 A9) |

## Cel

Ukryj zakres (C67) — część epiku E8 „Zgłoszenia, moderacja i nakazy organów”.

## Kontekst

- Moduł **dostep** — kluczowe pytanie: Czy ta osoba może teraz zobaczyć tę treść?
- Czego moduł nie wie (nie wolno tego wymagać): dlaczego ktoś ma lub stracił dostęp; zawartość treści
- Projekt modułu: model procesów modułu dostep
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Ukryj zakres (C67) (źródło: ES:C67 — przejście 2: Moderacja → Dostęp: skutek „zakres ukryty” (S5 A9))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E8-DOSTEP-02, MEM-E8-DOSTEP-03)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when polityka/port wykonuje „Ukryj zakres”, then powstaje „Zakres ukryty” (E161). *Sprawdzenie: test jednostkowy handlera*
- [ ] Niepoprawne dane wejściowe odrzucone walidacją z czytelnym komunikatem, bez zapisu. *Sprawdzenie: test jednostkowy schematu wejścia*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
