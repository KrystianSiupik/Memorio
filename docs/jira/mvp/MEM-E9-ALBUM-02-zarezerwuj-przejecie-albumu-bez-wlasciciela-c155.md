# MEM-E9-ALBUM-02: Zarezerwuj przejęcie albumu bez właściciela (C155)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E9](epiki/E9-sukcesja-i-przejecie-albumu-nastepca-albo-inny-ucz.md) Sukcesja i przejęcie albumu (następca albo inny uczestnik) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, album, MEM-E9-ALBUM-02 |
| Moduł | album |
| Warstwa | domena + aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | RC (rywalizacja o zasoby) |
| Priorytet | P0 |
| Estymacja | 3–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-ALBUM-01](MEM-E0-ALBUM-01-szkielet-modulu-album-struktura-publiczny-kontrakt.md); [MEM-E3-ALBUM-02](MEM-E3-ALBUM-02-zapros-osobe-do-albumu-c42.md); [MEM-E9-ALBUM-05](MEM-E9-ALBUM-05-zapytanie-album-bez-wlasciciela-stan-przejecia-czy.md) |
| Blokuje | [MEM-E3-PLAT-02](MEM-E3-PLAT-02-ekran-s10-album.md) |
| Kod roboczy | MEM-E9-ALBUM-02 |
| Źródło | ES:C155 — przegląd decyzji 2026-10-19 (D-004) |

## Cel

Zarezerwuj przejęcie albumu bez właściciela (C155) — część epiku E9 „Sukcesja i przejęcie albumu (następca albo inny uczestnik)”.

## Kontekst

- Moduł **album** — kluczowe pytanie: Kto współtworzy ten wspólny zbiór i co do niego wniósł?
- Czego moduł nie wie (nie wolno tego wymagać): jak działa wydawanie mediów i link; pieniądze (tylko limit miejsc); treść wspomnień; dlaczego album stracił właściciela
- Projekt modułu: model procesów modułu album
- Agregat AG1 „Skład albumu”
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zarezerwuj przejęcie albumu bez właściciela (C155) (źródło: ES:C155 — przegląd decyzji 2026-10-19 (D-004))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E3-PLAT-02)

## Kryteria akceptacji

- [ ] Given aktor widzi: Album bez właściciela: stan przejęcia (czy można przejąć, trwająca rezerwacja i do kiedy), when Uczestnik albumu wykonuje „Zarezerwuj przejęcie albumu bez właściciela”, then powstaje „Przejęcie albumu zarezerwowane (30 min)” (E165). *Sprawdzenie: test jednostkowy handlera i agregatu*
- [ ] Given warunek odrzucenia, when „Zarezerwuj przejęcie albumu bez właściciela”, then „Rezerwacja przejęcia odrzucona (album ma właściciela albo trwa inna rezerwacja)” (E166) i brak zapisu. *Sprawdzenie: test jednostkowy*
- [ ] Niezmiennik AG1 „Skład albumu”: Uczestnicy + zaproszenia niepotwierdzone ≤ limit miejsc z planu (S2:B1). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG1 „Skład albumu”: Powtórne zaproszenie tej samej osoby odświeża istniejące, nie zajmuje drugiego miejsca (S2:B1). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG1 „Skład albumu”: Wycofanie, odrzucenie, wyjście i usunięcie zwalniają miejsce natychmiast (S2:B1). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG1 „Skład albumu”: Najwyżej jedna aktywna rezerwacja przejęcia albumu bez właściciela (30 min); przejmuje tylko rezerwujący uczestnik po opłaceniu, w trakcie 24 miesięcy trybu tylko do odczytu (D-004). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Współbieżność: dwie równoległe komendy z macierzy konfliktów AG1 (aggregates.md) — jedna odrzucona albo stan spójny; strategia: blokada pesymistyczna wiersza albumu. *Sprawdzenie: test integracyjny z dwoma równoległymi transakcjami*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
