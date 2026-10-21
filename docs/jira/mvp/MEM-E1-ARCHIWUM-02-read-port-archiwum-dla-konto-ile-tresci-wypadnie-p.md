# MEM-E1-ARCHIWUM-02: Read port archiwum → dla konto: ile treści wypadnie przy usunięciu konta

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E1](epiki/E1-konto-rejestracja-16-usuniecie-z-oknem-30-dni.md) Konto: rejestracja 16+, usunięcie z oknem 30 dni |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, archiwum, MEM-E1-ARCHIWUM-02 |
| Moduł | archiwum |
| Warstwa | kontrakt |
| Rodzaj | kontrakt |
| Klasa problemu | T&P |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-ARCHIWUM-01](MEM-E0-ARCHIWUM-01-szkielet-modulu-archiwum-struktura-publiczny-kontr.md) |
| Blokuje | [MEM-E1-KONTO-03](MEM-E1-KONTO-03-zlec-usuniecie-konta-c3.md) |
| Kod roboczy | MEM-E1-ARCHIWUM-02 |
| Źródło | kontrakty modułów porty: konto → archiwum (read-port) C3 |

## Cel

Read port archiwum → dla konto: ile treści wypadnie przy usunięciu konta — część epiku E1 „Konto: rejestracja 16+, usunięcie z oknem 30 dni”.

## Kontekst

- Moduł **archiwum** — kluczowe pytanie: Co ta osoba zapisała, gdzie i kiedy?
- Czego moduł nie wie (nie wolno tego wymagać): kto poza autorem widzi treść; w których albumach jest wspomnienie; plany i pieniądze; czy treść poszła do AI
- Projekt modułu: model procesów modułu archiwum
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Read port archiwum → dla konto: ile treści wypadnie przy usunięciu konta (źródło: kontrakty modułów porty: konto → archiwum (read-port) C3)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E1-KONTO-03)

## Kryteria akceptacji

- [ ] Read port archiwum → dla konto: ile treści wypadnie przy usunięciu konta. *Sprawdzenie: test/skrypt w CI lub ręcznie na środowisku*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
