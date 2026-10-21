# MEM-E7-PLANY-20: Zdarzenie integracyjne „Plan aktywny” (E94) w kontrakcie modułu plany

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E7](epiki/E7-plany-limity-i-retencja-24-miesiace.md) Plany, limity i retencja 24 miesiące |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, plany, MEM-E7-PLANY-20 |
| Moduł | plany |
| Warstwa | kontrakt |
| Rodzaj | kontrakt |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-PLANY-01](MEM-E0-PLANY-01-szkielet-modulu-plany-struktura-publiczny-kontrakt.md); [MEM-E7-PLANY-01](MEM-E7-PLANY-01-nadaj-uprawnienia-planu-c90.md) |
| Blokuje | [MEM-E7-LIMITY-02](MEM-E7-LIMITY-02-polityka-p27-zawsze-gdy-plan-aktywny-albo-przelacz.md); [MEM-E7-ARCHIWUM-05](MEM-E7-ARCHIWUM-05-polityka-p44-zawsze-gdy-plan-znowu-aktywny-w-ciagu.md) |
| Kod roboczy | MEM-E7-PLANY-20 |
| Źródło | ES:E94; kontrakty zdarzeń; ADR 0006 (outbox) |

## Cel

Zdarzenie integracyjne „Plan aktywny” (E94) w kontrakcie modułu plany — część epiku E7 „Plany, limity i retencja 24 miesiące”.

## Kontekst

- Moduł **plany** — kluczowe pytanie: W jakim stanie jest teraz plan tego konta lub albumu i co z tego wynika?
- Czego moduł nie wie (nie wolno tego wymagać): co jest w archiwum; kto co wniósł; zgony, następcy, odmowy przejęcia (zna tylko „album bez właściciela”)
- Projekt modułu: model procesów modułu plany
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zdarzenie integracyjne „Plan aktywny” (E94) w kontrakcie modułu plany (źródło: ES:E94; kontrakty zdarzeń; ADR 0006 (outbox))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E7-LIMITY-02, MEM-E7-ARCHIWUM-05)

## Kryteria akceptacji

- [ ] Zdarzenie „Plan aktywny” zapisywane w outboxie w tej samej transakcji co zmiana stanu. *Sprawdzenie: test integracyjny (awaria po zapisie nie gubi zdarzenia)*
- [ ] Ładunek zawiera tylko skutek (id zasobu, osoba, numer kolejny, czas), nie przyczynę. *Sprawdzenie: test kontraktu*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
