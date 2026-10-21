# MEM-E4-DOSTEP-08: Zdarzenie integracyjne „Dostęp odebrany” (E66) w kontrakcie modułu dostep

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E4](epiki/E4-udostepnianie-linkiem-i-odbieranie-dostepu.md) Udostępnianie linkiem i odbieranie dostępu |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, dostep, MEM-E4-DOSTEP-08 |
| Moduł | dostep |
| Warstwa | kontrakt |
| Rodzaj | kontrakt |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-DOSTEP-01](MEM-E0-DOSTEP-01-szkielet-modulu-dostep-struktura-publiczny-kontrak.md); [MEM-E4-DOSTEP-04](MEM-E4-DOSTEP-04-odbierz-dostep-osobie-c63.md) |
| Blokuje | [MEM-E6-POCHODZENIE-06](MEM-E6-POCHODZENIE-06-polityka-p10-zawsze-gdy-dostep-odebrany-wtedy-unie.md) |
| Kod roboczy | MEM-E4-DOSTEP-08 |
| Źródło | ES:E66; kontrakty zdarzeń; ADR 0006 (outbox) |

## Cel

Zdarzenie integracyjne „Dostęp odebrany” (E66) w kontrakcie modułu dostep — część epiku E4 „Udostępnianie linkiem i odbieranie dostępu”.

## Kontekst

- Moduł **dostep** — kluczowe pytanie: Czy ta osoba może teraz zobaczyć tę treść?
- Czego moduł nie wie (nie wolno tego wymagać): dlaczego ktoś ma lub stracił dostęp; zawartość treści
- Projekt modułu: model procesów modułu dostep
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zdarzenie integracyjne „Dostęp odebrany” (E66) w kontrakcie modułu dostep (źródło: ES:E66; kontrakty zdarzeń; ADR 0006 (outbox))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E6-POCHODZENIE-06)

## Kryteria akceptacji

- [ ] Zdarzenie „Dostęp odebrany” zapisywane w outboxie w tej samej transakcji co zmiana stanu. *Sprawdzenie: test integracyjny (awaria po zapisie nie gubi zdarzenia)*
- [ ] Ładunek zawiera tylko skutek (id zasobu, osoba, numer kolejny, czas), nie przyczynę. *Sprawdzenie: test kontraktu*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
