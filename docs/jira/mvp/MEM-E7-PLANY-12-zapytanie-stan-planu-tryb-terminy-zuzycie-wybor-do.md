# MEM-E7-PLANY-12: Zapytanie: Stan planu (tryb, terminy, zużycie, wybór do zachowania) (R16)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E7](epiki/E7-plany-limity-i-retencja-24-miesiace.md) Plany, limity i retencja 24 miesiące |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, plany, MEM-E7-PLANY-12 |
| Moduł | plany |
| Warstwa | infrastruktura (query handler) |
| Rodzaj | zwykły |
| Klasa problemu | T&P |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-PLANY-01](MEM-E0-PLANY-01-szkielet-modulu-plany-struktura-publiczny-kontrakt.md) |
| Blokuje | [MEM-E7-PLANY-03](MEM-E7-PLANY-03-odswiez-stan-subskrypcji-u-posrednika-c96.md); [MEM-E7-PLANY-05](MEM-E7-PLANY-05-wskaz-material-do-zachowania-c94.md); [MEM-E7-PLAT-01](MEM-E7-PLAT-01-ekran-s18-plan-i-subskrypcja.md); [MEM-E7-PLAT-03](MEM-E7-PLAT-03-ekran-s19-wybierz-co-zostanie-po-przycieciu.md) |
| Kod roboczy | MEM-E7-PLANY-12 |
| Źródło | ES:R16 — S2:B1 Retencja, „właściciel wskazuje, co zostaje, w okienku 30 dni”; dopisany przy modelowaniu ekranów 2026-10-17 (D-042) |

## Cel

Zapytanie: Stan planu (tryb, terminy, zużycie, wybór do zachowania) (R16) — część epiku E7 „Plany, limity i retencja 24 miesiące”.

## Kontekst

- Moduł **plany** — kluczowe pytanie: W jakim stanie jest teraz plan tego konta lub albumu i co z tego wynika?
- Czego moduł nie wie (nie wolno tego wymagać): co jest w archiwum; kto co wniósł; zgony, następcy, odmowy przejęcia (zna tylko „album bez właściciela”)
- Projekt modułu: model procesów modułu plany
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zapytanie: Stan planu (tryb, terminy, zużycie, wybór do zachowania) (R16) (źródło: ES:R16 — S2:B1 Retencja, „właściciel wskazuje, co zostaje, w okienku 30 dni”; dopisany przy modelowaniu ekranów 2026-10-17 (D-042))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E7-PLANY-03, MEM-E7-PLANY-05, MEM-E7-PLAT-01)

## Kryteria akceptacji

- [ ] Given dane źródłowe, when zapytanie „Stan planu (tryb, terminy, zużycie, wybór do zachowania)”, then wynik zawiera tylko treści dostępne dla pytającego (filtr Dostępu, ADR 0011). *Sprawdzenie: test integracyjny macierzy użytkownik × zasób*
- [ ] Projekcja nie ma logiki biznesowej i daje się odbudować z danych źródłowych. *Sprawdzenie: review + test przebudowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
