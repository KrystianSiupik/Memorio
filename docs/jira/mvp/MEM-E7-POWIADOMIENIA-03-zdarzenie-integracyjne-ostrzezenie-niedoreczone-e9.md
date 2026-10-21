# MEM-E7-POWIADOMIENIA-03: Zdarzenie integracyjne „Ostrzeżenie niedoręczone” (E99) w kontrakcie modułu powiadomienia

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E7](epiki/E7-plany-limity-i-retencja-24-miesiace.md) Plany, limity i retencja 24 miesiące |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, powiadomienia, MEM-E7-POWIADOMIENIA-03 |
| Moduł | powiadomienia |
| Warstwa | kontrakt |
| Rodzaj | kontrakt |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-POWIADOMIENIA-01](MEM-E0-POWIADOMIENIA-01-szkielet-modulu-powiadomienia-struktura-publiczny-.md); [MEM-E7-POWIADOMIENIA-01](MEM-E7-POWIADOMIENIA-01-wyslij-ostrzezenie-przed-przycieciem-c92.md) |
| Blokuje | [MEM-E7-PLANY-16](MEM-E7-PLANY-16-polityka-p16-zawsze-gdy-ostrzezenie-niedoreczone-w.md) |
| Kod roboczy | MEM-E7-POWIADOMIENIA-03 |
| Źródło | ES:E99; kontrakty zdarzeń; ADR 0006 (outbox) |

## Cel

Zdarzenie integracyjne „Ostrzeżenie niedoręczone” (E99) w kontrakcie modułu powiadomienia — część epiku E7 „Plany, limity i retencja 24 miesiące”.

## Kontekst

- Moduł **powiadomienia** — kluczowe pytanie: Czy wiadomość dotarła i do kogo nie wolno pisać?
- Czego moduł nie wie (nie wolno tego wymagać): sens biznesowy wiadomości poza kategorią
- Projekt modułu: model procesów modułu powiadomienia
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zdarzenie integracyjne „Ostrzeżenie niedoręczone” (E99) w kontrakcie modułu powiadomienia (źródło: ES:E99; kontrakty zdarzeń; ADR 0006 (outbox))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E7-PLANY-16)

## Kryteria akceptacji

- [ ] Zdarzenie „Ostrzeżenie niedoręczone” zapisywane w outboxie w tej samej transakcji co zmiana stanu. *Sprawdzenie: test integracyjny (awaria po zapisie nie gubi zdarzenia)*
- [ ] Ładunek zawiera tylko skutek (id zasobu, osoba, numer kolejny, czas), nie przyczynę. *Sprawdzenie: test kontraktu*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
