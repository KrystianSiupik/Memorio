# MEM-E7-PLANY-03: Odśwież stan subskrypcji u pośrednika (C96)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E7](epiki/E7-plany-limity-i-retencja-24-miesiace.md) Plany, limity i retencja 24 miesiące |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, plany, MEM-E7-PLANY-03 |
| Moduł | plany |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-PLANY-01](MEM-E0-PLANY-01-szkielet-modulu-plany-struktura-publiczny-kontrakt.md); [MEM-E7-PLANY-12](MEM-E7-PLANY-12-zapytanie-stan-planu-tryb-terminy-zuzycie-wybor-do.md) |
| Blokuje | [MEM-E7-PLAT-01](MEM-E7-PLAT-01-ekran-s18-plan-i-subskrypcja.md) |
| Kod roboczy | MEM-E7-PLANY-03 |
| Źródło | ES:C96 — S2:C3 „Produkt musi umieć DOPYTAĆ pośrednika” |

## Cel

Odśwież stan subskrypcji u pośrednika (C96) — część epiku E7 „Plany, limity i retencja 24 miesiące”.

## Kontekst

- Moduł **plany** — kluczowe pytanie: W jakim stanie jest teraz plan tego konta lub albumu i co z tego wynika?
- Czego moduł nie wie (nie wolno tego wymagać): co jest w archiwum; kto co wniósł; zgony, następcy, odmowy przejęcia (zna tylko „album bez właściciela”)
- Projekt modułu: model procesów modułu plany
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Odśwież stan subskrypcji u pośrednika (C96) (źródło: ES:C96 — S2:C3 „Produkt musi umieć DOPYTAĆ pośrednika”)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E7-PLAT-01)

## Kryteria akceptacji

- [ ] Given aktor widzi: Stan planu (tryb, terminy, zużycie, wybór do zachowania), when Użytkownik (autor wspomnień) wykonuje „Odśwież stan subskrypcji u pośrednika”, then powstaje „Stan subskrypcji zsynchronizowany” (E104). *Sprawdzenie: test jednostkowy handlera*
- [ ] Ponowienie tej samej operacji (ten sam klucz/zdarzenie) nie daje drugiego skutku; brak odpowiedzi drugiej strony kończy się stanem opisanym w kontraktach modułów. *Sprawdzenie: test integracyjny z podwójnym wywołaniem*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
