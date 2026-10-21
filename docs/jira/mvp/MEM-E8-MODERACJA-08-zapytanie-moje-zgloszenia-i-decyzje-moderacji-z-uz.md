# MEM-E8-MODERACJA-08: Zapytanie: Moje zgłoszenia i decyzje moderacji (z uzasadnieniem) (R18)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E8](epiki/E8-zgloszenia-moderacja-i-nakazy-organow.md) Zgłoszenia, moderacja i nakazy organów |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, moderacja, MEM-E8-MODERACJA-08 |
| Moduł | moderacja |
| Warstwa | infrastruktura (query handler) |
| Rodzaj | zwykły |
| Klasa problemu | T&P |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-MODERACJA-01](MEM-E0-MODERACJA-01-szkielet-modulu-moderacja-struktura-publiczny-kont.md) |
| Blokuje | [MEM-E8-MODERACJA-03](MEM-E8-MODERACJA-03-odwolaj-sie-od-decyzji-c112.md); [MEM-E8-PLAT-02](MEM-E8-PLAT-02-ekran-s22-moje-zgloszenia-i-decyzje.md) |
| Kod roboczy | MEM-E8-MODERACJA-08 |
| Źródło | ES:R18 — S2:C2 decyzja z uzasadnieniem, odwołanie; S6:L10; dopisany przy modelowaniu ekranów 2026-10-17 (D-042) |

## Cel

Zapytanie: Moje zgłoszenia i decyzje moderacji (z uzasadnieniem) (R18) — część epiku E8 „Zgłoszenia, moderacja i nakazy organów”.

## Kontekst

- Moduł **moderacja** — kluczowe pytanie: Co z tą treścią robimy po zgłoszeniu lub nakazie, i w jakim terminie?
- Czego moduł nie wie (nie wolno tego wymagać): prywatna treść poza zgłoszonym zakresem; relacje w albumach
- Projekt modułu: model procesów modułu moderacja
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zapytanie: Moje zgłoszenia i decyzje moderacji (z uzasadnieniem) (R18) (źródło: ES:R18 — S2:C2 decyzja z uzasadnieniem, odwołanie; S6:L10; dopisany przy modelowaniu ekranów 2026-10-17 (D-042))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E8-MODERACJA-03, MEM-E8-PLAT-02)

## Kryteria akceptacji

- [ ] Given dane źródłowe, when zapytanie „Moje zgłoszenia i decyzje moderacji (z uzasadnieniem)”, then wynik zawiera tylko treści dostępne dla pytającego (filtr Dostępu, ADR 0011). *Sprawdzenie: test integracyjny macierzy użytkownik × zasób*
- [ ] Projekcja nie ma logiki biznesowej i daje się odbudować z danych źródłowych. *Sprawdzenie: review + test przebudowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
