# MEM-E8-MODERACJA-09: Polityka P19: Zawsze gdy przyjdzie nakaz usunięcia, wtedy natychmiast ukryj treść

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E8](epiki/E8-zgloszenia-moderacja-i-nakazy-organow.md) Zgłoszenia, moderacja i nakazy organów |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, moderacja, MEM-E8-MODERACJA-09 |
| Moduł | moderacja |
| Warstwa | aplikacja (handler zdarzenia) |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-MODERACJA-01](MEM-E0-MODERACJA-01-szkielet-modulu-moderacja-struktura-publiczny-kont.md); [MEM-E8-MODERACJA-04](MEM-E8-MODERACJA-04-ukryj-tresc-natychmiast-c113.md) |
| Blokuje | - |
| Kod roboczy | MEM-E8-MODERACJA-09 |
| Źródło | ES:P19 — S3:L11; S2:C11 |

## Cel

Polityka P19: Zawsze gdy przyjdzie nakaz usunięcia, wtedy natychmiast ukryj treść — część epiku E8 „Zgłoszenia, moderacja i nakazy organów”.

## Kontekst

- Moduł **moderacja** — kluczowe pytanie: Co z tą treścią robimy po zgłoszeniu lub nakazie, i w jakim terminie?
- Czego moduł nie wie (nie wolno tego wymagać): prywatna treść poza zgłoszonym zakresem; relacje w albumach
- Projekt modułu: model procesów modułu moderacja
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Polityka P19: Zawsze gdy przyjdzie nakaz usunięcia, wtedy natychmiast ukryj treść (źródło: ES:P19 — S3:L11; S2:C11)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Given zdarzenie E115, when handler je przetwarza, then wywołana jest komenda C113. *Sprawdzenie: test integracyjny handlera*
- [ ] Podwójne dostarczenie zdarzenia nie daje drugiego skutku (inbox, ADR 0006). *Sprawdzenie: test idempotentności*
- [ ] Zdarzenie starsze niż ostatnio zastosowane dla tej samej pary (zasób, osoba) jest pomijane (D-013). *Sprawdzenie: test jednostkowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
