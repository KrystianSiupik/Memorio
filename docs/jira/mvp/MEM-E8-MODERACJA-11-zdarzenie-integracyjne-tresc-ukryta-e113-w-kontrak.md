# MEM-E8-MODERACJA-11: Zdarzenie integracyjne „Treść ukryta” (E113) w kontrakcie modułu moderacja

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E8](epiki/E8-zgloszenia-moderacja-i-nakazy-organow.md) Zgłoszenia, moderacja i nakazy organów |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, moderacja, MEM-E8-MODERACJA-11 |
| Moduł | moderacja |
| Warstwa | kontrakt |
| Rodzaj | kontrakt |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-MODERACJA-01](MEM-E0-MODERACJA-01-szkielet-modulu-moderacja-struktura-publiczny-kont.md); [MEM-E8-MODERACJA-02](MEM-E8-MODERACJA-02-rozstrzygnij-zgloszenie-c111.md) |
| Blokuje | [MEM-E8-DOSTEP-03](MEM-E8-DOSTEP-03-polityka-p32-zawsze-gdy-tresc-ukryta-decyzja-moder.md) |
| Kod roboczy | MEM-E8-MODERACJA-11 |
| Źródło | ES:E113; kontrakty zdarzeń; ADR 0006 (outbox) |

## Cel

Zdarzenie integracyjne „Treść ukryta” (E113) w kontrakcie modułu moderacja — część epiku E8 „Zgłoszenia, moderacja i nakazy organów”.

## Kontekst

- Moduł **moderacja** — kluczowe pytanie: Co z tą treścią robimy po zgłoszeniu lub nakazie, i w jakim terminie?
- Czego moduł nie wie (nie wolno tego wymagać): prywatna treść poza zgłoszonym zakresem; relacje w albumach
- Projekt modułu: model procesów modułu moderacja
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zdarzenie integracyjne „Treść ukryta” (E113) w kontrakcie modułu moderacja (źródło: ES:E113; kontrakty zdarzeń; ADR 0006 (outbox))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E8-DOSTEP-03)

## Kryteria akceptacji

- [ ] Zdarzenie „Treść ukryta” zapisywane w outboxie w tej samej transakcji co zmiana stanu. *Sprawdzenie: test integracyjny (awaria po zapisie nie gubi zdarzenia)*
- [ ] Ładunek zawiera tylko skutek (id zasobu, osoba, numer kolejny, czas), nie przyczynę. *Sprawdzenie: test kontraktu*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
