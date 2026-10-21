# MEM-E7-ARCHIWUM-04: Zdarzenie integracyjne „Archiwum przycięte (nadmiar w koszu 30 dni)” (E25) w kontrakcie modułu archiwum

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E7](epiki/E7-plany-limity-i-retencja-24-miesiace.md) Plany, limity i retencja 24 miesiące |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, archiwum, MEM-E7-ARCHIWUM-04 |
| Moduł | archiwum |
| Warstwa | kontrakt |
| Rodzaj | kontrakt |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-ARCHIWUM-01](MEM-E0-ARCHIWUM-01-szkielet-modulu-archiwum-struktura-publiczny-kontr.md); [MEM-E7-ARCHIWUM-01](MEM-E7-ARCHIWUM-01-przytnij-archiwum-do-limitu-nadmiar-do-kosza-na-30.md) |
| Blokuje | [MEM-E6-POCHODZENIE-08](MEM-E6-POCHODZENIE-08-polityka-p34-zawsze-gdy-archiwum-przyciete-wtedy-u.md) |
| Kod roboczy | MEM-E7-ARCHIWUM-04 |
| Źródło | ES:E25; kontrakty zdarzeń; ADR 0006 (outbox) |

## Cel

Zdarzenie integracyjne „Archiwum przycięte (nadmiar w koszu 30 dni)” (E25) w kontrakcie modułu archiwum — część epiku E7 „Plany, limity i retencja 24 miesiące”.

## Kontekst

- Moduł **archiwum** — kluczowe pytanie: Co ta osoba zapisała, gdzie i kiedy?
- Czego moduł nie wie (nie wolno tego wymagać): kto poza autorem widzi treść; w których albumach jest wspomnienie; plany i pieniądze; czy treść poszła do AI
- Projekt modułu: model procesów modułu archiwum
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zdarzenie integracyjne „Archiwum przycięte (nadmiar w koszu 30 dni)” (E25) w kontrakcie modułu archiwum (źródło: ES:E25; kontrakty zdarzeń; ADR 0006 (outbox))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E6-POCHODZENIE-08)

## Kryteria akceptacji

- [ ] Zdarzenie „Archiwum przycięte (nadmiar w koszu 30 dni)” zapisywane w outboxie w tej samej transakcji co zmiana stanu. *Sprawdzenie: test integracyjny (awaria po zapisie nie gubi zdarzenia)*
- [ ] Ładunek zawiera tylko skutek (id zasobu, osoba, numer kolejny, czas), nie przyczynę. *Sprawdzenie: test kontraktu*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
