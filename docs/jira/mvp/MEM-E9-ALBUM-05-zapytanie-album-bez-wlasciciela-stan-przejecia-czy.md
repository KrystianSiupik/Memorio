# MEM-E9-ALBUM-05: Zapytanie: Album bez właściciela: stan przejęcia (czy można przejąć, trwająca rezerwacja i do kiedy) (R20)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E9](epiki/E9-sukcesja-i-przejecie-albumu-nastepca-albo-inny-ucz.md) Sukcesja i przejęcie albumu (następca albo inny uczestnik) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, album, MEM-E9-ALBUM-05 |
| Moduł | album |
| Warstwa | infrastruktura (query handler) |
| Rodzaj | zwykły |
| Klasa problemu | T&P |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-ALBUM-01](MEM-E0-ALBUM-01-szkielet-modulu-album-struktura-publiczny-kontrakt.md) |
| Blokuje | [MEM-E9-ALBUM-02](MEM-E9-ALBUM-02-zarezerwuj-przejecie-albumu-bez-wlasciciela-c155.md); [MEM-E3-PLAT-02](MEM-E3-PLAT-02-ekran-s10-album.md) |
| Kod roboczy | MEM-E9-ALBUM-05 |
| Źródło | ES:R20 — przegląd decyzji 2026-10-19 (D-004) |

## Cel

Zapytanie: Album bez właściciela: stan przejęcia (czy można przejąć, trwająca rezerwacja i do kiedy) (R20) — część epiku E9 „Sukcesja i przejęcie albumu (następca albo inny uczestnik)”.

## Kontekst

- Moduł **album** — kluczowe pytanie: Kto współtworzy ten wspólny zbiór i co do niego wniósł?
- Czego moduł nie wie (nie wolno tego wymagać): jak działa wydawanie mediów i link; pieniądze (tylko limit miejsc); treść wspomnień; dlaczego album stracił właściciela
- Projekt modułu: model procesów modułu album
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zapytanie: Album bez właściciela: stan przejęcia (czy można przejąć, trwająca rezerwacja i do kiedy) (R20) (źródło: ES:R20 — przegląd decyzji 2026-10-19 (D-004))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E9-ALBUM-02, MEM-E3-PLAT-02)

## Kryteria akceptacji

- [ ] Given dane źródłowe, when zapytanie „Album bez właściciela: stan przejęcia (czy można przejąć, trwająca rezerwacja i do kiedy)”, then wynik zawiera tylko treści dostępne dla pytającego (filtr Dostępu, ADR 0011). *Sprawdzenie: test integracyjny macierzy użytkownik × zasób*
- [ ] Projekcja nie ma logiki biznesowej i daje się odbudować z danych źródłowych. *Sprawdzenie: review + test przebudowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
