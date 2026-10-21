# MEM-E3-ALBUM-26: Zdarzenie integracyjne „Uczestnik oznaczony we wspomnieniu” (E175) w kontrakcie modułu album

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E3](epiki/E3-album-wspoldzielony-sklad-wklady-zgody.md) Album współdzielony: skład, wkłady, zgody |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, album, MEM-E3-ALBUM-26 |
| Moduł | album |
| Warstwa | kontrakt |
| Rodzaj | kontrakt |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-ALBUM-01](MEM-E0-ALBUM-01-szkielet-modulu-album-struktura-publiczny-kontrakt.md); [MEM-E3-ALBUM-14](MEM-E3-ALBUM-14-oznacz-uczestnika-albumu-we-wspomnieniu-c169.md) |
| Blokuje | [MEM-E3-POWIADOMIENIA-10](MEM-E3-POWIADOMIENIA-10-polityka-p45-zawsze-gdy-uczestnik-oznaczony-wtedy-.md) |
| Kod roboczy | MEM-E3-ALBUM-26 |
| Źródło | ES:E175; kontrakty zdarzeń; ADR 0006 (outbox) |

## Cel

Zdarzenie integracyjne „Uczestnik oznaczony we wspomnieniu” (E175) w kontrakcie modułu album — część epiku E3 „Album współdzielony: skład, wkłady, zgody”.

## Kontekst

- Moduł **album** — kluczowe pytanie: Kto współtworzy ten wspólny zbiór i co do niego wniósł?
- Czego moduł nie wie (nie wolno tego wymagać): jak działa wydawanie mediów i link; pieniądze (tylko limit miejsc); treść wspomnień; dlaczego album stracił właściciela
- Projekt modułu: model procesów modułu album
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zdarzenie integracyjne „Uczestnik oznaczony we wspomnieniu” (E175) w kontrakcie modułu album (źródło: ES:E175; kontrakty zdarzeń; ADR 0006 (outbox))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E3-POWIADOMIENIA-10)

## Kryteria akceptacji

- [ ] Zdarzenie „Uczestnik oznaczony we wspomnieniu” zapisywane w outboxie w tej samej transakcji co zmiana stanu. *Sprawdzenie: test integracyjny (awaria po zapisie nie gubi zdarzenia)*
- [ ] Ładunek zawiera tylko skutek (id zasobu, osoba, numer kolejny, czas), nie przyczynę. *Sprawdzenie: test kontraktu*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
