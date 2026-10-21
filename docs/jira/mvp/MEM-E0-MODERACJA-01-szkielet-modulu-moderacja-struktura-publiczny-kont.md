# MEM-E0-MODERACJA-01: Szkielet modułu moderacja: struktura, publiczny kontrakt, testy architektury

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E0](epiki/E0-szkielet-end-to-end-i-platforma-etap-1.md) Szkielet end-to-end i platforma (etap 1) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, moderacja, MEM-E0-MODERACJA-01 |
| Moduł | moderacja |
| Warstwa | szkielet modułu |
| Rodzaj | szkielet modułu |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-01](MEM-E0-PLAT-01-szkielet-modularnego-monolitu-net-14-modulow-kontr.md); [MEM-E0-PLAT-03](MEM-E0-PLAT-03-postgresql-postgis-schemat-i-rola-bazy-na-modul-im.md) |
| Blokuje | [MEM-E8-MODERACJA-01](MEM-E8-MODERACJA-01-zglos-tresc-c110.md); [MEM-E8-MODERACJA-02](MEM-E8-MODERACJA-02-rozstrzygnij-zgloszenie-c111.md); [MEM-E8-MODERACJA-03](MEM-E8-MODERACJA-03-odwolaj-sie-od-decyzji-c112.md); [MEM-E8-MODERACJA-04](MEM-E8-MODERACJA-04-ukryj-tresc-natychmiast-c113.md); [MEM-E8-MODERACJA-05](MEM-E8-MODERACJA-05-zabezpiecz-dane-konta-c114.md); [MEM-E8-MODERACJA-06](MEM-E8-MODERACJA-06-wydaj-dane-organowi-c115.md); [MEM-E8-MODERACJA-07](MEM-E8-MODERACJA-07-zapytanie-kolejka-zgloszen-r9.md); [MEM-E8-MODERACJA-08](MEM-E8-MODERACJA-08-zapytanie-moje-zgloszenia-i-decyzje-moderacji-z-uz.md); [MEM-E8-MODERACJA-09](MEM-E8-MODERACJA-09-polityka-p19-zawsze-gdy-przyjdzie-nakaz-usuniecia-.md); [MEM-E8-MODERACJA-10](MEM-E8-MODERACJA-10-polityka-p20-zawsze-gdy-przyjdzie-nakaz-wydania-lu.md); [MEM-E8-MODERACJA-11](MEM-E8-MODERACJA-11-zdarzenie-integracyjne-tresc-ukryta-e113-w-kontrak.md) |
| Kod roboczy | MEM-E0-MODERACJA-01 |
| Źródło | mapa modułów, karta moderacja; ADR 0001, 0004, 0019 |

## Cel

Szkielet modułu moderacja: struktura, publiczny kontrakt, testy architektury — część epiku E0 „Szkielet end-to-end i platforma (etap 1)”.

## Kontekst

- Moduł **moderacja** — kluczowe pytanie: Co z tą treścią robimy po zgłoszeniu lub nakazie, i w jakim terminie?
- Czego moduł nie wie (nie wolno tego wymagać): prywatna treść poza zgłoszonym zakresem; relacje w albumach
- Projekt modułu: model procesów modułu moderacja
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- schemat bazy i rola dla modułu moderacja (ADR 0004) (źródło: mapa modułów, karta moderacja; ADR 0001, 0004, 0019)
- warstwy Onion: Domain / Application / Infrastructure / API, zależności tylko do środka (ADR 0019) (źródło: mapa modułów, karta moderacja; ADR 0001, 0004, 0019)
- publiczny kontrakt modułu (tylko kontrakty eksportowane) (źródło: mapa modułów, karta moderacja; ADR 0001, 0004, 0019)
- test architektury: moduł nie importuje wnętrza innych modułów (ADR 0001) i kierunek warstw (ADR 0019) (źródło: mapa modułów, karta moderacja; ADR 0001, 0004, 0019)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E8-MODERACJA-01, MEM-E8-MODERACJA-02, MEM-E8-MODERACJA-03)

## Kryteria akceptacji

- [ ] Moduł ma warstwy Onion i publiczny kontrakt; Domain nie zależy od Application/Infrastructure/API; inne moduły importują tylko kontrakt. *Sprawdzenie: test architektury (fixture łamiący regułę failuje)*
- [ ] Rola bazy modułu nie czyta cudzych schematów. *Sprawdzenie: test integracyjny uprawnień*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
