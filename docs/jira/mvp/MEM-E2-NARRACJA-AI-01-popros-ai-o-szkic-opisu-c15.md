# MEM-E2-NARRACJA-AI-01: Poproś AI o szkic opisu (C15)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E2](epiki/E2-wspomnienia-na-prywatnej-mapie-tworzenie-kosz-usuw.md) Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, narracja-ai, MEM-E2-NARRACJA-AI-01 |
| Moduł | narracja-ai |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-NARRACJA-AI-01](MEM-E0-NARRACJA-AI-01-szkielet-modulu-narracja-ai-struktura-publiczny-ko.md); [MEM-E2-LIMITY-03](MEM-E2-LIMITY-03-write-port-limity-dla-narracja-ai-zuzyj-limit-ai.md) |
| Blokuje | [MEM-E2-PLAT-02](MEM-E2-PLAT-02-ekran-s04-nowe-wspomnienie-kreator-miejsce-tresc.md) |
| Kod roboczy | MEM-E2-NARRACJA-AI-01 |
| Źródło | ES:C15 — S1:§10 „poprosić AI o przygotowanie szkicu opisu”; S3:L13 (znakowanie) |

## Cel

Poproś AI o szkic opisu (C15) — część epiku E2 „Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie”.

## Kontekst

- Moduł **narracja-ai** — kluczowe pytanie: Jaką opowieść wolno opowiedzieć z tych materiałów, nie zmyślając?
- Czego moduł nie wie (nie wolno tego wymagać): trwałość artefaktu; dokładne nazwy i imiona po stronie dostawcy
- Projekt modułu: model procesów modułu narracja-ai
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Poproś AI o szkic opisu (C15) (źródło: ES:C15 — S1:§10 „poprosić AI o przygotowanie szkicu opisu”; S3:L13 (znakowanie))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E2-PLAT-02)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when Użytkownik (autor wspomnień) wykonuje „Poproś AI o szkic opisu”, then powstaje „Szkic opisu wygenerowany (oznaczony jako treść AI)” (E17). *Sprawdzenie: test jednostkowy handlera*
- [ ] Given warunek odrzucenia, when „Poproś AI o szkic opisu”, then „Szkic niedostępny (dostawca AI niedostępny lub limit AI wyczerpany)” (E18) i brak zapisu. *Sprawdzenie: test jednostkowy*
- [ ] Ponowienie tej samej operacji (ten sam klucz/zdarzenie) nie daje drugiego skutku; brak odpowiedzi drugiej strony kończy się stanem opisanym w kontraktach modułów. *Sprawdzenie: test integracyjny z podwójnym wywołaniem*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
