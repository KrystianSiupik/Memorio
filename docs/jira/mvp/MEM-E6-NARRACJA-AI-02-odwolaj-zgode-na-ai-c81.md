# MEM-E6-NARRACJA-AI-02: Odwołaj zgodę na AI (C81)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E6](epiki/E6-podsumuj-podsumowania-ai-bez-zmyslania-z-kaskada.md) Podsumuj: podsumowania AI bez zmyślania, z kaskadą |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, narracja-ai, MEM-E6-NARRACJA-AI-02 |
| Moduł | narracja-ai |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-NARRACJA-AI-01](MEM-E0-NARRACJA-AI-01-szkielet-modulu-narracja-ai-struktura-publiczny-ko.md) |
| Blokuje | [MEM-E6-NARRACJA-AI-07](MEM-E6-NARRACJA-AI-07-zdarzenie-integracyjne-zgoda-na-ai-odwolana-e81-w-.md); [MEM-E3-PLAT-04](MEM-E3-PLAT-04-ekran-s12-moje-zgody-i-powierzenia-w-albumie.md) |
| Kod roboczy | MEM-E6-NARRACJA-AI-02 |
| Źródło | ES:C81 — S2:C5 „Odwołanie zgody: wchodzi w kaskadę z B2” |

## Cel

Odwołaj zgodę na AI (C81) — część epiku E6 „Podsumuj: podsumowania AI bez zmyślania, z kaskadą”.

## Kontekst

- Moduł **narracja-ai** — kluczowe pytanie: Jaką opowieść wolno opowiedzieć z tych materiałów, nie zmyślając?
- Czego moduł nie wie (nie wolno tego wymagać): trwałość artefaktu; dokładne nazwy i imiona po stronie dostawcy
- Projekt modułu: model procesów modułu narracja-ai
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Odwołaj zgodę na AI (C81) (źródło: ES:C81 — S2:C5 „Odwołanie zgody: wchodzi w kaskadę z B2”)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E6-NARRACJA-AI-07, MEM-E3-PLAT-04)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when Uczestnik albumu wykonuje „Odwołaj zgodę na AI”, then powstaje „Zgoda na AI odwołana” (E81). *Sprawdzenie: test jednostkowy handlera*
- [ ] Niepoprawne dane wejściowe odrzucone walidacją z czytelnym komunikatem, bez zapisu. *Sprawdzenie: test jednostkowy schematu wejścia*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
