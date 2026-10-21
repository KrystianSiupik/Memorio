# MEM-E1-KONTO-01: Zarejestruj konto (C1)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E1](epiki/E1-konto-rejestracja-16-usuniecie-z-oknem-30-dni.md) Konto: rejestracja 16+, usunięcie z oknem 30 dni |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, konto, MEM-E1-KONTO-01 |
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
| Kod roboczy | MEM-E1-KONTO-01 |
| Źródło | ES:C1 — S1:§15 pkt 1; S3:L05 (od 16 lat) |

## Cel

Zarejestruj konto (C1) — część epiku E1 „Konto: rejestracja 16+, usunięcie z oknem 30 dni”.

## Kontekst

- Moduł **konto** — kluczowe pytanie: Czy ta osoba jest aktywnym użytkownikiem i czy jej konto ma zostać?
- Czego moduł nie wie (nie wolno tego wymagać): co dokładnie wypadnie przy usunięciu (pyta inne konteksty)
- Projekt modułu: model procesów modułu konto
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zarejestruj konto (C1) (źródło: ES:C1 — S1:§15 pkt 1; S3:L05 (od 16 lat))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E1-PLAT-01)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when Użytkownik (autor wspomnień) wykonuje „Zarejestruj konto”, then powstaje „Konto założone” (E1). *Sprawdzenie: test jednostkowy handlera*
- [ ] Given warunek odrzucenia, when „Zarejestruj konto”, then „Rejestracja odrzucona (wiek poniżej 16 lat)” (E2) i brak zapisu. *Sprawdzenie: test jednostkowy*
- [ ] Niepoprawne dane wejściowe odrzucone walidacją z czytelnym komunikatem, bez zapisu. *Sprawdzenie: test jednostkowy schematu wejścia*

## Ryzyka i pytania

- HS-008: Wiek 16+: deklaracja przy rejestracji czy weryfikacja? Co z kontem, gdy okaże się, że osoba ma mniej lat? — PO + prawnik
