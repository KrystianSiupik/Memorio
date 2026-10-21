# MEM-E1-KONTO-02: Zaakceptuj regulamin (C2)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E1](epiki/E1-konto-rejestracja-16-usuniecie-z-oknem-30-dni.md) Konto: rejestracja 16+, usunięcie z oknem 30 dni |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, konto, MEM-E1-KONTO-02 |
| Moduł | konto |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-KONTO-01](MEM-E0-KONTO-01-szkielet-modulu-konto-struktura-publiczny-kontrakt.md) |
| Blokuje | [MEM-E1-PLAT-01](MEM-E1-PLAT-01-ekran-s01-rejestracja.md) |
| Kod roboczy | MEM-E1-KONTO-02 |
| Źródło | ES:C2 — S2:C11 SKLEPY „regulamin akceptowany przed dodaniem pierwszej treści” |

## Cel

Zaakceptuj regulamin (C2) — część epiku E1 „Konto: rejestracja 16+, usunięcie z oknem 30 dni”.

## Kontekst

- Moduł **konto** — kluczowe pytanie: Czy ta osoba jest aktywnym użytkownikiem i czy jej konto ma zostać?
- Czego moduł nie wie (nie wolno tego wymagać): co dokładnie wypadnie przy usunięciu (pyta inne konteksty)
- Projekt modułu: model procesów modułu konto
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zaakceptuj regulamin (C2) (źródło: ES:C2 — S2:C11 SKLEPY „regulamin akceptowany przed dodaniem pierwszej treści”)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E1-PLAT-01)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when Użytkownik (autor wspomnień) wykonuje „Zaakceptuj regulamin”, then powstaje „Regulamin zaakceptowany” (E3). *Sprawdzenie: test jednostkowy handlera*
- [ ] Niepoprawne dane wejściowe odrzucone walidacją z czytelnym komunikatem, bez zapisu. *Sprawdzenie: test jednostkowy schematu wejścia*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
