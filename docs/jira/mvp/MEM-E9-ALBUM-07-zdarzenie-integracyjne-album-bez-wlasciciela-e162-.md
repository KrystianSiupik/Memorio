# MEM-E9-ALBUM-07: Zdarzenie integracyjne „Album bez właściciela” (E162) w kontrakcie modułu album

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E9](epiki/E9-sukcesja-i-przejecie-albumu-nastepca-albo-inny-ucz.md) Sukcesja i przejęcie albumu (następca albo inny uczestnik) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, album, MEM-E9-ALBUM-07 |
| Moduł | album |
| Warstwa | kontrakt |
| Rodzaj | kontrakt |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-ALBUM-01](MEM-E0-ALBUM-01-szkielet-modulu-album-struktura-publiczny-kontrakt.md); [MEM-E9-ALBUM-01](MEM-E9-ALBUM-01-zwolnij-role-wlasciciela-albumu-c153.md) |
| Blokuje | [MEM-E7-PLANY-23](MEM-E7-PLANY-23-polityka-p36-zawsze-gdy-album-bez-wlasciciela-wted.md) |
| Kod roboczy | MEM-E9-ALBUM-07 |
| Źródło | ES:E162; kontrakty zdarzeń; ADR 0006 (outbox) |

## Cel

Zdarzenie integracyjne „Album bez właściciela” (E162) w kontrakcie modułu album — część epiku E9 „Sukcesja i przejęcie albumu (następca albo inny uczestnik)”.

## Kontekst

- Moduł **album** — kluczowe pytanie: Kto współtworzy ten wspólny zbiór i co do niego wniósł?
- Czego moduł nie wie (nie wolno tego wymagać): jak działa wydawanie mediów i link; pieniądze (tylko limit miejsc); treść wspomnień; dlaczego album stracił właściciela
- Projekt modułu: model procesów modułu album
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zdarzenie integracyjne „Album bez właściciela” (E162) w kontrakcie modułu album (źródło: ES:E162; kontrakty zdarzeń; ADR 0006 (outbox))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E7-PLANY-23)

## Kryteria akceptacji

- [ ] Zdarzenie „Album bez właściciela” zapisywane w outboxie w tej samej transakcji co zmiana stanu. *Sprawdzenie: test integracyjny (awaria po zapisie nie gubi zdarzenia)*
- [ ] Ładunek zawiera tylko skutek (id zasobu, osoba, numer kolejny, czas), nie przyczynę. *Sprawdzenie: test kontraktu*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
