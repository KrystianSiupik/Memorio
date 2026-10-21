# MEM-E0-ALBUM-01: Szkielet modułu album: struktura, publiczny kontrakt, testy architektury

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E0](epiki/E0-szkielet-end-to-end-i-platforma-etap-1.md) Szkielet end-to-end i platforma (etap 1) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, album, MEM-E0-ALBUM-01 |
| Moduł | album |
| Warstwa | szkielet modułu |
| Rodzaj | szkielet modułu |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-01](MEM-E0-PLAT-01-szkielet-modularnego-monolitu-net-14-modulow-kontr.md); [MEM-E0-PLAT-03](MEM-E0-PLAT-03-postgresql-postgis-schemat-i-rola-bazy-na-modul-im.md) |
| Blokuje | [MEM-E3-ALBUM-01](MEM-E3-ALBUM-01-utworz-album-c40.md); [MEM-E3-ALBUM-02](MEM-E3-ALBUM-02-zapros-osobe-do-albumu-c42.md); [MEM-E3-ALBUM-03](MEM-E3-ALBUM-03-przyjmij-zaproszenie-c44.md); [MEM-E3-ALBUM-04](MEM-E3-ALBUM-04-odrzuc-zaproszenie-c45.md); [MEM-E3-ALBUM-05](MEM-E3-ALBUM-05-dodaj-wspomnienie-do-albumu-c47.md); [MEM-E3-ALBUM-06](MEM-E3-ALBUM-06-powierz-tresci-kuratorowi-c49.md); [MEM-E3-ALBUM-07](MEM-E3-ALBUM-07-odwolaj-powierzenie-c50.md); [MEM-E3-ALBUM-08](MEM-E3-ALBUM-08-uporzadkuj-powierzone-tresci-c51.md); [MEM-E3-ALBUM-09](MEM-E3-ALBUM-09-opusc-album-c52.md); [MEM-E3-ALBUM-10](MEM-E3-ALBUM-10-usun-uczestnika-z-albumu-c53.md); [MEM-E3-ALBUM-11](MEM-E3-ALBUM-11-odwolaj-zgode-na-pozostawienie-wkladow-c55.md); [MEM-E3-ALBUM-12](MEM-E3-ALBUM-12-wycofaj-wklady-osoby-z-albumow-c30.md); [MEM-E5-ALBUM-01](MEM-E5-ALBUM-01-utworz-album-z-odtworzonego-okresu-c71.md); [MEM-E3-ALBUM-13](MEM-E3-ALBUM-13-dodaj-zdjecia-do-wspomnienia-w-albumie-c56.md); [MEM-E9-ALBUM-01](MEM-E9-ALBUM-01-zwolnij-role-wlasciciela-albumu-c153.md); [MEM-E9-ALBUM-02](MEM-E9-ALBUM-02-zarezerwuj-przejecie-albumu-bez-wlasciciela-c155.md); [MEM-E9-ALBUM-03](MEM-E9-ALBUM-03-przejmij-album-bez-wlasciciela-c156.md); [MEM-E9-ALBUM-04](MEM-E9-ALBUM-04-wygas-rezerwacje-przejecia-c157.md); [MEM-E3-ALBUM-14](MEM-E3-ALBUM-14-oznacz-uczestnika-albumu-we-wspomnieniu-c169.md); [MEM-E3-ALBUM-15](MEM-E3-ALBUM-15-usun-swoje-oznaczenie-c170.md); [MEM-E3-ALBUM-16](MEM-E3-ALBUM-16-zapytanie-miejsca-w-planie-zajete-zaproszone-wolne.md); [MEM-E3-ALBUM-17](MEM-E3-ALBUM-17-zapytanie-album-wspomnienia-sklad-i-moje-wklady-r1.md); [MEM-E3-ALBUM-18](MEM-E3-ALBUM-18-zapytanie-zaproszenie-kto-zaprasza-nazwa-albumu-be.md); [MEM-E9-ALBUM-05](MEM-E9-ALBUM-05-zapytanie-album-bez-wlasciciela-stan-przejecia-czy.md); [MEM-E1-ALBUM-01](MEM-E1-ALBUM-01-read-port-album-dla-konto-z-ilu-albumow-czy-wlasci.md); [MEM-E9-ALBUM-06](MEM-E9-ALBUM-06-write-port-album-dla-sukcesja-zmien-wlasciciela-zw.md); [MEM-E3-ALBUM-19](MEM-E3-ALBUM-19-polityka-p2-zawsze-gdy-konto-usuniete-wtedy-wycofa.md); [MEM-E3-ALBUM-20](MEM-E3-ALBUM-20-zdarzenie-integracyjne-minelo-7-dni-od-zaproszenia.md); [MEM-E3-ALBUM-21](MEM-E3-ALBUM-21-zdarzenie-integracyjne-pula-albumu-wyczerpana-e51-.md); [MEM-E3-ALBUM-22](MEM-E3-ALBUM-22-zdarzenie-integracyjne-uczestnik-usuniety-z-albumu.md); [MEM-E3-ALBUM-23](MEM-E3-ALBUM-23-zdarzenie-integracyjne-wklady-wycofane-z-albumu-e5.md); [MEM-E3-ALBUM-24](MEM-E3-ALBUM-24-zdarzenie-integracyjne-zaproszenie-przyjete-e47-w-.md); [MEM-E3-ALBUM-25](MEM-E3-ALBUM-25-zdarzenie-integracyjne-uczestnik-opuscil-album-mie.md); [MEM-E9-ALBUM-07](MEM-E9-ALBUM-07-zdarzenie-integracyjne-album-bez-wlasciciela-e162-.md); [MEM-E9-ALBUM-08](MEM-E9-ALBUM-08-polityka-p37-zawsze-gdy-konto-wlasciciela-bez-nast.md); [MEM-E9-ALBUM-09](MEM-E9-ALBUM-09-polityka-p39-zawsze-gdy-minie-30-minut-rezerwacji-.md); [MEM-E9-ALBUM-10](MEM-E9-ALBUM-10-polityka-p40-zawsze-gdy-plan-albumu-bez-wlasciciel.md); [MEM-E9-ALBUM-11](MEM-E9-ALBUM-11-zdarzenie-integracyjne-album-przejety-przez-uczest.md); [MEM-E3-ALBUM-26](MEM-E3-ALBUM-26-zdarzenie-integracyjne-uczestnik-oznaczony-we-wspo.md) |
| Kod roboczy | MEM-E0-ALBUM-01 |
| Źródło | mapa modułów, karta album; ADR 0001, 0004, 0019 |

## Cel

Szkielet modułu album: struktura, publiczny kontrakt, testy architektury — część epiku E0 „Szkielet end-to-end i platforma (etap 1)”.

## Kontekst

- Moduł **album** — kluczowe pytanie: Kto współtworzy ten wspólny zbiór i co do niego wniósł?
- Czego moduł nie wie (nie wolno tego wymagać): jak działa wydawanie mediów i link; pieniądze (tylko limit miejsc); treść wspomnień; dlaczego album stracił właściciela
- Projekt modułu: model procesów modułu album
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- schemat bazy i rola dla modułu album (ADR 0004) (źródło: mapa modułów, karta album; ADR 0001, 0004, 0019)
- warstwy Onion: Domain / Application / Infrastructure / API, zależności tylko do środka (ADR 0019) (źródło: mapa modułów, karta album; ADR 0001, 0004, 0019)
- publiczny kontrakt modułu (tylko kontrakty eksportowane) (źródło: mapa modułów, karta album; ADR 0001, 0004, 0019)
- test architektury: moduł nie importuje wnętrza innych modułów (ADR 0001) i kierunek warstw (ADR 0019) (źródło: mapa modułów, karta album; ADR 0001, 0004, 0019)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E3-ALBUM-01, MEM-E3-ALBUM-02, MEM-E3-ALBUM-03)

## Kryteria akceptacji

- [ ] Moduł ma warstwy Onion i publiczny kontrakt; Domain nie zależy od Application/Infrastructure/API; inne moduły importują tylko kontrakt. *Sprawdzenie: test architektury (fixture łamiący regułę failuje)*
- [ ] Rola bazy modułu nie czyta cudzych schematów. *Sprawdzenie: test integracyjny uprawnień*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
