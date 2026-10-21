# MEM-E0-SUKCESJA-01: Szkielet modułu sukcesja: struktura, publiczny kontrakt, testy architektury

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E0](epiki/E0-szkielet-end-to-end-i-platforma-etap-1.md) Szkielet end-to-end i platforma (etap 1) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, sukcesja, MEM-E0-SUKCESJA-01 |
| Moduł | sukcesja |
| Warstwa | szkielet modułu |
| Rodzaj | szkielet modułu |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-01](MEM-E0-PLAT-01-szkielet-modularnego-monolitu-net-14-modulow-kontr.md); [MEM-E0-PLAT-03](MEM-E0-PLAT-03-postgresql-postgis-schemat-i-rola-bazy-na-modul-im.md) |
| Blokuje | [MEM-E3-SUKCESJA-01](MEM-E3-SUKCESJA-01-wskaz-nastepce-albumu-c41.md); [MEM-E9-SUKCESJA-01](MEM-E9-SUKCESJA-01-wskaz-kontakt-spadkowy-c124.md); [MEM-E9-SUKCESJA-02](MEM-E9-SUKCESJA-02-zglos-zgon-wlasciciela-c120.md); [MEM-E9-SUKCESJA-03](MEM-E9-SUKCESJA-03-zweryfikuj-akt-zgonu-c121.md); [MEM-E9-SUKCESJA-04](MEM-E9-SUKCESJA-04-przekaz-album-nastepcy-c122.md); [MEM-E9-SUKCESJA-05](MEM-E9-SUKCESJA-05-odmow-przejecia-albumu-c123.md); [MEM-E9-SUKCESJA-06](MEM-E9-SUKCESJA-06-udostepnij-prywatny-album-kontaktowi-spadkowemu-c1.md); [MEM-E9-SUKCESJA-07](MEM-E9-SUKCESJA-07-uznaj-brak-odpowiedzi-nastepcy-c126.md); [MEM-E9-SUKCESJA-08](MEM-E9-SUKCESJA-08-zapytanie-sprawa-przejecia-albumu-album-termin-los.md); [MEM-E9-SUKCESJA-09](MEM-E9-SUKCESJA-09-polityka-p21-zawsze-gdy-zgon-potwierdzony-wtedy-pr.md); [MEM-E9-SUKCESJA-10](MEM-E9-SUKCESJA-10-polityka-p23-zawsze-gdy-zgon-potwierdzony-wtedy-ud.md); [MEM-E9-SUKCESJA-11](MEM-E9-SUKCESJA-11-zdarzenie-integracyjne-album-prywatny-udostepniony.md); [MEM-E9-SUKCESJA-12](MEM-E9-SUKCESJA-12-polityka-p38-zawsze-gdy-termin-przejecia-minal-wte.md); [MEM-E9-SUKCESJA-13](MEM-E9-SUKCESJA-13-zdarzenie-integracyjne-zgon-potwierdzony-e121-w-ko.md) |
| Kod roboczy | MEM-E0-SUKCESJA-01 |
| Źródło | mapa modułów, karta sukcesja; ADR 0001, 0004, 0019 |

## Cel

Szkielet modułu sukcesja: struktura, publiczny kontrakt, testy architektury — część epiku E0 „Szkielet end-to-end i platforma (etap 1)”.

## Kontekst

- Moduł **sukcesja** — kluczowe pytanie: Kto przejmuje album lub odczyt, gdy właściciela już nie ma?
- Czego moduł nie wie (nie wolno tego wymagać): budowa albumu; mechanika dostępu
- Projekt modułu: model procesów modułu sukcesja
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- schemat bazy i rola dla modułu sukcesja (ADR 0004) (źródło: mapa modułów, karta sukcesja; ADR 0001, 0004, 0019)
- warstwy Onion: Domain / Application / Infrastructure / API, zależności tylko do środka (ADR 0019) (źródło: mapa modułów, karta sukcesja; ADR 0001, 0004, 0019)
- publiczny kontrakt modułu (tylko kontrakty eksportowane) (źródło: mapa modułów, karta sukcesja; ADR 0001, 0004, 0019)
- test architektury: moduł nie importuje wnętrza innych modułów (ADR 0001) i kierunek warstw (ADR 0019) (źródło: mapa modułów, karta sukcesja; ADR 0001, 0004, 0019)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E3-SUKCESJA-01, MEM-E9-SUKCESJA-01, MEM-E9-SUKCESJA-02)

## Kryteria akceptacji

- [ ] Moduł ma warstwy Onion i publiczny kontrakt; Domain nie zależy od Application/Infrastructure/API; inne moduły importują tylko kontrakt. *Sprawdzenie: test architektury (fixture łamiący regułę failuje)*
- [ ] Rola bazy modułu nie czyta cudzych schematów. *Sprawdzenie: test integracyjny uprawnień*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
