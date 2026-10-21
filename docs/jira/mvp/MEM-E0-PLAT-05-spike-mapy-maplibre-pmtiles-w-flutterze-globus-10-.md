# MEM-E0-PLAT-05: Spike mapy: MapLibre + PMTiles w Flutterze (globus, 10 tys. punktów, web)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E0](epiki/E0-szkielet-end-to-end-i-platforma-etap-1.md) Szkielet end-to-end i platforma (etap 1) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Platforma, -, MEM-E0-PLAT-05 |
| Moduł | - |
| Warstwa | infrastruktura |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-01](MEM-E0-PLAT-01-szkielet-modularnego-monolitu-net-14-modulow-kontr.md) |
| Blokuje | - |
| Kod roboczy | MEM-E0-PLAT-05 |
| Źródło | projekt systemu T5; SD8, SD3, QS3 |

## Cel

Spike mapy: MapLibre + PMTiles w Flutterze (globus, 10 tys. punktów, web) — część epiku E0 „Szkielet end-to-end i platforma (etap 1)”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Spike mapy: MapLibre + PMTiles w Flutterze (globus, 10 tys. punktów, web) (źródło: projekt systemu T5; SD8, SD3, QS3)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Spike mapy: MapLibre + PMTiles w Flutterze (globus, 10 tys. punktów, web). *Sprawdzenie: test/skrypt w CI lub ręcznie na środowisku*
- [ ] Fitness function SD8: czas pierwszego obrazu mapy przy 10 tys. punktów. *Sprawdzenie: CI (test wydajności klienta) — scenariusz testowy na profilu syntetycznym*
- [ ] Fitness function SD3: rozmiar paczki web i czas pierwszego obrazu mapy. *Sprawdzenie: CI — test wydajności na profilu z 10 tys. punktów*
- [ ] Scenariusz QS3: p95 < 2 s do pierwszego obrazu mapy z punktami; płynne przewijanie. *Sprawdzenie: test wydajności / monitoring*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
