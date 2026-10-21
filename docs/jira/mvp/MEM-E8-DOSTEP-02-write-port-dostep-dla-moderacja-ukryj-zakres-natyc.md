# MEM-E8-DOSTEP-02: Write port dostep → dla moderacja: ukryj zakres natychmiast i potwierdź (nakaz 1 h)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E8](epiki/E8-zgloszenia-moderacja-i-nakazy-organow.md) Zgłoszenia, moderacja i nakazy organów |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, dostep, MEM-E8-DOSTEP-02 |
| Moduł | dostep |
| Warstwa | kontrakt |
| Rodzaj | kontrakt |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-DOSTEP-01](MEM-E0-DOSTEP-01-szkielet-modulu-dostep-struktura-publiczny-kontrak.md); [MEM-E8-DOSTEP-01](MEM-E8-DOSTEP-01-ukryj-zakres-c67.md) |
| Blokuje | [MEM-E8-MODERACJA-04](MEM-E8-MODERACJA-04-ukryj-tresc-natychmiast-c113.md) |
| Kod roboczy | MEM-E8-DOSTEP-02 |
| Źródło | kontrakty modułów porty: moderacja → dostep (write-port) C113 |

## Cel

Write port dostep → dla moderacja: ukryj zakres natychmiast i potwierdź (nakaz 1 h) — część epiku E8 „Zgłoszenia, moderacja i nakazy organów”.

## Kontekst

- Moduł **dostep** — kluczowe pytanie: Czy ta osoba może teraz zobaczyć tę treść?
- Czego moduł nie wie (nie wolno tego wymagać): dlaczego ktoś ma lub stracił dostęp; zawartość treści
- Projekt modułu: model procesów modułu dostep
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Write port dostep → dla moderacja: ukryj zakres natychmiast i potwierdź (nakaz 1 h) (źródło: kontrakty modułów porty: moderacja → dostep (write-port) C113)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E8-MODERACJA-04)

## Kryteria akceptacji

- [ ] Write port dostep → dla moderacja: ukryj zakres natychmiast i potwierdź (nakaz 1 h). *Sprawdzenie: test/skrypt w CI lub ręcznie na środowisku*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
