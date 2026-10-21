# MEM-E3-ALBUM-02: Zaproś osobę do albumu (C42)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E3](epiki/E3-album-wspoldzielony-sklad-wklady-zgody.md) Album współdzielony: skład, wkłady, zgody |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, album, MEM-E3-ALBUM-02 |
| Moduł | album |
| Warstwa | domena + aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | RC (rywalizacja o zasoby) |
| Priorytet | P0 |
| Estymacja | 3–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-ALBUM-01](MEM-E0-ALBUM-01-szkielet-modulu-album-struktura-publiczny-kontrakt.md); [MEM-E3-ALBUM-16](MEM-E3-ALBUM-16-zapytanie-miejsca-w-planie-zajete-zaproszone-wolne.md); [MEM-E3-PLANY-01](MEM-E3-PLANY-01-read-port-plany-dla-album-limit-miejsc-dla-albumu.md); [MEM-E3-POWIADOMIENIA-06](MEM-E3-POWIADOMIENIA-06-read-port-powiadomienia-dla-album-czy-adres-zablok.md) |
| Blokuje | [MEM-E3-ALBUM-03](MEM-E3-ALBUM-03-przyjmij-zaproszenie-c44.md); [MEM-E3-ALBUM-04](MEM-E3-ALBUM-04-odrzuc-zaproszenie-c45.md); [MEM-E3-ALBUM-09](MEM-E3-ALBUM-09-opusc-album-c52.md); [MEM-E3-ALBUM-10](MEM-E3-ALBUM-10-usun-uczestnika-z-albumu-c53.md); [MEM-E9-ALBUM-01](MEM-E9-ALBUM-01-zwolnij-role-wlasciciela-albumu-c153.md); [MEM-E9-ALBUM-02](MEM-E9-ALBUM-02-zarezerwuj-przejecie-albumu-bez-wlasciciela-c155.md); [MEM-E9-ALBUM-03](MEM-E9-ALBUM-03-przejmij-album-bez-wlasciciela-c156.md); [MEM-E9-ALBUM-04](MEM-E9-ALBUM-04-wygas-rezerwacje-przejecia-c157.md); [MEM-E3-PLAT-03](MEM-E3-PLAT-03-ekran-s11-zarzadzanie-albumem-wlasciciel.md) |
| Kod roboczy | MEM-E3-ALBUM-02 |
| Źródło | ES:C42 — S2:B1 KOMENDA A; S2:C10 |

## Cel

Zaproś osobę do albumu (C42) — część epiku E3 „Album współdzielony: skład, wkłady, zgody”.

## Kontekst

- Moduł **album** — kluczowe pytanie: Kto współtworzy ten wspólny zbiór i co do niego wniósł?
- Czego moduł nie wie (nie wolno tego wymagać): jak działa wydawanie mediów i link; pieniądze (tylko limit miejsc); treść wspomnień; dlaczego album stracił właściciela
- Projekt modułu: model procesów modułu album
- Agregat AG1 „Skład albumu”
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zaproś osobę do albumu (C42) (źródło: ES:C42 — S2:B1 KOMENDA A; S2:C10)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E3-ALBUM-03, MEM-E3-ALBUM-04, MEM-E3-ALBUM-09)

## Kryteria akceptacji

- [ ] Given aktor widzi: Miejsca w planie: zajęte / zaproszone / wolne, when Właściciel albumu wykonuje „Zaproś osobę do albumu”, then powstaje „Zaproszenie wysłane (miejsce zajęte)” (E42). *Sprawdzenie: test jednostkowy handlera i agregatu*
- [ ] Given warunek odrzucenia, when „Zaproś osobę do albumu”, then „Zaproszenie odrzucone (brak wolnych miejsc)” (E43) i brak zapisu. *Sprawdzenie: test jednostkowy*
- [ ] Given warunek odrzucenia, when „Zaproś osobę do albumu”, then „Zaproszenie niedostarczalne (adres zablokowany)” (E44) i brak zapisu. *Sprawdzenie: test jednostkowy*
- [ ] Niezmiennik AG1 „Skład albumu”: Uczestnicy + zaproszenia niepotwierdzone ≤ limit miejsc z planu (S2:B1). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG1 „Skład albumu”: Powtórne zaproszenie tej samej osoby odświeża istniejące, nie zajmuje drugiego miejsca (S2:B1). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG1 „Skład albumu”: Wycofanie, odrzucenie, wyjście i usunięcie zwalniają miejsce natychmiast (S2:B1). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG1 „Skład albumu”: Najwyżej jedna aktywna rezerwacja przejęcia albumu bez właściciela (30 min); przejmuje tylko rezerwujący uczestnik po opłaceniu, w trakcie 24 miesięcy trybu tylko do odczytu (D-004). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Współbieżność: dwie równoległe komendy z macierzy konfliktów AG1 (aggregates.md) — jedna odrzucona albo stan spójny; strategia: blokada pesymistyczna wiersza albumu. *Sprawdzenie: test integracyjny z dwoma równoległymi transakcjami*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
