# MEM-E0-MIEJSCA-01: Szkielet modułu miejsca: struktura, publiczny kontrakt, testy architektury

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E0](epiki/E0-szkielet-end-to-end-i-platforma-etap-1.md) Szkielet end-to-end i platforma (etap 1) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, miejsca, MEM-E0-MIEJSCA-01 |
| Moduł | miejsca |
| Warstwa | szkielet modułu |
| Rodzaj | szkielet modułu |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-01](MEM-E0-PLAT-01-szkielet-modularnego-monolitu-net-14-modulow-kontr.md); [MEM-E0-PLAT-03](MEM-E0-PLAT-03-postgresql-postgis-schemat-i-rola-bazy-na-modul-im.md) |
| Blokuje | [MEM-E2-MIEJSCA-01](MEM-E2-MIEJSCA-01-wybierz-miejsce-z-podpowiedzi-c10.md); [MEM-E2-MIEJSCA-02](MEM-E2-MIEJSCA-02-dodaj-miejsce-wlasne-c11.md); [MEM-E2-MIEJSCA-03](MEM-E2-MIEJSCA-03-zapytanie-podpowiedzi-miejsc-openstreetmap-miejsca.md); [MEM-E2-MIEJSCA-04](MEM-E2-MIEJSCA-04-read-port-miejsca-dla-archiwum-odwolanie-do-miejsc.md) |
| Kod roboczy | MEM-E0-MIEJSCA-01 |
| Źródło | mapa modułów, karta miejsca; ADR 0001, 0004, 0019 |

## Cel

Szkielet modułu miejsca: struktura, publiczny kontrakt, testy architektury — część epiku E0 „Szkielet end-to-end i platforma (etap 1)”.

## Kontekst

- Moduł **miejsca** — kluczowe pytanie: Jakie to miejsce na świecie i jak autor je nazywa?
- Czego moduł nie wie (nie wolno tego wymagać): czyje wspomnienia wiszą pod miejscem i co zawierają
- Projekt modułu: model procesów modułu miejsca
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- schemat bazy i rola dla modułu miejsca (ADR 0004) (źródło: mapa modułów, karta miejsca; ADR 0001, 0004, 0019)
- warstwy Onion: Domain / Application / Infrastructure / API, zależności tylko do środka (ADR 0019) (źródło: mapa modułów, karta miejsca; ADR 0001, 0004, 0019)
- publiczny kontrakt modułu (tylko kontrakty eksportowane) (źródło: mapa modułów, karta miejsca; ADR 0001, 0004, 0019)
- test architektury: moduł nie importuje wnętrza innych modułów (ADR 0001) i kierunek warstw (ADR 0019) (źródło: mapa modułów, karta miejsca; ADR 0001, 0004, 0019)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E2-MIEJSCA-01, MEM-E2-MIEJSCA-02, MEM-E2-MIEJSCA-03)

## Kryteria akceptacji

- [ ] Moduł ma warstwy Onion i publiczny kontrakt; Domain nie zależy od Application/Infrastructure/API; inne moduły importują tylko kontrakt. *Sprawdzenie: test architektury (fixture łamiący regułę failuje)*
- [ ] Rola bazy modułu nie czyta cudzych schematów. *Sprawdzenie: test integracyjny uprawnień*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
