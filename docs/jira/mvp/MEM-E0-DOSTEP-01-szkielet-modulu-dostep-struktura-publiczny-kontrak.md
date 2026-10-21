# MEM-E0-DOSTEP-01: Szkielet modułu dostep: struktura, publiczny kontrakt, testy architektury

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E0](epiki/E0-szkielet-end-to-end-i-platforma-etap-1.md) Szkielet end-to-end i platforma (etap 1) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, dostep, MEM-E0-DOSTEP-01 |
| Moduł | dostep |
| Warstwa | szkielet modułu |
| Rodzaj | szkielet modułu |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-01](MEM-E0-PLAT-01-szkielet-modularnego-monolitu-net-14-modulow-kontr.md); [MEM-E0-PLAT-03](MEM-E0-PLAT-03-postgresql-postgis-schemat-i-rola-bazy-na-modul-im.md) |
| Blokuje | [MEM-E4-DOSTEP-01](MEM-E4-DOSTEP-01-udostepnij-album-linkiem-c60.md); [MEM-E4-DOSTEP-02](MEM-E4-DOSTEP-02-wejdz-do-albumu-przez-link-c61.md); [MEM-E4-DOSTEP-03](MEM-E4-DOSTEP-03-uniewaznij-link-c62.md); [MEM-E4-DOSTEP-04](MEM-E4-DOSTEP-04-odbierz-dostep-osobie-c63.md); [MEM-E4-DOSTEP-05](MEM-E4-DOSTEP-05-zablokuj-uzytkownika-c64.md); [MEM-E4-DOSTEP-06](MEM-E4-DOSTEP-06-przyznaj-odczyt-zakresu-c65.md); [MEM-E8-DOSTEP-01](MEM-E8-DOSTEP-01-ukryj-zakres-c67.md); [MEM-E4-DOSTEP-07](MEM-E4-DOSTEP-07-zapytanie-imienna-lista-ogladajacych-albumu-r10.md); [MEM-E3-DOSTEP-01](MEM-E3-DOSTEP-01-zapytanie-udostepnione-mi-albumy-z-odczytem-r14.md); [MEM-E6-DOSTEP-01](MEM-E6-DOSTEP-01-read-port-dostep-dla-narracja-ai-ktore-tresci-osob.md); [MEM-E8-DOSTEP-02](MEM-E8-DOSTEP-02-write-port-dostep-dla-moderacja-ukryj-zakres-natyc.md); [MEM-E4-DOSTEP-08](MEM-E4-DOSTEP-08-zdarzenie-integracyjne-dostep-odebrany-e66-w-kontr.md); [MEM-E4-DOSTEP-09](MEM-E4-DOSTEP-09-polityka-p29-zawsze-gdy-zaproszenie-przyjete-wtedy.md); [MEM-E4-DOSTEP-10](MEM-E4-DOSTEP-10-polityka-p30-zawsze-gdy-uczestnik-opuscil-album-al.md); [MEM-E4-DOSTEP-11](MEM-E4-DOSTEP-11-polityka-p31-zawsze-gdy-album-prywatny-udostepnion.md); [MEM-E8-DOSTEP-03](MEM-E8-DOSTEP-03-polityka-p32-zawsze-gdy-tresc-ukryta-decyzja-moder.md) |
| Kod roboczy | MEM-E0-DOSTEP-01 |
| Źródło | mapa modułów, karta dostep; ADR 0001, 0004, 0019 |

## Cel

Szkielet modułu dostep: struktura, publiczny kontrakt, testy architektury — część epiku E0 „Szkielet end-to-end i platforma (etap 1)”.

## Kontekst

- Moduł **dostep** — kluczowe pytanie: Czy ta osoba może teraz zobaczyć tę treść?
- Czego moduł nie wie (nie wolno tego wymagać): dlaczego ktoś ma lub stracił dostęp; zawartość treści
- Projekt modułu: model procesów modułu dostep
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- schemat bazy i rola dla modułu dostep (ADR 0004) (źródło: mapa modułów, karta dostep; ADR 0001, 0004, 0019)
- warstwy Onion: Domain / Application / Infrastructure / API, zależności tylko do środka (ADR 0019) (źródło: mapa modułów, karta dostep; ADR 0001, 0004, 0019)
- publiczny kontrakt modułu (tylko kontrakty eksportowane) (źródło: mapa modułów, karta dostep; ADR 0001, 0004, 0019)
- test architektury: moduł nie importuje wnętrza innych modułów (ADR 0001) i kierunek warstw (ADR 0019) (źródło: mapa modułów, karta dostep; ADR 0001, 0004, 0019)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E4-DOSTEP-01, MEM-E4-DOSTEP-02, MEM-E4-DOSTEP-03)

## Kryteria akceptacji

- [ ] Moduł ma warstwy Onion i publiczny kontrakt; Domain nie zależy od Application/Infrastructure/API; inne moduły importują tylko kontrakt. *Sprawdzenie: test architektury (fixture łamiący regułę failuje)*
- [ ] Rola bazy modułu nie czyta cudzych schematów. *Sprawdzenie: test integracyjny uprawnień*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
