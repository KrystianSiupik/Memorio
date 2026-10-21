# MEM-E6-NARRACJA-AI-03: Zamów podsumowanie okresu (C82)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E6](epiki/E6-podsumuj-podsumowania-ai-bez-zmyslania-z-kaskada.md) Podsumuj: podsumowania AI bez zmyślania, z kaskadą |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, narracja-ai, MEM-E6-NARRACJA-AI-03 |
| Moduł | narracja-ai |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-NARRACJA-AI-01](MEM-E0-NARRACJA-AI-01-szkielet-modulu-narracja-ai-struktura-publiczny-ko.md); [MEM-E6-NARRACJA-AI-06](MEM-E6-NARRACJA-AI-06-zapytanie-materialy-dozwolone-do-podsumowania-zgod.md); [MEM-E6-DOSTEP-01](MEM-E6-DOSTEP-01-read-port-dostep-dla-narracja-ai-ktore-tresci-osob.md); [MEM-E2-LIMITY-03](MEM-E2-LIMITY-03-write-port-limity-dla-narracja-ai-zuzyj-limit-ai.md) |
| Blokuje | [MEM-E6-PLAT-01](MEM-E6-PLAT-01-ekran-s17-podsumowanie-okresu-ai.md) |
| Kod roboczy | MEM-E6-NARRACJA-AI-03 |
| Źródło | ES:C82 — S1:§9 |

## Cel

Zamów podsumowanie okresu (C82) — część epiku E6 „Podsumuj: podsumowania AI bez zmyślania, z kaskadą”.

## Kontekst

- Moduł **narracja-ai** — kluczowe pytanie: Jaką opowieść wolno opowiedzieć z tych materiałów, nie zmyślając?
- Czego moduł nie wie (nie wolno tego wymagać): trwałość artefaktu; dokładne nazwy i imiona po stronie dostawcy
- Projekt modułu: model procesów modułu narracja-ai
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zamów podsumowanie okresu (C82) (źródło: ES:C82 — S1:§9)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E6-PLAT-01)

## Kryteria akceptacji

- [ ] Given aktor widzi: Materiały dozwolone do podsumowania (zgody, uprawnienia), when Użytkownik (autor wspomnień) wykonuje „Zamów podsumowanie okresu”, then powstaje „Podsumowanie wygenerowane (lista źródeł, znacznik AI, pominięci uczestnicy)” (E82). *Sprawdzenie: test jednostkowy handlera*
- [ ] Given warunek odrzucenia, when „Zamów podsumowanie okresu”, then „Podsumowanie niedostępne (AI niedostępne lub pula AI wyczerpana)” (E83) i brak zapisu. *Sprawdzenie: test jednostkowy*
- [ ] Ponowienie tej samej operacji (ten sam klucz/zdarzenie) nie daje drugiego skutku; brak odpowiedzi drugiej strony kończy się stanem opisanym w kontraktach modułów. *Sprawdzenie: test integracyjny z podwójnym wywołaniem*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
