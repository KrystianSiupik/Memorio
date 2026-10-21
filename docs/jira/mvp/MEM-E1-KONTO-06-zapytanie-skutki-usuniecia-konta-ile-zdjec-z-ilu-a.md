# MEM-E1-KONTO-06: Zapytanie: Skutki usunięcia konta (ile zdjęć, z ilu albumów, czyich) (R2)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E1](epiki/E1-konto-rejestracja-16-usuniecie-z-oknem-30-dni.md) Konto: rejestracja 16+, usunięcie z oknem 30 dni |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, konto, MEM-E1-KONTO-06 |
| Moduł | konto |
| Warstwa | infrastruktura (query handler) |
| Rodzaj | zwykły |
| Klasa problemu | T&P |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-KONTO-01](MEM-E0-KONTO-01-szkielet-modulu-konto-struktura-publiczny-kontrakt.md) |
| Blokuje | [MEM-E1-KONTO-03](MEM-E1-KONTO-03-zlec-usuniecie-konta-c3.md); [MEM-E1-PLAT-02](MEM-E1-PLAT-02-ekran-s24-konto-i-prywatnosc.md) |
| Kod roboczy | MEM-E1-KONTO-06 |
| Źródło | ES:R2 — S2:B3 „pokazuje LICZBAMI, co wypadnie i skąd” |

## Cel

Zapytanie: Skutki usunięcia konta (ile zdjęć, z ilu albumów, czyich) (R2) — część epiku E1 „Konto: rejestracja 16+, usunięcie z oknem 30 dni”.

## Kontekst

- Moduł **konto** — kluczowe pytanie: Czy ta osoba jest aktywnym użytkownikiem i czy jej konto ma zostać?
- Czego moduł nie wie (nie wolno tego wymagać): co dokładnie wypadnie przy usunięciu (pyta inne konteksty)
- Projekt modułu: model procesów modułu konto
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zapytanie: Skutki usunięcia konta (ile zdjęć, z ilu albumów, czyich) (R2) (źródło: ES:R2 — S2:B3 „pokazuje LICZBAMI, co wypadnie i skąd”)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E1-KONTO-03, MEM-E1-PLAT-02)

## Kryteria akceptacji

- [ ] Given dane źródłowe, when zapytanie „Skutki usunięcia konta (ile zdjęć, z ilu albumów, czyich)”, then wynik zawiera tylko treści dostępne dla pytającego (filtr Dostępu, ADR 0011). *Sprawdzenie: test integracyjny macierzy użytkownik × zasób*
- [ ] Projekcja nie ma logiki biznesowej i daje się odbudować z danych źródłowych. *Sprawdzenie: review + test przebudowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
