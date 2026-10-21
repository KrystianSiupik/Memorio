# MEM-E9-SUKCESJA-08: Zapytanie: Sprawa przejęcia albumu (album, termin, losy treści fundatora) (R17)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E9](epiki/E9-sukcesja-i-przejecie-albumu-nastepca-albo-inny-ucz.md) Sukcesja i przejęcie albumu (następca albo inny uczestnik) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, sukcesja, MEM-E9-SUKCESJA-08 |
| Moduł | sukcesja |
| Warstwa | infrastruktura (query handler) |
| Rodzaj | zwykły |
| Klasa problemu | T&P |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-SUKCESJA-01](MEM-E0-SUKCESJA-01-szkielet-modulu-sukcesja-struktura-publiczny-kontr.md) |
| Blokuje | [MEM-E9-SUKCESJA-04](MEM-E9-SUKCESJA-04-przekaz-album-nastepcy-c122.md); [MEM-E9-SUKCESJA-05](MEM-E9-SUKCESJA-05-odmow-przejecia-albumu-c123.md); [MEM-E9-PLAT-01](MEM-E9-PLAT-01-ekran-s23-przejecie-albumu-nastepca.md) |
| Kod roboczy | MEM-E9-SUKCESJA-08 |
| Źródło | ES:R17 — S2:B3 „Następca potwierdza przejęcie”; dopisany przy modelowaniu ekranów 2026-10-17 (D-042) |

## Cel

Zapytanie: Sprawa przejęcia albumu (album, termin, losy treści fundatora) (R17) — część epiku E9 „Sukcesja i przejęcie albumu (następca albo inny uczestnik)”.

## Kontekst

- Moduł **sukcesja** — kluczowe pytanie: Kto przejmuje album lub odczyt, gdy właściciela już nie ma?
- Czego moduł nie wie (nie wolno tego wymagać): budowa albumu; mechanika dostępu
- Projekt modułu: model procesów modułu sukcesja
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zapytanie: Sprawa przejęcia albumu (album, termin, losy treści fundatora) (R17) (źródło: ES:R17 — S2:B3 „Następca potwierdza przejęcie”; dopisany przy modelowaniu ekranów 2026-10-17 (D-042))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E9-SUKCESJA-04, MEM-E9-SUKCESJA-05, MEM-E9-PLAT-01)

## Kryteria akceptacji

- [ ] Given dane źródłowe, when zapytanie „Sprawa przejęcia albumu (album, termin, losy treści fundatora)”, then wynik zawiera tylko treści dostępne dla pytającego (filtr Dostępu, ADR 0011). *Sprawdzenie: test integracyjny macierzy użytkownik × zasób*
- [ ] Projekcja nie ma logiki biznesowej i daje się odbudować z danych źródłowych. *Sprawdzenie: review + test przebudowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
