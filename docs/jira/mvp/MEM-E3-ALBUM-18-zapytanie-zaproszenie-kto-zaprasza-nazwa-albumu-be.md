# MEM-E3-ALBUM-18: Zapytanie: Zaproszenie (kto zaprasza, nazwa albumu — bez treści) (R19)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E3](epiki/E3-album-wspoldzielony-sklad-wklady-zgody.md) Album współdzielony: skład, wkłady, zgody |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, album, MEM-E3-ALBUM-18 |
| Moduł | album |
| Warstwa | infrastruktura (query handler) |
| Rodzaj | zwykły |
| Klasa problemu | T&P |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-ALBUM-01](MEM-E0-ALBUM-01-szkielet-modulu-album-struktura-publiczny-kontrakt.md) |
| Blokuje | [MEM-E3-ALBUM-03](MEM-E3-ALBUM-03-przyjmij-zaproszenie-c44.md); [MEM-E3-ALBUM-04](MEM-E3-ALBUM-04-odrzuc-zaproszenie-c45.md); [MEM-E3-PLAT-06](MEM-E3-PLAT-06-ekran-s14-zaproszenie-do-albumu.md) |
| Kod roboczy | MEM-E3-ALBUM-18 |
| Źródło | ES:R19 — S2:B1 „Zaproszony nie widzi żadnej treści albumu do chwili przyjęcia”; dopisany przy modelowaniu ekranów 2026-10-17 (D-042) |

## Cel

Zapytanie: Zaproszenie (kto zaprasza, nazwa albumu — bez treści) (R19) — część epiku E3 „Album współdzielony: skład, wkłady, zgody”.

## Kontekst

- Moduł **album** — kluczowe pytanie: Kto współtworzy ten wspólny zbiór i co do niego wniósł?
- Czego moduł nie wie (nie wolno tego wymagać): jak działa wydawanie mediów i link; pieniądze (tylko limit miejsc); treść wspomnień; dlaczego album stracił właściciela
- Projekt modułu: model procesów modułu album
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zapytanie: Zaproszenie (kto zaprasza, nazwa albumu — bez treści) (R19) (źródło: ES:R19 — S2:B1 „Zaproszony nie widzi żadnej treści albumu do chwili przyjęcia”; dopisany przy modelowaniu ekranów 2026-10-17 (D-042))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E3-ALBUM-03, MEM-E3-ALBUM-04, MEM-E3-PLAT-06)

## Kryteria akceptacji

- [ ] Given dane źródłowe, when zapytanie „Zaproszenie (kto zaprasza, nazwa albumu — bez treści)”, then wynik zawiera tylko treści dostępne dla pytającego (filtr Dostępu, ADR 0011). *Sprawdzenie: test integracyjny macierzy użytkownik × zasób*
- [ ] Projekcja nie ma logiki biznesowej i daje się odbudować z danych źródłowych. *Sprawdzenie: review + test przebudowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
