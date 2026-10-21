# MEM-E2-ARCHIWUM-02: Dodaj zdjęcia do wspomnienia (C13)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E2](epiki/E2-wspomnienia-na-prywatnej-mapie-tworzenie-kosz-usuw.md) Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, archiwum, MEM-E2-ARCHIWUM-02 |
| Moduł | archiwum |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-ARCHIWUM-01](MEM-E0-ARCHIWUM-01-szkielet-modulu-archiwum-struktura-publiczny-kontr.md); [MEM-E2-LIMITY-02](MEM-E2-LIMITY-02-write-port-limity-dla-archiwum-zarezerwuj-zwroc-pr.md) |
| Blokuje | [MEM-E3-ARCHIWUM-01](MEM-E3-ARCHIWUM-01-write-port-archiwum-dla-album-zapisz-tresc-z-numer.md); [MEM-E2-PLAT-02](MEM-E2-PLAT-02-ekran-s04-nowe-wspomnienie-kreator-miejsce-tresc.md) |
| Kod roboczy | MEM-E2-ARCHIWUM-02 |
| Źródło | ES:C13 — S1:§4 „Zdjęcia: 17” |

## Cel

Dodaj zdjęcia do wspomnienia (C13) — część epiku E2 „Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie”.

## Kontekst

- Moduł **archiwum** — kluczowe pytanie: Co ta osoba zapisała, gdzie i kiedy?
- Czego moduł nie wie (nie wolno tego wymagać): kto poza autorem widzi treść; w których albumach jest wspomnienie; plany i pieniądze; czy treść poszła do AI
- Projekt modułu: model procesów modułu archiwum
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Dodaj zdjęcia do wspomnienia (C13) (źródło: ES:C13 — S1:§4 „Zdjęcia: 17”)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E3-ARCHIWUM-01, MEM-E2-PLAT-02)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when Użytkownik (autor wspomnień) wykonuje „Dodaj zdjęcia do wspomnienia”, then powstaje „Zdjęcia dodane” (E14). *Sprawdzenie: test jednostkowy handlera*
- [ ] Given warunek odrzucenia, when „Dodaj zdjęcia do wspomnienia”, then „Limit przestrzeni przekroczony” (E15) i brak zapisu. *Sprawdzenie: test jednostkowy*
- [ ] Ponowienie tej samej operacji (ten sam klucz/zdarzenie) nie daje drugiego skutku; brak odpowiedzi drugiej strony kończy się stanem opisanym w kontraktach modułów. *Sprawdzenie: test integracyjny z podwójnym wywołaniem*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
