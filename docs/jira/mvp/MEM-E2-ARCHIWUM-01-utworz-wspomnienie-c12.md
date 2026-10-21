# MEM-E2-ARCHIWUM-01: Utwórz wspomnienie (C12)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E2](epiki/E2-wspomnienia-na-prywatnej-mapie-tworzenie-kosz-usuw.md) Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, archiwum, MEM-E2-ARCHIWUM-01 |
| Moduł | archiwum |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-ARCHIWUM-01](MEM-E0-ARCHIWUM-01-szkielet-modulu-archiwum-struktura-publiczny-kontr.md); [MEM-E2-MIEJSCA-04](MEM-E2-MIEJSCA-04-read-port-miejsca-dla-archiwum-odwolanie-do-miejsc.md) |
| Blokuje | [MEM-E2-ARCHIWUM-13](MEM-E2-ARCHIWUM-13-polityka-p5-zawsze-gdy-zdjecia-maja-przypisane-mie.md); [MEM-E2-PLAT-02](MEM-E2-PLAT-02-ekran-s04-nowe-wspomnienie-kreator-miejsce-tresc.md) |
| Kod roboczy | MEM-E2-ARCHIWUM-01 |
| Źródło | ES:C12 — S1:§2, §4; S2:C1, C6 |

## Cel

Utwórz wspomnienie (C12) — część epiku E2 „Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie”.

## Kontekst

- Moduł **archiwum** — kluczowe pytanie: Co ta osoba zapisała, gdzie i kiedy?
- Czego moduł nie wie (nie wolno tego wymagać): kto poza autorem widzi treść; w których albumach jest wspomnienie; plany i pieniądze; czy treść poszła do AI
- Projekt modułu: model procesów modułu archiwum
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Utwórz wspomnienie (C12) (źródło: ES:C12 — S1:§2, §4; S2:C1, C6)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E2-ARCHIWUM-13, MEM-E2-PLAT-02)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when Użytkownik (autor wspomnień) wykonuje „Utwórz wspomnienie”, then powstaje „Wspomnienie utworzone” (E12). *Sprawdzenie: test jednostkowy handlera*
- [ ] Given warunek odrzucenia, when „Utwórz wspomnienie”, then „Wspomnienie odrzucone (data w przyszłości)” (E13) i brak zapisu. *Sprawdzenie: test jednostkowy*
- [ ] Niepoprawne dane wejściowe odrzucone walidacją z czytelnym komunikatem, bez zapisu. *Sprawdzenie: test jednostkowy schematu wejścia*

## Ryzyka i pytania

- HS-019: Wspólne zdarzenie wielu autorów (symulacja S2 przy wyznaczaniu granic modułów): kto ustala miejsce i datę pojemnika, gdy autorzy się różnią? — PO
