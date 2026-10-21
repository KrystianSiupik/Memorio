# MEM-E0-ARCHIWUM-01: Szkielet modułu archiwum: struktura, publiczny kontrakt, testy architektury

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E0](epiki/E0-szkielet-end-to-end-i-platforma-etap-1.md) Szkielet end-to-end i platforma (etap 1) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, archiwum, MEM-E0-ARCHIWUM-01 |
| Moduł | archiwum |
| Warstwa | szkielet modułu |
| Rodzaj | szkielet modułu |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-01](MEM-E0-PLAT-01-szkielet-modularnego-monolitu-net-14-modulow-kontr.md); [MEM-E0-PLAT-03](MEM-E0-PLAT-03-postgresql-postgis-schemat-i-rola-bazy-na-modul-im.md) |
| Blokuje | [MEM-E2-ARCHIWUM-01](MEM-E2-ARCHIWUM-01-utworz-wspomnienie-c12.md); [MEM-E2-ARCHIWUM-02](MEM-E2-ARCHIWUM-02-dodaj-zdjecia-do-wspomnienia-c13.md); [MEM-E2-ARCHIWUM-03](MEM-E2-ARCHIWUM-03-zatwierdz-opis-c16.md); [MEM-E2-ARCHIWUM-04](MEM-E2-ARCHIWUM-04-usun-swoja-tresc-c17.md); [MEM-E2-ARCHIWUM-05](MEM-E2-ARCHIWUM-05-usun-tresc-definitywnie-c18.md); [MEM-E2-ARCHIWUM-06](MEM-E2-ARCHIWUM-06-przywroc-tresc-z-kosza-c19.md); [MEM-E2-ARCHIWUM-07](MEM-E2-ARCHIWUM-07-zloz-zadanie-usuniecia-danych-c20.md); [MEM-E10-ARCHIWUM-01](MEM-E10-ARCHIWUM-01-zaimportuj-zdjecia-z-telefonu-c21.md); [MEM-E10-ARCHIWUM-02](MEM-E10-ARCHIWUM-02-przypisz-miejsce-grupie-zdjec-c22.md); [MEM-E10-ARCHIWUM-03](MEM-E10-ARCHIWUM-03-usun-zdjecie-z-poczekalni-c23.md); [MEM-E7-ARCHIWUM-01](MEM-E7-ARCHIWUM-01-przytnij-archiwum-do-limitu-nadmiar-do-kosza-na-30.md); [MEM-E1-ARCHIWUM-01](MEM-E1-ARCHIWUM-01-usun-tresci-konta-c25.md); [MEM-E7-ARCHIWUM-02](MEM-E7-ARCHIWUM-02-przywroc-przyciety-material-z-kosza-c168.md); [MEM-E10-ARCHIWUM-04](MEM-E10-ARCHIWUM-04-zapytanie-poczekalnia-do-przypisania-r4.md); [MEM-E5-ARCHIWUM-01](MEM-E5-ARCHIWUM-01-read-port-wspomnienia-okresu-przecinajace-okres-w-.md); [MEM-E2-ARCHIWUM-08](MEM-E2-ARCHIWUM-08-zapytanie-mapa-wspomnien-punkty-wlasne-z-albumow-d.md); [MEM-E2-ARCHIWUM-09](MEM-E2-ARCHIWUM-09-zapytanie-szczegoly-wspomnienia-miejsce-data-z-pre.md); [MEM-E2-ARCHIWUM-10](MEM-E2-ARCHIWUM-10-zapytanie-kosz-moje-tresci-usuniete-w-ostatnich-30.md); [MEM-E3-ARCHIWUM-01](MEM-E3-ARCHIWUM-01-write-port-archiwum-dla-album-zapisz-tresc-z-numer.md); [MEM-E1-ARCHIWUM-02](MEM-E1-ARCHIWUM-02-read-port-archiwum-dla-konto-ile-tresci-wypadnie-p.md); [MEM-E2-ARCHIWUM-11](MEM-E2-ARCHIWUM-11-polityka-p3-zawsze-gdy-okno-kosza-minie-wtedy-usun.md); [MEM-E2-ARCHIWUM-12](MEM-E2-ARCHIWUM-12-zdarzenie-integracyjne-tresc-usunieta-definitywnie.md); [MEM-E2-ARCHIWUM-13](MEM-E2-ARCHIWUM-13-polityka-p5-zawsze-gdy-zdjecia-maja-przypisane-mie.md); [MEM-E7-ARCHIWUM-03](MEM-E7-ARCHIWUM-03-polityka-p33-zawsze-gdy-przyciecie-zlecone-wtedy-p.md); [MEM-E7-ARCHIWUM-04](MEM-E7-ARCHIWUM-04-zdarzenie-integracyjne-archiwum-przyciete-nadmiar-.md); [MEM-E1-ARCHIWUM-03](MEM-E1-ARCHIWUM-03-polityka-p35-zawsze-gdy-konto-usuniete-wtedy-usun-.md); [MEM-E7-ARCHIWUM-05](MEM-E7-ARCHIWUM-05-polityka-p44-zawsze-gdy-plan-znowu-aktywny-w-ciagu.md) |
| Kod roboczy | MEM-E0-ARCHIWUM-01 |
| Źródło | mapa modułów, karta archiwum; ADR 0001, 0004, 0019 |

## Cel

Szkielet modułu archiwum: struktura, publiczny kontrakt, testy architektury — część epiku E0 „Szkielet end-to-end i platforma (etap 1)”.

## Kontekst

- Moduł **archiwum** — kluczowe pytanie: Co ta osoba zapisała, gdzie i kiedy?
- Czego moduł nie wie (nie wolno tego wymagać): kto poza autorem widzi treść; w których albumach jest wspomnienie; plany i pieniądze; czy treść poszła do AI
- Projekt modułu: model procesów modułu archiwum
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- schemat bazy i rola dla modułu archiwum (ADR 0004) (źródło: mapa modułów, karta archiwum; ADR 0001, 0004, 0019)
- warstwy Onion: Domain / Application / Infrastructure / API, zależności tylko do środka (ADR 0019) (źródło: mapa modułów, karta archiwum; ADR 0001, 0004, 0019)
- publiczny kontrakt modułu (tylko kontrakty eksportowane) (źródło: mapa modułów, karta archiwum; ADR 0001, 0004, 0019)
- test architektury: moduł nie importuje wnętrza innych modułów (ADR 0001) i kierunek warstw (ADR 0019) (źródło: mapa modułów, karta archiwum; ADR 0001, 0004, 0019)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E2-ARCHIWUM-01, MEM-E2-ARCHIWUM-02, MEM-E2-ARCHIWUM-03)

## Kryteria akceptacji

- [ ] Moduł ma warstwy Onion i publiczny kontrakt; Domain nie zależy od Application/Infrastructure/API; inne moduły importują tylko kontrakt. *Sprawdzenie: test architektury (fixture łamiący regułę failuje)*
- [ ] Rola bazy modułu nie czyta cudzych schematów. *Sprawdzenie: test integracyjny uprawnień*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
