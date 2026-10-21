# MEM-E9-ALBUM-01: Zwolnij rolę właściciela albumu (C153)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E9](epiki/E9-sukcesja-i-przejecie-albumu-nastepca-albo-inny-ucz.md) Sukcesja i przejęcie albumu (następca albo inny uczestnik) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, album, MEM-E9-ALBUM-01 |
| Moduł | album |
| Warstwa | domena + aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-ALBUM-01](MEM-E0-ALBUM-01-szkielet-modulu-album-struktura-publiczny-kontrakt.md); [MEM-E3-ALBUM-02](MEM-E3-ALBUM-02-zapros-osobe-do-albumu-c42.md) |
| Blokuje | [MEM-E9-ALBUM-06](MEM-E9-ALBUM-06-write-port-album-dla-sukcesja-zmien-wlasciciela-zw.md); [MEM-E9-ALBUM-07](MEM-E9-ALBUM-07-zdarzenie-integracyjne-album-bez-wlasciciela-e162-.md); [MEM-E9-ALBUM-08](MEM-E9-ALBUM-08-polityka-p37-zawsze-gdy-konto-wlasciciela-bez-nast.md) |
| Kod roboczy | MEM-E9-ALBUM-01 |
| Źródło | ES:C153 — S2:B3 „przy odmowie album wchodzi w stan »tylko do odczytu«”; „świadomej zgody na wygaśnięcie albumu”; S5 sonda HS-017 2026-10-17 (D-001) |

## Cel

Zwolnij rolę właściciela albumu (C153) — część epiku E9 „Sukcesja i przejęcie albumu (następca albo inny uczestnik)”.

## Kontekst

- Moduł **album** — kluczowe pytanie: Kto współtworzy ten wspólny zbiór i co do niego wniósł?
- Czego moduł nie wie (nie wolno tego wymagać): jak działa wydawanie mediów i link; pieniądze (tylko limit miejsc); treść wspomnień; dlaczego album stracił właściciela
- Projekt modułu: model procesów modułu album
- Agregat AG1 „Skład albumu”
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zwolnij rolę właściciela albumu (C153) (źródło: ES:C153 — S2:B3 „przy odmowie album wchodzi w stan »tylko do odczytu«”; „świadomej zgody na wygaśnięcie albumu”; S5 sonda HS-017 2026-10-17 (D-001))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E9-ALBUM-06, MEM-E9-ALBUM-07, MEM-E9-ALBUM-08)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when polityka/port wykonuje „Zwolnij rolę właściciela albumu”, then powstaje „Album bez właściciela” (E162). *Sprawdzenie: test jednostkowy handlera i agregatu*
- [ ] Niezmiennik AG1 „Skład albumu”: Uczestnicy + zaproszenia niepotwierdzone ≤ limit miejsc z planu (S2:B1). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG1 „Skład albumu”: Powtórne zaproszenie tej samej osoby odświeża istniejące, nie zajmuje drugiego miejsca (S2:B1). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG1 „Skład albumu”: Wycofanie, odrzucenie, wyjście i usunięcie zwalniają miejsce natychmiast (S2:B1). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG1 „Skład albumu”: Najwyżej jedna aktywna rezerwacja przejęcia albumu bez właściciela (30 min); przejmuje tylko rezerwujący uczestnik po opłaceniu, w trakcie 24 miesięcy trybu tylko do odczytu (D-004). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Ponowienie tej samej operacji (ten sam klucz/zdarzenie) nie daje drugiego skutku; brak odpowiedzi drugiej strony kończy się stanem opisanym w kontraktach modułów. *Sprawdzenie: test integracyjny z podwójnym wywołaniem*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
