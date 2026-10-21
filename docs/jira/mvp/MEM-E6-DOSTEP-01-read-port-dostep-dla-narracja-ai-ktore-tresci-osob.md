# MEM-E6-DOSTEP-01: Read port dostep → dla narracja-ai: które treści osoba może czytać teraz

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E6](epiki/E6-podsumuj-podsumowania-ai-bez-zmyslania-z-kaskada.md) Podsumuj: podsumowania AI bez zmyślania, z kaskadą |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, dostep, MEM-E6-DOSTEP-01 |
| Moduł | dostep |
| Warstwa | kontrakt |
| Rodzaj | kontrakt |
| Klasa problemu | T&P |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-DOSTEP-01](MEM-E0-DOSTEP-01-szkielet-modulu-dostep-struktura-publiczny-kontrak.md); [MEM-E4-DOSTEP-06](MEM-E4-DOSTEP-06-przyznaj-odczyt-zakresu-c65.md) |
| Blokuje | [MEM-E6-NARRACJA-AI-03](MEM-E6-NARRACJA-AI-03-zamow-podsumowanie-okresu-c82.md) |
| Kod roboczy | MEM-E6-DOSTEP-01 |
| Źródło | kontrakty modułów porty: narracja-ai → dostep (read-port) C82 |

## Cel

Read port dostep → dla narracja-ai: które treści osoba może czytać teraz — część epiku E6 „Podsumuj: podsumowania AI bez zmyślania, z kaskadą”.

## Kontekst

- Moduł **dostep** — kluczowe pytanie: Czy ta osoba może teraz zobaczyć tę treść?
- Czego moduł nie wie (nie wolno tego wymagać): dlaczego ktoś ma lub stracił dostęp; zawartość treści
- Projekt modułu: model procesów modułu dostep
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Read port dostep → dla narracja-ai: które treści osoba może czytać teraz (źródło: kontrakty modułów porty: narracja-ai → dostep (read-port) C82)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E6-NARRACJA-AI-03)

## Kryteria akceptacji

- [ ] Read port dostep → dla narracja-ai: które treści osoba może czytać teraz. *Sprawdzenie: test/skrypt w CI lub ręcznie na środowisku*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
