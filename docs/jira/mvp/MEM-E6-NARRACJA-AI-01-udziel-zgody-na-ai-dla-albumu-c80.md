# MEM-E6-NARRACJA-AI-01: Udziel zgody na AI dla albumu (C80)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E6](epiki/E6-podsumuj-podsumowania-ai-bez-zmyslania-z-kaskada.md) Podsumuj: podsumowania AI bez zmyślania, z kaskadą |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, narracja-ai, MEM-E6-NARRACJA-AI-01 |
| Moduł | narracja-ai |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-NARRACJA-AI-01](MEM-E0-NARRACJA-AI-01-szkielet-modulu-narracja-ai-struktura-publiczny-ko.md) |
| Blokuje | [MEM-E3-PLAT-04](MEM-E3-PLAT-04-ekran-s12-moje-zgody-i-powierzenia-w-albumie.md) |
| Kod roboczy | MEM-E6-NARRACJA-AI-01 |
| Źródło | ES:C80 — S2:C5 „zgoda osobno dla każdego albumu”; S3:L19 (zgoda na przekazanie do AI) |

## Cel

Udziel zgody na AI dla albumu (C80) — część epiku E6 „Podsumuj: podsumowania AI bez zmyślania, z kaskadą”.

## Kontekst

- Moduł **narracja-ai** — kluczowe pytanie: Jaką opowieść wolno opowiedzieć z tych materiałów, nie zmyślając?
- Czego moduł nie wie (nie wolno tego wymagać): trwałość artefaktu; dokładne nazwy i imiona po stronie dostawcy
- Projekt modułu: model procesów modułu narracja-ai
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Udziel zgody na AI dla albumu (C80) (źródło: ES:C80 — S2:C5 „zgoda osobno dla każdego albumu”; S3:L19 (zgoda na przekazanie do AI))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E3-PLAT-04)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when Uczestnik albumu wykonuje „Udziel zgody na AI dla albumu”, then powstaje „Zgoda na AI udzielona dla albumu” (E80). *Sprawdzenie: test jednostkowy handlera*
- [ ] Niepoprawne dane wejściowe odrzucone walidacją z czytelnym komunikatem, bez zapisu. *Sprawdzenie: test jednostkowy schematu wejścia*

## Ryzyka i pytania

- HS-004: Uczestnik opuścił album z wyborem „zostaw”. Kto wtedy decyduje o zgodzie na AI dla jego treści (C5: zgoda per uczestnik i album)? Czy jego zgoda przeżywa wyjście z albumu? — PO
