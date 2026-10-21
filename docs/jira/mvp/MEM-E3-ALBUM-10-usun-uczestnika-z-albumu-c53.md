# MEM-E3-ALBUM-10: Usuń uczestnika z albumu (C53)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E3](epiki/E3-album-wspoldzielony-sklad-wklady-zgody.md) Album współdzielony: skład, wkłady, zgody |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, album, MEM-E3-ALBUM-10 |
| Moduł | album |
| Warstwa | domena + aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | RC (rywalizacja o zasoby) |
| Priorytet | P0 |
| Estymacja | 3–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-ALBUM-01](MEM-E0-ALBUM-01-szkielet-modulu-album-struktura-publiczny-kontrakt.md); [MEM-E3-ALBUM-02](MEM-E3-ALBUM-02-zapros-osobe-do-albumu-c42.md) |
| Blokuje | [MEM-E3-ALBUM-22](MEM-E3-ALBUM-22-zdarzenie-integracyjne-uczestnik-usuniety-z-albumu.md); [MEM-E3-PLAT-03](MEM-E3-PLAT-03-ekran-s11-zarzadzanie-albumem-wlasciciel.md) |
| Kod roboczy | MEM-E3-ALBUM-10 |
| Źródło | ES:C53 — S2:B1 KOMENDA C „Właściciel usuwający uczestnika NIE decyduje za niego” |

## Cel

Usuń uczestnika z albumu (C53) — część epiku E3 „Album współdzielony: skład, wkłady, zgody”.

## Kontekst

- Moduł **album** — kluczowe pytanie: Kto współtworzy ten wspólny zbiór i co do niego wniósł?
- Czego moduł nie wie (nie wolno tego wymagać): jak działa wydawanie mediów i link; pieniądze (tylko limit miejsc); treść wspomnień; dlaczego album stracił właściciela
- Projekt modułu: model procesów modułu album
- Agregat AG1 „Skład albumu”
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Usuń uczestnika z albumu (C53) (źródło: ES:C53 — S2:B1 KOMENDA C „Właściciel usuwający uczestnika NIE decyduje za niego”)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E3-ALBUM-22, MEM-E3-PLAT-03)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when Właściciel albumu wykonuje „Usuń uczestnika z albumu”, then powstaje „Uczestnik usunięty z albumu” (E59). *Sprawdzenie: test jednostkowy handlera i agregatu*
- [ ] Niezmiennik AG1 „Skład albumu”: Uczestnicy + zaproszenia niepotwierdzone ≤ limit miejsc z planu (S2:B1). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG1 „Skład albumu”: Powtórne zaproszenie tej samej osoby odświeża istniejące, nie zajmuje drugiego miejsca (S2:B1). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG1 „Skład albumu”: Wycofanie, odrzucenie, wyjście i usunięcie zwalniają miejsce natychmiast (S2:B1). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG1 „Skład albumu”: Najwyżej jedna aktywna rezerwacja przejęcia albumu bez właściciela (30 min); przejmuje tylko rezerwujący uczestnik po opłaceniu, w trakcie 24 miesięcy trybu tylko do odczytu (D-004). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Współbieżność: dwie równoległe komendy z macierzy konfliktów AG1 (aggregates.md) — jedna odrzucona albo stan spójny; strategia: blokada pesymistyczna wiersza albumu. *Sprawdzenie: test integracyjny z dwoma równoległymi transakcjami*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
