# MEM-E5-ARCHIWUM-01: Read port: Wspomnienia okresu (przecinające okres, w kolejności użytkownika) (R7)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E5](epiki/E5-przesledz-odtwarzanie-okresu-na-mapie.md) Prześledź: odtwarzanie okresu na mapie |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, archiwum, MEM-E5-ARCHIWUM-01 |
| Moduł | archiwum |
| Warstwa | kontrakt |
| Rodzaj | kontrakt |
| Klasa problemu | T&P |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-ARCHIWUM-01](MEM-E0-ARCHIWUM-01-szkielet-modulu-archiwum-struktura-publiczny-kontr.md) |
| Blokuje | [MEM-E5-ALBUM-01](MEM-E5-ALBUM-01-utworz-album-z-odtworzonego-okresu-c71.md); [MEM-E5-PLAT-01](MEM-E5-PLAT-01-ekran-s08-przesledz-okres.md) |
| Kod roboczy | MEM-E5-ARCHIWUM-01 |
| Źródło | ES:R7 — S2:C1 FILTR OKRESU, PORZĄDEK |

## Cel

Read port: Wspomnienia okresu (przecinające okres, w kolejności użytkownika) (R7) — część epiku E5 „Prześledź: odtwarzanie okresu na mapie”.

## Kontekst

- Moduł **archiwum** — kluczowe pytanie: Co ta osoba zapisała, gdzie i kiedy?
- Czego moduł nie wie (nie wolno tego wymagać): kto poza autorem widzi treść; w których albumach jest wspomnienie; plany i pieniądze; czy treść poszła do AI
- Projekt modułu: model procesów modułu archiwum
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Read port: Wspomnienia okresu (przecinające okres, w kolejności użytkownika) (R7) (źródło: ES:R7 — S2:C1 FILTR OKRESU, PORZĄDEK)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E5-ALBUM-01, MEM-E5-PLAT-01)

## Kryteria akceptacji

- [ ] Given dane źródłowe, when zapytanie „Wspomnienia okresu (przecinające okres, w kolejności użytkownika)”, then wynik zawiera tylko treści dostępne dla pytającego (filtr Dostępu, ADR 0011). *Sprawdzenie: test integracyjny macierzy użytkownik × zasób*
- [ ] Projekcja nie ma logiki biznesowej i daje się odbudować z danych źródłowych. *Sprawdzenie: review + test przebudowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
