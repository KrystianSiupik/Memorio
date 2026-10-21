# MEM-E6-NARRACJA-AI-04: Zatwierdź i zapisz podsumowanie (C83)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E6](epiki/E6-podsumuj-podsumowania-ai-bez-zmyslania-z-kaskada.md) Podsumuj: podsumowania AI bez zmyślania, z kaskadą |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, narracja-ai, MEM-E6-NARRACJA-AI-04 |
| Moduł | narracja-ai |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-NARRACJA-AI-01](MEM-E0-NARRACJA-AI-01-szkielet-modulu-narracja-ai-struktura-publiczny-ko.md); [MEM-E6-POCHODZENIE-03](MEM-E6-POCHODZENIE-03-write-port-pochodzenie-dla-narracja-ai-zarejestruj.md) |
| Blokuje | [MEM-E6-PLAT-01](MEM-E6-PLAT-01-ekran-s17-podsumowanie-okresu-ai.md) |
| Kod roboczy | MEM-E6-NARRACJA-AI-04 |
| Źródło | ES:C83 — S2:A2 „zatwierdzenie przed zapisem”; S1:§9 „zapisane jako pamiątka” |

## Cel

Zatwierdź i zapisz podsumowanie (C83) — część epiku E6 „Podsumuj: podsumowania AI bez zmyślania, z kaskadą”.

## Kontekst

- Moduł **narracja-ai** — kluczowe pytanie: Jaką opowieść wolno opowiedzieć z tych materiałów, nie zmyślając?
- Czego moduł nie wie (nie wolno tego wymagać): trwałość artefaktu; dokładne nazwy i imiona po stronie dostawcy
- Projekt modułu: model procesów modułu narracja-ai
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zatwierdź i zapisz podsumowanie (C83) (źródło: ES:C83 — S2:A2 „zatwierdzenie przed zapisem”; S1:§9 „zapisane jako pamiątka”)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E6-PLAT-01)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when Użytkownik (autor wspomnień) wykonuje „Zatwierdź i zapisz podsumowanie”, then powstaje „Podsumowanie zapisane jako pamiątka” (E84). *Sprawdzenie: test jednostkowy handlera*
- [ ] Niepoprawne dane wejściowe odrzucone walidacją z czytelnym komunikatem, bez zapisu. *Sprawdzenie: test jednostkowy schematu wejścia*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
