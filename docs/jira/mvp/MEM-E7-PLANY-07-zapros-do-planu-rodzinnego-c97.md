# MEM-E7-PLANY-07: Zaproś do planu rodzinnego (C97)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E7](epiki/E7-plany-limity-i-retencja-24-miesiace.md) Plany, limity i retencja 24 miesiące |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, plany, MEM-E7-PLANY-07 |
| Moduł | plany |
| Warstwa | domena + aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | RC (rywalizacja o zasoby) |
| Priorytet | P2 |
| Estymacja | 3–4 h |
| Pewność | średnia |
| Status | zablokowany: HS-012: plan rodzinny w sklepie czy własny? (PO) |
| Zależy od | [MEM-E0-PLANY-01](MEM-E0-PLANY-01-szkielet-modulu-plany-struktura-publiczny-kontrakt.md) |
| Blokuje | [MEM-E7-PLANY-08](MEM-E7-PLANY-08-dolacz-do-planu-rodzinnego-c98.md); [MEM-E7-PLANY-09](MEM-E7-PLANY-09-usun-osobe-z-planu-rodzinnego-c99.md); [MEM-E7-PLAT-04](MEM-E7-PLAT-04-ekran-s20-plan-rodzinny.md) |
| Kod roboczy | MEM-E7-PLANY-07 |
| Źródło | ES:C97 — S2:C7 KOMENDA A |

## Cel

Zaproś do planu rodzinnego (C97) — część epiku E7 „Plany, limity i retencja 24 miesiące”.

## Kontekst

- Moduł **plany** — kluczowe pytanie: W jakim stanie jest teraz plan tego konta lub albumu i co z tego wynika?
- Czego moduł nie wie (nie wolno tego wymagać): co jest w archiwum; kto co wniósł; zgony, następcy, odmowy przejęcia (zna tylko „album bez właściciela”)
- Projekt modułu: model procesów modułu plany
- Agregat AG5 „Plan rodzinny”
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zaproś do planu rodzinnego (C97) (źródło: ES:C97 — S2:C7 KOMENDA A)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E7-PLANY-08, MEM-E7-PLANY-09, MEM-E7-PLAT-04)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when Płacący za plan rodzinny wykonuje „Zaproś do planu rodzinnego”, then powstaje „Zaproszenie do planu rodzinnego wysłane” (E105). *Sprawdzenie: test jednostkowy handlera i agregatu*
- [ ] Niezmiennik AG5 „Plan rodzinny”: Członkowie + zaproszenia ≤ limit planu (S2:C7). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG5 „Plan rodzinny”: Osoba w najwyżej jednym planie rodzinnym — ograniczenie unikalności w bazie, nie blokada (S2:C7). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Współbieżność: dwie równoległe komendy z macierzy konfliktów AG5 (aggregates.md) — jedna odrzucona albo stan spójny; strategia: blokada wiersza planu rodzinnego + unikalność „jeden plan na osobę”. *Sprawdzenie: test integracyjny z dwoma równoległymi transakcjami*

## Ryzyka i pytania

- HS-012: Plan rodzinny: sprzedawany przez sklepy (np. udostępnianie rodzinne w sklepie) czy własny mechanizm miejsc u pośrednika? Od tego zależy, kto emituje E106/E107. — PO
