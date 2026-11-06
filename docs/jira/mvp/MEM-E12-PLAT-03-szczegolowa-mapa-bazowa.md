# MEM-E12-PLAT-03: Szczegółowa mapa bazowa: styl i wycinek na serwer

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E12](epiki/E12-ui-mapy.md) UI mapy |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, Web, MEM-E12-PLAT-03 |
| Moduł | - |
| Warstwa | UI + infrastruktura |
| Rodzaj | zwykły |
| Klasa problemu | Transformacja i prezentacja |
| Priorytet | P1 |
| Estymacja | 4–8 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | MEM-E0-PLAT-05 (spike mapy, D-193) |
| Blokuje | - |
| Kod roboczy | MEM-E12-PLAT-03 |
| Źródło | prośba PO z 2026-11-06 (pokaz aplikacji w przeglądarce) |

## Cel

Na produkcji mapa ma szczegóły do poziomu ulic: lasy, wody, drogi, szczyty z wysokością i nazwy po polsku, z własnego serwera (D-032).

## Zakres

- Styl mapy z warstwami schematu Protomaps: ląd, lasy i tereny, woda, granice, drogi, zabudowa, szczyty z wysokością, miejscowości; nazwy po polsku, gdy są, inaczej oryginalne
- Skrypt na serwerze: wycinek obszaru (domyślnie Europa) z wybranej wersji mapy Protomaps przez `pmtiles extract`, do katalogu serwowanego przez nginx obok aplikacji
- Lokalna próbka (Tatry i Warszawa do zoomu 14) do pokazu i testów wyglądu

## Poza zakresem

- cieniowanie rzeźby terenu (MEM-E12-PLAT-04)
- aktualizacje mapy według harmonogramu

## Kryteria akceptacji

- [ ] Mapa w przeglądarce pokazuje lasy, drogi i szczyty z wysokością w Tatrach na lokalnej próbce. *Sprawdzenie: ręcznie i zrzut ekranu*
- [ ] Skrypt pobiera wycinek o zadanych granicach i zoomie, sprawdza rozmiar i podmienia plik bez przerwy w działaniu. *Sprawdzenie: shellcheck i ręcznie*
- [ ] Nic z mapy nie idzie od zewnętrznego dostawcy w trakcie używania aplikacji. *Sprawdzenie: test pomiaru mapy (żądania tylko do własnego hosta)*

## Ryzyka i pytania

- rozmiar: Europa do zoomu 15 to kilkadziesiąt GB na dysku serwera; mniejszy obszar albo niższy zoom zmniejsza koszt (do decyzji PO przy wdrożeniu)
- licencja ODbL: podpis OpenStreetMap na mapie zostaje
