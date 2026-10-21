# MEM-E1-KONTO-08: Zdarzenie integracyjne „Konto usunięte” (E7) w kontrakcie modułu konto

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E1](epiki/E1-konto-rejestracja-16-usuniecie-z-oknem-30-dni.md) Konto: rejestracja 16+, usunięcie z oknem 30 dni |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, konto, MEM-E1-KONTO-08 |
| Moduł | konto |
| Warstwa | kontrakt |
| Rodzaj | kontrakt |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-KONTO-01](MEM-E0-KONTO-01-szkielet-modulu-konto-struktura-publiczny-kontrakt.md); [MEM-E1-KONTO-04](MEM-E1-KONTO-04-usun-konto-definitywnie-c4.md) |
| Blokuje | [MEM-E3-ALBUM-19](MEM-E3-ALBUM-19-polityka-p2-zawsze-gdy-konto-usuniete-wtedy-wycofa.md); [MEM-E1-ARCHIWUM-03](MEM-E1-ARCHIWUM-03-polityka-p35-zawsze-gdy-konto-usuniete-wtedy-usun-.md); [MEM-E9-ALBUM-08](MEM-E9-ALBUM-08-polityka-p37-zawsze-gdy-konto-wlasciciela-bez-nast.md) |
| Kod roboczy | MEM-E1-KONTO-08 |
| Źródło | ES:E7; kontrakty zdarzeń; ADR 0006 (outbox) |

## Cel

Zdarzenie integracyjne „Konto usunięte” (E7) w kontrakcie modułu konto — część epiku E1 „Konto: rejestracja 16+, usunięcie z oknem 30 dni”.

## Kontekst

- Moduł **konto** — kluczowe pytanie: Czy ta osoba jest aktywnym użytkownikiem i czy jej konto ma zostać?
- Czego moduł nie wie (nie wolno tego wymagać): co dokładnie wypadnie przy usunięciu (pyta inne konteksty)
- Projekt modułu: model procesów modułu konto
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zdarzenie integracyjne „Konto usunięte” (E7) w kontrakcie modułu konto (źródło: ES:E7; kontrakty zdarzeń; ADR 0006 (outbox))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E3-ALBUM-19, MEM-E1-ARCHIWUM-03, MEM-E9-ALBUM-08)

## Kryteria akceptacji

- [ ] Zdarzenie „Konto usunięte” zapisywane w outboxie w tej samej transakcji co zmiana stanu. *Sprawdzenie: test integracyjny (awaria po zapisie nie gubi zdarzenia)*
- [ ] Ładunek zawiera tylko skutek (id zasobu, osoba, numer kolejny, czas), nie przyczynę. *Sprawdzenie: test kontraktu*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
