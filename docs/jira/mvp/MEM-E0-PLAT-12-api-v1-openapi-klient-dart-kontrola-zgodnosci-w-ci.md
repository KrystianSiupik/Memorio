# MEM-E0-PLAT-12: API v1: OpenAPI → klient Dart, kontrola zgodności w CI, minimalna wersja aplikacji

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E0](epiki/E0-szkielet-end-to-end-i-platforma-etap-1.md) Szkielet end-to-end i platforma (etap 1) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Platforma, -, MEM-E0-PLAT-12 |
| Moduł | - |
| Warstwa | infrastruktura |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-01](MEM-E0-PLAT-01-szkielet-modularnego-monolitu-net-14-modulow-kontr.md) |
| Blokuje | [MEM-E0-PLAT-19](MEM-E0-PLAT-19-nawigacja-aplikacji-zakladki-mapa-albumy-udostepni.md) |
| Kod roboczy | MEM-E0-PLAT-12 |
| Źródło | projekt systemu T12; SD17 |

## Cel

API v1: OpenAPI → klient Dart, kontrola zgodności w CI, minimalna wersja aplikacji — część epiku E0 „Szkielet end-to-end i platforma (etap 1)”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- API v1: OpenAPI → klient Dart, kontrola zgodności w CI, minimalna wersja aplikacji (źródło: projekt systemu T12; SD17)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E0-PLAT-19)

## Kryteria akceptacji

- [ ] API v1: OpenAPI → klient Dart, kontrola zgodności w CI, minimalna wersja aplikacji. *Sprawdzenie: test/skrypt w CI lub ręcznie na środowisku*
- [ ] Fitness function SD1: moduł nie importuje wnętrza innego modułu (tylko contracts). *Sprawdzenie: CI — test architektury (NetArchTest/ArchUnitNET)*
- [ ] Fitness function SD2: konwencje modułów (domain bez infrastruktury). *Sprawdzenie: CI — test architektury*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
