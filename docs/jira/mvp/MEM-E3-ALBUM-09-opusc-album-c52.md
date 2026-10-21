# MEM-E3-ALBUM-09: Opuść album (C52)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E3](epiki/E3-album-wspoldzielony-sklad-wklady-zgody.md) Album współdzielony: skład, wkłady, zgody |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, album, MEM-E3-ALBUM-09 |
| Moduł | album |
| Warstwa | domena + aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | RC (rywalizacja o zasoby) |
| Priorytet | P0 |
| Estymacja | 3–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-ALBUM-01](MEM-E0-ALBUM-01-szkielet-modulu-album-struktura-publiczny-kontrakt.md); [MEM-E3-ALBUM-02](MEM-E3-ALBUM-02-zapros-osobe-do-albumu-c42.md); [MEM-E3-ALBUM-17](MEM-E3-ALBUM-17-zapytanie-album-wspomnienia-sklad-i-moje-wklady-r1.md) |
| Blokuje | [MEM-E3-ALBUM-23](MEM-E3-ALBUM-23-zdarzenie-integracyjne-wklady-wycofane-z-albumu-e5.md); [MEM-E3-ALBUM-25](MEM-E3-ALBUM-25-zdarzenie-integracyjne-uczestnik-opuscil-album-mie.md); [MEM-E3-PLAT-02](MEM-E3-PLAT-02-ekran-s10-album.md) |
| Kod roboczy | MEM-E3-ALBUM-09 |
| Źródło | ES:C52 — S2:B1 KOMENDA C „Jawny wybór »zostaw« / »zabierz«” |

## Cel

Opuść album (C52) — część epiku E3 „Album współdzielony: skład, wkłady, zgody”.

## Kontekst

- Moduł **album** — kluczowe pytanie: Kto współtworzy ten wspólny zbiór i co do niego wniósł?
- Czego moduł nie wie (nie wolno tego wymagać): jak działa wydawanie mediów i link; pieniądze (tylko limit miejsc); treść wspomnień; dlaczego album stracił właściciela
- Projekt modułu: model procesów modułu album
- Agregat AG1 „Skład albumu”
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Opuść album (C52) (źródło: ES:C52 — S2:B1 KOMENDA C „Jawny wybór »zostaw« / »zabierz«”)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E3-ALBUM-23, MEM-E3-ALBUM-25, MEM-E3-PLAT-02)

## Kryteria akceptacji

- [ ] Given aktor widzi: Album: wspomnienia, skład i moje wkłady, when Uczestnik albumu wykonuje „Opuść album”, then powstaje „Uczestnik opuścił album (miejsce zwolnione)” (E56), „Wkłady zostawione w albumie (zgoda)” (E57), „Wkłady wycofane z albumu” (E58). *Sprawdzenie: test jednostkowy handlera i agregatu*
- [ ] Niezmiennik AG1 „Skład albumu”: Uczestnicy + zaproszenia niepotwierdzone ≤ limit miejsc z planu (S2:B1). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG1 „Skład albumu”: Powtórne zaproszenie tej samej osoby odświeża istniejące, nie zajmuje drugiego miejsca (S2:B1). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG1 „Skład albumu”: Wycofanie, odrzucenie, wyjście i usunięcie zwalniają miejsce natychmiast (S2:B1). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG1 „Skład albumu”: Najwyżej jedna aktywna rezerwacja przejęcia albumu bez właściciela (30 min); przejmuje tylko rezerwujący uczestnik po opłaceniu, w trakcie 24 miesięcy trybu tylko do odczytu (D-004). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Współbieżność: dwie równoległe komendy z macierzy konfliktów AG1 (aggregates.md) — jedna odrzucona albo stan spójny; strategia: blokada pesymistyczna wiersza albumu. *Sprawdzenie: test integracyjny z dwoma równoległymi transakcjami*

## Ryzyka i pytania

- HS-004: Uczestnik opuścił album z wyborem „zostaw”. Kto wtedy decyduje o zgodzie na AI dla jego treści (C5: zgoda per uczestnik i album)? Czy jego zgoda przeżywa wyjście z albumu? — PO
