# MEM-E2-ARCHIWUM-13: Polityka P5: Zawsze gdy zdjęcia mają przypisane miejsce, wtedy utwórz z nich wspomnienie

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E2](epiki/E2-wspomnienia-na-prywatnej-mapie-tworzenie-kosz-usuw.md) Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, archiwum, MEM-E2-ARCHIWUM-13 |
| Moduł | archiwum |
| Warstwa | aplikacja (handler zdarzenia) |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P2 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-ARCHIWUM-01](MEM-E0-ARCHIWUM-01-szkielet-modulu-archiwum-struktura-publiczny-kontr.md); [MEM-E2-ARCHIWUM-01](MEM-E2-ARCHIWUM-01-utworz-wspomnienie-c12.md); [MEM-E10-ARCHIWUM-02](MEM-E10-ARCHIWUM-02-przypisz-miejsce-grupie-zdjec-c22.md) |
| Blokuje | - |
| Kod roboczy | MEM-E2-ARCHIWUM-13 |
| Źródło | ES:P5 — poczekalnia ma sens tylko, jeśli przypisanie kończy się wspomnieniem na mapie (S2:C6 „stan przejściowy do uzupełnienia”) |

## Cel

Polityka P5: Zawsze gdy zdjęcia mają przypisane miejsce, wtedy utwórz z nich wspomnienie — część epiku E2 „Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie”.

## Kontekst

- Moduł **archiwum** — kluczowe pytanie: Co ta osoba zapisała, gdzie i kiedy?
- Czego moduł nie wie (nie wolno tego wymagać): kto poza autorem widzi treść; w których albumach jest wspomnienie; plany i pieniądze; czy treść poszła do AI
- Projekt modułu: model procesów modułu archiwum
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Polityka P5: Zawsze gdy zdjęcia mają przypisane miejsce, wtedy utwórz z nich wspomnienie (źródło: ES:P5 — poczekalnia ma sens tylko, jeśli przypisanie kończy się wspomnieniem na mapie (S2:C6 „stan przejściowy do uzupełnienia”))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Given zdarzenie E33, when handler je przetwarza, then wywołana jest komenda C12. *Sprawdzenie: test integracyjny handlera*
- [ ] Podwójne dostarczenie zdarzenia nie daje drugiego skutku (inbox, ADR 0006). *Sprawdzenie: test idempotentności*
- [ ] Zdarzenie starsze niż ostatnio zastosowane dla tej samej pary (zasób, osoba) jest pomijane (D-013). *Sprawdzenie: test jednostkowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
