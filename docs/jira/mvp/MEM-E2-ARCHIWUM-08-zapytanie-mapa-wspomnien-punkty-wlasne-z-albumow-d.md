# MEM-E2-ARCHIWUM-08: Zapytanie: Mapa wspomnień (punkty: własne + z albumów, do których mam dostęp; filtry dat, osób, albumów) (R11)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E2](epiki/E2-wspomnienia-na-prywatnej-mapie-tworzenie-kosz-usuw.md) Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, archiwum, MEM-E2-ARCHIWUM-08 |
| Moduł | archiwum |
| Warstwa | infrastruktura (query handler) |
| Rodzaj | zwykły |
| Klasa problemu | T&P |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-ARCHIWUM-01](MEM-E0-ARCHIWUM-01-szkielet-modulu-archiwum-struktura-publiczny-kontr.md) |
| Blokuje | [MEM-E2-PLAT-01](MEM-E2-PLAT-01-ekran-s03-mapa-i-globus.md) |
| Kod roboczy | MEM-E2-ARCHIWUM-08 |
| Źródło | ES:R11 — S1:§3 „Mapa pokazuje…”, „grupowanie bliskich punktów, filtry dat, kategorii, osób i albumów”; dopisany przy modelowaniu ekranów 2026-10-17 (D-042) |

## Cel

Zapytanie: Mapa wspomnień (punkty: własne + z albumów, do których mam dostęp; filtry dat, osób, albumów) (R11) — część epiku E2 „Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie”.

## Kontekst

- Moduł **archiwum** — kluczowe pytanie: Co ta osoba zapisała, gdzie i kiedy?
- Czego moduł nie wie (nie wolno tego wymagać): kto poza autorem widzi treść; w których albumach jest wspomnienie; plany i pieniądze; czy treść poszła do AI
- Projekt modułu: model procesów modułu archiwum
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zapytanie: Mapa wspomnień (punkty: własne + z albumów, do których mam dostęp; filtry dat, osób, albumów) (R11) (źródło: ES:R11 — S1:§3 „Mapa pokazuje…”, „grupowanie bliskich punktów, filtry dat, kategorii, osób i albumów”; dopisany przy modelowaniu ekranów 2026-10-17 (D-042))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E2-PLAT-01)

## Kryteria akceptacji

- [ ] Given dane źródłowe, when zapytanie „Mapa wspomnień (punkty: własne + z albumów, do których mam dostęp; filtry dat, osób, albumów)”, then wynik zawiera tylko treści dostępne dla pytającego (filtr Dostępu, ADR 0011). *Sprawdzenie: test integracyjny macierzy użytkownik × zasób*
- [ ] Projekcja nie ma logiki biznesowej i daje się odbudować z danych źródłowych. *Sprawdzenie: review + test przebudowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
