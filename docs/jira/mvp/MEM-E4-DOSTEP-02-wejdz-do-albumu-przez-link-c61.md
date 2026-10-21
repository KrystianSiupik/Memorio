# MEM-E4-DOSTEP-02: Wejdź do albumu przez link (C61)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E4](epiki/E4-udostepnianie-linkiem-i-odbieranie-dostepu.md) Udostępnianie linkiem i odbieranie dostępu |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, dostep, MEM-E4-DOSTEP-02 |
| Moduł | dostep |
| Warstwa | domena + aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | RC (rywalizacja o zasoby) |
| Priorytet | P0 |
| Estymacja | 3–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-DOSTEP-01](MEM-E0-DOSTEP-01-szkielet-modulu-dostep-struktura-publiczny-kontrak.md) |
| Blokuje | [MEM-E4-DOSTEP-03](MEM-E4-DOSTEP-03-uniewaznij-link-c62.md); [MEM-E4-DOSTEP-05](MEM-E4-DOSTEP-05-zablokuj-uzytkownika-c64.md); [MEM-E4-PLAT-01](MEM-E4-PLAT-01-ekran-s15-wejscie-do-albumu-przez-link.md) |
| Kod roboczy | MEM-E4-DOSTEP-02 |
| Źródło | ES:C61 — S2:B2 „Link otwiera album wyłącznie osobie zalogowanej” |

## Cel

Wejdź do albumu przez link (C61) — część epiku E4 „Udostępnianie linkiem i odbieranie dostępu”.

## Kontekst

- Moduł **dostep** — kluczowe pytanie: Czy ta osoba może teraz zobaczyć tę treść?
- Czego moduł nie wie (nie wolno tego wymagać): dlaczego ktoś ma lub stracił dostęp; zawartość treści
- Projekt modułu: model procesów modułu dostep
- Agregat AG3 „Oglądający albumu”
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Wejdź do albumu przez link (C61) (źródło: ES:C61 — S2:B2 „Link otwiera album wyłącznie osobie zalogowanej”)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E4-DOSTEP-03, MEM-E4-DOSTEP-05, MEM-E4-PLAT-01)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when Osoba zaproszona wykonuje „Wejdź do albumu przez link”, then powstaje „Oglądający dopisany do imiennej listy” (E63). *Sprawdzenie: test jednostkowy handlera i agregatu*
- [ ] Given warunek odrzucenia, when „Wejdź do albumu przez link”, then „Wejście odrzucone (link unieważniony)” (E64) i brak zapisu. *Sprawdzenie: test jednostkowy*
- [ ] Niezmiennik AG3 „Oglądający albumu”: Po unieważnieniu nikt nowy nie wejdzie — sprawdzenie i dopisanie w jednej transakcji (S2:B2, [U] 2026-10-16). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG3 „Oglądający albumu”: Powtórne wejście nie tworzy drugiego wpisu (S2:B2). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG3 „Oglądający albumu”: Zablokowany użytkownik nie wchodzi ani linkiem, ani przez zaproszenie (S2:C11, D-009, D-022). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Współbieżność: dwie równoległe komendy z macierzy konfliktów AG3 (aggregates.md) — jedna odrzucona albo stan spójny; strategia: blokada pesymistyczna wiersza albumu w Dostępie. *Sprawdzenie: test integracyjny z dwoma równoległymi transakcjami*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
