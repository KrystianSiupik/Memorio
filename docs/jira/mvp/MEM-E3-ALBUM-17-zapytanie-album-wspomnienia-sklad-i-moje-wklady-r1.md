# MEM-E3-ALBUM-17: Zapytanie: Album: wspomnienia, skład i moje wkłady (R13)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E3](epiki/E3-album-wspoldzielony-sklad-wklady-zgody.md) Album współdzielony: skład, wkłady, zgody |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, album, MEM-E3-ALBUM-17 |
| Moduł | album |
| Warstwa | infrastruktura (query handler) |
| Rodzaj | zwykły |
| Klasa problemu | T&P |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-ALBUM-01](MEM-E0-ALBUM-01-szkielet-modulu-album-struktura-publiczny-kontrakt.md) |
| Blokuje | [MEM-E3-ALBUM-09](MEM-E3-ALBUM-09-opusc-album-c52.md); [MEM-E3-ALBUM-11](MEM-E3-ALBUM-11-odwolaj-zgode-na-pozostawienie-wkladow-c55.md); [MEM-E3-ALBUM-14](MEM-E3-ALBUM-14-oznacz-uczestnika-albumu-we-wspomnieniu-c169.md); [MEM-E3-PLAT-02](MEM-E3-PLAT-02-ekran-s10-album.md); [MEM-E3-PLAT-04](MEM-E3-PLAT-04-ekran-s12-moje-zgody-i-powierzenia-w-albumie.md); [MEM-E3-PLAT-05](MEM-E3-PLAT-05-ekran-s13-powierzone-tresci-kurator.md) |
| Kod roboczy | MEM-E3-ALBUM-17 |
| Źródło | ES:R13 — S1:§5; S2:B1 KOMENDA C „z pokazaniem, czego dotyczy (ile zdjęć, wpisów)”; dopisany przy modelowaniu ekranów 2026-10-17 (D-042) |

## Cel

Zapytanie: Album: wspomnienia, skład i moje wkłady (R13) — część epiku E3 „Album współdzielony: skład, wkłady, zgody”.

## Kontekst

- Moduł **album** — kluczowe pytanie: Kto współtworzy ten wspólny zbiór i co do niego wniósł?
- Czego moduł nie wie (nie wolno tego wymagać): jak działa wydawanie mediów i link; pieniądze (tylko limit miejsc); treść wspomnień; dlaczego album stracił właściciela
- Projekt modułu: model procesów modułu album
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zapytanie: Album: wspomnienia, skład i moje wkłady (R13) (źródło: ES:R13 — S1:§5; S2:B1 KOMENDA C „z pokazaniem, czego dotyczy (ile zdjęć, wpisów)”; dopisany przy modelowaniu ekranów 2026-10-17 (D-042))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E3-ALBUM-09, MEM-E3-ALBUM-11, MEM-E3-ALBUM-14)

## Kryteria akceptacji

- [ ] Given dane źródłowe, when zapytanie „Album: wspomnienia, skład i moje wkłady”, then wynik zawiera tylko treści dostępne dla pytającego (filtr Dostępu, ADR 0011). *Sprawdzenie: test integracyjny macierzy użytkownik × zasób*
- [ ] Projekcja nie ma logiki biznesowej i daje się odbudować z danych źródłowych. *Sprawdzenie: review + test przebudowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
