# MEM-E0-PLAT-17: Konfiguracja z ENV z walidacją przy starcie, health i readiness endpoint, kontener (RD1, RD2, RD4)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E0](epiki/E0-szkielet-end-to-end-i-platforma-etap-1.md) Szkielet end-to-end i platforma (etap 1) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Platforma, -, MEM-E0-PLAT-17 |
| Moduł | - |
| Warstwa | infrastruktura |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 2–3 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-01](MEM-E0-PLAT-01-szkielet-modularnego-monolitu-net-14-modulow-kontr.md) |
| Blokuje | - |
| Kod roboczy | MEM-E0-PLAT-17 |
| Źródło | gotowość infrastruktury RD1, RD2, RD4 |

## Cel

Konfiguracja z ENV z walidacją przy starcie, health i readiness endpoint, kontener (RD1, RD2, RD4) — część epiku E0 „Szkielet end-to-end i platforma (etap 1)”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- walidacja ENV przy starcie (nazwy zmiennych w błędach, nigdy wartości) (źródło: gotowość infrastruktury RD1, RD2, RD4)
- GET /health i /ready (źródło: gotowość infrastruktury RD1, RD2, RD4)
- jeden obraz kontenera na każde środowisko (źródło: gotowość infrastruktury RD1, RD2, RD4)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] walidacja ENV przy starcie (nazwy zmiennych w błędach, nigdy wartości). *Sprawdzenie: test/skrypt w CI lub ręcznie na środowisku*
- [ ] GET /health i /ready. *Sprawdzenie: test/skrypt w CI lub ręcznie na środowisku*
- [ ] jeden obraz kontenera na każde środowisko. *Sprawdzenie: test/skrypt w CI lub ręcznie na środowisku*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
