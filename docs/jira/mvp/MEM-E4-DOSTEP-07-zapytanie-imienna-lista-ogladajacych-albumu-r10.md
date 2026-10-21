# MEM-E4-DOSTEP-07: Zapytanie: Imienna lista oglądających albumu (R10)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E4](epiki/E4-udostepnianie-linkiem-i-odbieranie-dostepu.md) Udostępnianie linkiem i odbieranie dostępu |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, dostep, MEM-E4-DOSTEP-07 |
| Moduł | dostep |
| Warstwa | infrastruktura (query handler) |
| Rodzaj | zwykły |
| Klasa problemu | T&P |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-DOSTEP-01](MEM-E0-DOSTEP-01-szkielet-modulu-dostep-struktura-publiczny-kontrak.md) |
| Blokuje | [MEM-E4-DOSTEP-03](MEM-E4-DOSTEP-03-uniewaznij-link-c62.md); [MEM-E4-DOSTEP-04](MEM-E4-DOSTEP-04-odbierz-dostep-osobie-c63.md); [MEM-E3-PLAT-03](MEM-E3-PLAT-03-ekran-s11-zarzadzanie-albumem-wlasciciel.md) |
| Kod roboczy | MEM-E4-DOSTEP-07 |
| Źródło | ES:R10 — S2:B2 |

## Cel

Zapytanie: Imienna lista oglądających albumu (R10) — część epiku E4 „Udostępnianie linkiem i odbieranie dostępu”.

## Kontekst

- Moduł **dostep** — kluczowe pytanie: Czy ta osoba może teraz zobaczyć tę treść?
- Czego moduł nie wie (nie wolno tego wymagać): dlaczego ktoś ma lub stracił dostęp; zawartość treści
- Projekt modułu: model procesów modułu dostep
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zapytanie: Imienna lista oglądających albumu (R10) (źródło: ES:R10 — S2:B2)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E4-DOSTEP-03, MEM-E4-DOSTEP-04, MEM-E3-PLAT-03)

## Kryteria akceptacji

- [ ] Given dane źródłowe, when zapytanie „Imienna lista oglądających albumu”, then wynik zawiera tylko treści dostępne dla pytającego (filtr Dostępu, ADR 0011). *Sprawdzenie: test integracyjny macierzy użytkownik × zasób*
- [ ] Projekcja nie ma logiki biznesowej i daje się odbudować z danych źródłowych. *Sprawdzenie: review + test przebudowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
