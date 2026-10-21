# MEM-E2-ARCHIWUM-04: Usuń swoją treść (C17)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E2](epiki/E2-wspomnienia-na-prywatnej-mapie-tworzenie-kosz-usuw.md) Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, archiwum, MEM-E2-ARCHIWUM-04 |
| Moduł | archiwum |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-ARCHIWUM-01](MEM-E0-ARCHIWUM-01-szkielet-modulu-archiwum-struktura-publiczny-kontr.md); [MEM-E2-ARCHIWUM-09](MEM-E2-ARCHIWUM-09-zapytanie-szczegoly-wspomnienia-miejsce-data-z-pre.md) |
| Blokuje | [MEM-E2-PLAT-03](MEM-E2-PLAT-03-ekran-s05-szczegoly-wspomnienia.md) |
| Kod roboczy | MEM-E2-ARCHIWUM-04 |
| Źródło | ES:C17 — S2:B3 KOMENDA A |

## Cel

Usuń swoją treść (C17) — część epiku E2 „Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie”.

## Kontekst

- Moduł **archiwum** — kluczowe pytanie: Co ta osoba zapisała, gdzie i kiedy?
- Czego moduł nie wie (nie wolno tego wymagać): kto poza autorem widzi treść; w których albumach jest wspomnienie; plany i pieniądze; czy treść poszła do AI
- Projekt modułu: model procesów modułu archiwum
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Usuń swoją treść (C17) (źródło: ES:C17 — S2:B3 KOMENDA A)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E2-PLAT-03)

## Kryteria akceptacji

- [ ] Given aktor widzi: Szczegóły wspomnienia (miejsce, data z precyzją, treści per autor, osoby: tekst autora albo oznaczeni uczestnicy albumu), when Użytkownik (autor wspomnień) wykonuje „Usuń swoją treść”, then powstaje „Treść przeniesiona do kosza” (E20). *Sprawdzenie: test jednostkowy handlera*
- [ ] Niepoprawne dane wejściowe odrzucone walidacją z czytelnym komunikatem, bez zapisu. *Sprawdzenie: test jednostkowy schematu wejścia*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
