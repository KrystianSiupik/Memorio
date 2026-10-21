# MEM-E6-POCHODZENIE-03: Write port pochodzenie → dla narracja-ai: zarejestruj artefakt z listą źródeł (C154)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E6](epiki/E6-podsumuj-podsumowania-ai-bez-zmyslania-z-kaskada.md) Podsumuj: podsumowania AI bez zmyślania, z kaskadą |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, pochodzenie, MEM-E6-POCHODZENIE-03 |
| Moduł | pochodzenie |
| Warstwa | kontrakt |
| Rodzaj | kontrakt |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-POCHODZENIE-01](MEM-E0-POCHODZENIE-01-szkielet-modulu-pochodzenie-struktura-publiczny-ko.md); [MEM-E6-POCHODZENIE-02](MEM-E6-POCHODZENIE-02-zarejestruj-artefakt-c154.md) |
| Blokuje | [MEM-E6-NARRACJA-AI-04](MEM-E6-NARRACJA-AI-04-zatwierdz-i-zapisz-podsumowanie-c83.md) |
| Kod roboczy | MEM-E6-POCHODZENIE-03 |
| Źródło | kontrakty modułów porty: narracja-ai → pochodzenie (write-port) C83 |

## Cel

Write port pochodzenie → dla narracja-ai: zarejestruj artefakt z listą źródeł (C154) — część epiku E6 „Podsumuj: podsumowania AI bez zmyślania, z kaskadą”.

## Kontekst

- Moduł **pochodzenie** — kluczowe pytanie: Z czego powstał ten artefakt i czy nadal wolno mu istnieć?
- Czego moduł nie wie (nie wolno tego wymagać): przyczyna niedostępności źródła; treść artefaktu
- Projekt modułu: model procesów modułu pochodzenie
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Write port pochodzenie → dla narracja-ai: zarejestruj artefakt z listą źródeł (C154) (źródło: kontrakty modułów porty: narracja-ai → pochodzenie (write-port) C83)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E6-NARRACJA-AI-04)

## Kryteria akceptacji

- [ ] Write port pochodzenie → dla narracja-ai: zarejestruj artefakt z listą źródeł (C154). *Sprawdzenie: test/skrypt w CI lub ręcznie na środowisku*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
