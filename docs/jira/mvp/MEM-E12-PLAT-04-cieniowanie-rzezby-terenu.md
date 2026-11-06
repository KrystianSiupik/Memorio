# MEM-E12-PLAT-04: Cieniowanie rzeźby terenu

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E12](epiki/E12-ui-mapy.md) UI mapy |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, Web, MEM-E12-PLAT-04 |
| Moduł | - |
| Warstwa | UI + infrastruktura |
| Rodzaj | zwykły |
| Klasa problemu | Transformacja i prezentacja |
| Priorytet | P1 |
| Estymacja | 4–8 h |
| Pewność | średnia |
| Status | czeka na decyzję PO |
| Zależy od | [MEM-E12-PLAT-03](MEM-E12-PLAT-03-szczegolowa-mapa-bazowa.md) |
| Blokuje | - |
| Kod roboczy | MEM-E12-PLAT-04 |
| Źródło | prośba PO z 2026-11-06 (pokaz aplikacji w przeglądarce) |

## Cel

Góry widać jako rzeźbę terenu (cieniowanie), nie tylko jako nazwy szczytów i lasy.

## Zakres

- Warstwa cieniowania z otwartego modelu wysokości (raster DEM w PMTiles), z własnego serwera
- Włączana w stylu, gdy plik jest na serwerze

## Poza zakresem

- widok 3D terenu

## Kryteria akceptacji

- [ ] Tatry w przeglądarce mają widoczny relief. *Sprawdzenie: ręcznie*

## Ryzyka i pytania

- **do decyzji PO:** dane wysokościowe dla Europy to dodatkowe dziesiątki GB na dysku i transfer przy każdym oglądaniu gór; można ograniczyć obszar (np. Polska) albo zoom
