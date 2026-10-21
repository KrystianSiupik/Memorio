# MEM-E1-ALBUM-01: Read port album → dla konto: z ilu albumów, czy właściciel bez następcy

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E1](epiki/E1-konto-rejestracja-16-usuniecie-z-oknem-30-dni.md) Konto: rejestracja 16+, usunięcie z oknem 30 dni |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, album, MEM-E1-ALBUM-01 |
| Moduł | album |
| Warstwa | kontrakt |
| Rodzaj | kontrakt |
| Klasa problemu | T&P |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-ALBUM-01](MEM-E0-ALBUM-01-szkielet-modulu-album-struktura-publiczny-kontrakt.md) |
| Blokuje | [MEM-E1-KONTO-03](MEM-E1-KONTO-03-zlec-usuniecie-konta-c3.md) |
| Kod roboczy | MEM-E1-ALBUM-01 |
| Źródło | kontrakty modułów porty: konto → album (read-port) C3 |

## Cel

Read port album → dla konto: z ilu albumów, czy właściciel bez następcy — część epiku E1 „Konto: rejestracja 16+, usunięcie z oknem 30 dni”.

## Kontekst

- Moduł **album** — kluczowe pytanie: Kto współtworzy ten wspólny zbiór i co do niego wniósł?
- Czego moduł nie wie (nie wolno tego wymagać): jak działa wydawanie mediów i link; pieniądze (tylko limit miejsc); treść wspomnień; dlaczego album stracił właściciela
- Projekt modułu: model procesów modułu album
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Read port album → dla konto: z ilu albumów, czy właściciel bez następcy (źródło: kontrakty modułów porty: konto → album (read-port) C3)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E1-KONTO-03)

## Kryteria akceptacji

- [ ] Read port album → dla konto: z ilu albumów, czy właściciel bez następcy. *Sprawdzenie: test/skrypt w CI lub ręcznie na środowisku*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
