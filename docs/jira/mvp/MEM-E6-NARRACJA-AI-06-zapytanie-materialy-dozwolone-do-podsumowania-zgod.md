# MEM-E6-NARRACJA-AI-06: Zapytanie: Materiały dozwolone do podsumowania (zgody, uprawnienia) (R8)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E6](epiki/E6-podsumuj-podsumowania-ai-bez-zmyslania-z-kaskada.md) Podsumuj: podsumowania AI bez zmyślania, z kaskadą |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, narracja-ai, MEM-E6-NARRACJA-AI-06 |
| Moduł | narracja-ai |
| Warstwa | infrastruktura (query handler) |
| Rodzaj | zwykły |
| Klasa problemu | T&P |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-NARRACJA-AI-01](MEM-E0-NARRACJA-AI-01-szkielet-modulu-narracja-ai-struktura-publiczny-ko.md) |
| Blokuje | [MEM-E6-NARRACJA-AI-03](MEM-E6-NARRACJA-AI-03-zamow-podsumowanie-okresu-c82.md); [MEM-E6-PLAT-01](MEM-E6-PLAT-01-ekran-s17-podsumowanie-okresu-ai.md) |
| Kod roboczy | MEM-E6-NARRACJA-AI-06 |
| Źródło | ES:R8 — S2:C5; S1:§9 „dostępne i dozwolone do wykorzystania” |

## Cel

Zapytanie: Materiały dozwolone do podsumowania (zgody, uprawnienia) (R8) — część epiku E6 „Podsumuj: podsumowania AI bez zmyślania, z kaskadą”.

## Kontekst

- Moduł **narracja-ai** — kluczowe pytanie: Jaką opowieść wolno opowiedzieć z tych materiałów, nie zmyślając?
- Czego moduł nie wie (nie wolno tego wymagać): trwałość artefaktu; dokładne nazwy i imiona po stronie dostawcy
- Projekt modułu: model procesów modułu narracja-ai
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zapytanie: Materiały dozwolone do podsumowania (zgody, uprawnienia) (R8) (źródło: ES:R8 — S2:C5; S1:§9 „dostępne i dozwolone do wykorzystania”)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E6-NARRACJA-AI-03, MEM-E6-PLAT-01)

## Kryteria akceptacji

- [ ] Given dane źródłowe, when zapytanie „Materiały dozwolone do podsumowania (zgody, uprawnienia)”, then wynik zawiera tylko treści dostępne dla pytającego (filtr Dostępu, ADR 0011). *Sprawdzenie: test integracyjny macierzy użytkownik × zasób*
- [ ] Projekcja nie ma logiki biznesowej i daje się odbudować z danych źródłowych. *Sprawdzenie: review + test przebudowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
