# MEM-E9-SUKCESJA-11: Zdarzenie integracyjne „Album prywatny udostępniony kontaktowi spadkowemu” (E126) w kontrakcie modułu sukcesja

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E9](epiki/E9-sukcesja-i-przejecie-albumu-nastepca-albo-inny-ucz.md) Sukcesja i przejęcie albumu (następca albo inny uczestnik) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, sukcesja, MEM-E9-SUKCESJA-11 |
| Moduł | sukcesja |
| Warstwa | kontrakt |
| Rodzaj | kontrakt |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-SUKCESJA-01](MEM-E0-SUKCESJA-01-szkielet-modulu-sukcesja-struktura-publiczny-kontr.md); [MEM-E9-SUKCESJA-06](MEM-E9-SUKCESJA-06-udostepnij-prywatny-album-kontaktowi-spadkowemu-c1.md) |
| Blokuje | [MEM-E4-DOSTEP-11](MEM-E4-DOSTEP-11-polityka-p31-zawsze-gdy-album-prywatny-udostepnion.md) |
| Kod roboczy | MEM-E9-SUKCESJA-11 |
| Źródło | ES:E126; kontrakty zdarzeń; ADR 0006 (outbox) |

## Cel

Zdarzenie integracyjne „Album prywatny udostępniony kontaktowi spadkowemu” (E126) w kontrakcie modułu sukcesja — część epiku E9 „Sukcesja i przejęcie albumu (następca albo inny uczestnik)”.

## Kontekst

- Moduł **sukcesja** — kluczowe pytanie: Kto przejmuje album lub odczyt, gdy właściciela już nie ma?
- Czego moduł nie wie (nie wolno tego wymagać): budowa albumu; mechanika dostępu
- Projekt modułu: model procesów modułu sukcesja
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zdarzenie integracyjne „Album prywatny udostępniony kontaktowi spadkowemu” (E126) w kontrakcie modułu sukcesja (źródło: ES:E126; kontrakty zdarzeń; ADR 0006 (outbox))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E4-DOSTEP-11)

## Kryteria akceptacji

- [ ] Zdarzenie „Album prywatny udostępniony kontaktowi spadkowemu” zapisywane w outboxie w tej samej transakcji co zmiana stanu. *Sprawdzenie: test integracyjny (awaria po zapisie nie gubi zdarzenia)*
- [ ] Ładunek zawiera tylko skutek (id zasobu, osoba, numer kolejny, czas), nie przyczynę. *Sprawdzenie: test kontraktu*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
