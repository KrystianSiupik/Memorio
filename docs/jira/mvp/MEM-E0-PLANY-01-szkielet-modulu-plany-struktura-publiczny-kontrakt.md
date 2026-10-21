# MEM-E0-PLANY-01: Szkielet modułu plany: struktura, publiczny kontrakt, testy architektury

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E0](epiki/E0-szkielet-end-to-end-i-platforma-etap-1.md) Szkielet end-to-end i platforma (etap 1) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, plany, MEM-E0-PLANY-01 |
| Moduł | plany |
| Warstwa | szkielet modułu |
| Rodzaj | szkielet modułu |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-01](MEM-E0-PLAT-01-szkielet-modularnego-monolitu-net-14-modulow-kontr.md); [MEM-E0-PLAT-03](MEM-E0-PLAT-03-postgresql-postgis-schemat-i-rola-bazy-na-modul-im.md) |
| Blokuje | [MEM-E7-PLANY-01](MEM-E7-PLANY-01-nadaj-uprawnienia-planu-c90.md); [MEM-E7-PLANY-02](MEM-E7-PLANY-02-przelacz-w-tryb-tylko-do-odczytu-c91.md); [MEM-E7-PLANY-03](MEM-E7-PLANY-03-odswiez-stan-subskrypcji-u-posrednika-c96.md); [MEM-E7-PLANY-04](MEM-E7-PLANY-04-wstrzymaj-przyciecie-archiwum-c93.md); [MEM-E7-PLANY-05](MEM-E7-PLANY-05-wskaz-material-do-zachowania-c94.md); [MEM-E7-PLANY-06](MEM-E7-PLANY-06-zlec-przyciecie-archiwum-do-darmowego-limitu-c95.md); [MEM-E7-PLANY-07](MEM-E7-PLANY-07-zapros-do-planu-rodzinnego-c97.md); [MEM-E7-PLANY-08](MEM-E7-PLANY-08-dolacz-do-planu-rodzinnego-c98.md); [MEM-E7-PLANY-09](MEM-E7-PLANY-09-usun-osobe-z-planu-rodzinnego-c99.md); [MEM-E7-PLANY-10](MEM-E7-PLANY-10-przelacz-konto-w-tryb-ponad-limitem-c100.md); [MEM-E7-PLANY-11](MEM-E7-PLANY-11-aktywuj-plan-albumu-po-przejeciu-c165.md); [MEM-E7-PLANY-12](MEM-E7-PLANY-12-zapytanie-stan-planu-tryb-terminy-zuzycie-wybor-do.md); [MEM-E3-PLANY-01](MEM-E3-PLANY-01-read-port-plany-dla-album-limit-miejsc-dla-albumu.md); [MEM-E7-PLANY-13](MEM-E7-PLANY-13-polityka-p13-zawsze-gdy-subskrypcja-aktywowana-wte.md); [MEM-E7-PLANY-14](MEM-E7-PLANY-14-polityka-p14-zawsze-gdy-subskrypcja-wygasla-wtedy-.md); [MEM-E7-PLANY-15](MEM-E7-PLANY-15-zdarzenie-integracyjne-nadszedl-termin-ostrzezenia.md); [MEM-E7-PLANY-16](MEM-E7-PLANY-16-polityka-p16-zawsze-gdy-ostrzezenie-niedoreczone-w.md); [MEM-E7-PLANY-17](MEM-E7-PLANY-17-zdarzenie-integracyjne-przyciecie-wstrzymane-e100-.md); [MEM-E7-PLANY-18](MEM-E7-PLANY-18-polityka-p17-zawsze-gdy-mina-24-miesiace-tylko-do-.md); [MEM-E7-PLANY-19](MEM-E7-PLANY-19-polityka-p18-zawsze-gdy-osoba-wypadnie-z-planu-rod.md); [MEM-E7-PLANY-20](MEM-E7-PLANY-20-zdarzenie-integracyjne-plan-aktywny-e94-w-kontrakc.md); [MEM-E7-PLANY-21](MEM-E7-PLANY-21-zdarzenie-integracyjne-album-lub-konto-w-trybie-ty.md); [MEM-E7-PLANY-22](MEM-E7-PLANY-22-zdarzenie-integracyjne-przyciecie-archiwum-zlecone.md); [MEM-E7-PLANY-23](MEM-E7-PLANY-23-polityka-p36-zawsze-gdy-album-bez-wlasciciela-wted.md); [MEM-E7-PLANY-24](MEM-E7-PLANY-24-zdarzenie-integracyjne-plan-albumu-bez-wlasciciela.md); [MEM-E7-PLANY-25](MEM-E7-PLANY-25-polityka-p41-zawsze-gdy-album-przejety-przez-uczes.md) |
| Kod roboczy | MEM-E0-PLANY-01 |
| Źródło | mapa modułów, karta plany; ADR 0001, 0004, 0019 |

## Cel

Szkielet modułu plany: struktura, publiczny kontrakt, testy architektury — część epiku E0 „Szkielet end-to-end i platforma (etap 1)”.

## Kontekst

- Moduł **plany** — kluczowe pytanie: W jakim stanie jest teraz plan tego konta lub albumu i co z tego wynika?
- Czego moduł nie wie (nie wolno tego wymagać): co jest w archiwum; kto co wniósł; zgony, następcy, odmowy przejęcia (zna tylko „album bez właściciela”)
- Projekt modułu: model procesów modułu plany
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- schemat bazy i rola dla modułu plany (ADR 0004) (źródło: mapa modułów, karta plany; ADR 0001, 0004, 0019)
- warstwy Onion: Domain / Application / Infrastructure / API, zależności tylko do środka (ADR 0019) (źródło: mapa modułów, karta plany; ADR 0001, 0004, 0019)
- publiczny kontrakt modułu (tylko kontrakty eksportowane) (źródło: mapa modułów, karta plany; ADR 0001, 0004, 0019)
- test architektury: moduł nie importuje wnętrza innych modułów (ADR 0001) i kierunek warstw (ADR 0019) (źródło: mapa modułów, karta plany; ADR 0001, 0004, 0019)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E7-PLANY-01, MEM-E7-PLANY-02, MEM-E7-PLANY-03)

## Kryteria akceptacji

- [ ] Moduł ma warstwy Onion i publiczny kontrakt; Domain nie zależy od Application/Infrastructure/API; inne moduły importują tylko kontrakt. *Sprawdzenie: test architektury (fixture łamiący regułę failuje)*
- [ ] Rola bazy modułu nie czyta cudzych schematów. *Sprawdzenie: test integracyjny uprawnień*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
