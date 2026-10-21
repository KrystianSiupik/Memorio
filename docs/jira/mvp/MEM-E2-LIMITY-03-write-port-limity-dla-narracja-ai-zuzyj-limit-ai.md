# MEM-E2-LIMITY-03: Write port limity → dla narracja-ai: zużyj limit AI

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E2](epiki/E2-wspomnienia-na-prywatnej-mapie-tworzenie-kosz-usuw.md) Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, limity, MEM-E2-LIMITY-03 |
| Moduł | limity |
| Warstwa | kontrakt |
| Rodzaj | kontrakt |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-LIMITY-01](MEM-E0-LIMITY-01-szkielet-modulu-limity-struktura-publiczny-kontrak.md); [MEM-E3-LIMITY-01](MEM-E3-LIMITY-01-zarezerwuj-jednostki-z-puli-c150.md) |
| Blokuje | [MEM-E2-NARRACJA-AI-01](MEM-E2-NARRACJA-AI-01-popros-ai-o-szkic-opisu-c15.md); [MEM-E6-NARRACJA-AI-03](MEM-E6-NARRACJA-AI-03-zamow-podsumowanie-okresu-c82.md) |
| Kod roboczy | MEM-E2-LIMITY-03 |
| Źródło | kontrakty modułów porty: narracja-ai → limity (write-port) C15, C82 |

## Cel

Write port limity → dla narracja-ai: zużyj limit AI — część epiku E2 „Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie”.

## Kontekst

- Moduł **limity** — kluczowe pytanie: Czy ta operacja zmieści się w puli i ile zostało?
- Czego moduł nie wie (nie wolno tego wymagać): skąd limit; kto płaci; czego dotyczy treść
- Projekt modułu: model procesów modułu limity
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Write port limity → dla narracja-ai: zużyj limit AI (źródło: kontrakty modułów porty: narracja-ai → limity (write-port) C15, C82)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E2-NARRACJA-AI-01, MEM-E6-NARRACJA-AI-03)

## Kryteria akceptacji

- [ ] Write port limity → dla narracja-ai: zużyj limit AI. *Sprawdzenie: test/skrypt w CI lub ręcznie na środowisku*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
