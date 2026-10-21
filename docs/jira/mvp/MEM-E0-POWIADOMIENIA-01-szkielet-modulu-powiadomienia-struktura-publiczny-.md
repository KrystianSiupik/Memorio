# MEM-E0-POWIADOMIENIA-01: Szkielet modułu powiadomienia: struktura, publiczny kontrakt, testy architektury

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E0](epiki/E0-szkielet-end-to-end-i-platforma-etap-1.md) Szkielet end-to-end i platforma (etap 1) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, powiadomienia, MEM-E0-POWIADOMIENIA-01 |
| Moduł | powiadomienia |
| Warstwa | szkielet modułu |
| Rodzaj | szkielet modułu |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-01](MEM-E0-PLAT-01-szkielet-modularnego-monolitu-net-14-modulow-kontr.md); [MEM-E0-PLAT-03](MEM-E0-PLAT-03-postgresql-postgis-schemat-i-rola-bazy-na-modul-im.md) |
| Blokuje | [MEM-E3-POWIADOMIENIA-01](MEM-E3-POWIADOMIENIA-01-wyslij-przypomnienie-o-zaproszeniu-c43.md); [MEM-E3-POWIADOMIENIA-02](MEM-E3-POWIADOMIENIA-02-zablokuj-zaproszenia-z-geo-na-swoj-adres-c46.md); [MEM-E3-POWIADOMIENIA-03](MEM-E3-POWIADOMIENIA-03-powiadom-wlasciciela-o-pelnej-puli-c48.md); [MEM-E3-POWIADOMIENIA-04](MEM-E3-POWIADOMIENIA-04-popros-o-wybor-zostaw-zabierz-c54.md); [MEM-E7-POWIADOMIENIA-01](MEM-E7-POWIADOMIENIA-01-wyslij-ostrzezenie-przed-przycieciem-c92.md); [MEM-E9-POWIADOMIENIA-01](MEM-E9-POWIADOMIENIA-01-powiadom-uczestnikow-o-nowym-wlascicielu-c166.md); [MEM-E9-POWIADOMIENIA-02](MEM-E9-POWIADOMIENIA-02-przypomnij-nastepcy-o-decyzji-7-i-21-dzien-c167.md); [MEM-E3-POWIADOMIENIA-05](MEM-E3-POWIADOMIENIA-05-powiadom-oznaczonego-c171.md); [MEM-E3-POWIADOMIENIA-06](MEM-E3-POWIADOMIENIA-06-read-port-powiadomienia-dla-album-czy-adres-zablok.md); [MEM-E3-POWIADOMIENIA-07](MEM-E3-POWIADOMIENIA-07-polityka-p6-zawsze-gdy-minie-7-dni-od-nieprzyjeteg.md); [MEM-E3-POWIADOMIENIA-08](MEM-E3-POWIADOMIENIA-08-polityka-p7-zawsze-gdy-pula-albumu-wyczerpana-wted.md); [MEM-E3-POWIADOMIENIA-09](MEM-E3-POWIADOMIENIA-09-polityka-p8-zawsze-gdy-uczestnik-usuniety-z-albumu.md); [MEM-E7-POWIADOMIENIA-02](MEM-E7-POWIADOMIENIA-02-polityka-p15-zawsze-gdy-nadejdzie-termin-ostrzezen.md); [MEM-E7-POWIADOMIENIA-03](MEM-E7-POWIADOMIENIA-03-zdarzenie-integracyjne-ostrzezenie-niedoreczone-e9.md); [MEM-E7-POWIADOMIENIA-04](MEM-E7-POWIADOMIENIA-04-polityka-p26-zawsze-gdy-przyciecie-wstrzymane-wted.md); [MEM-E9-POWIADOMIENIA-03](MEM-E9-POWIADOMIENIA-03-polityka-p42-zawsze-gdy-album-przejety-przez-uczes.md); [MEM-E9-POWIADOMIENIA-04](MEM-E9-POWIADOMIENIA-04-polityka-p43-zawsze-gdy-zgon-potwierdzony-wtedy-za.md); [MEM-E3-POWIADOMIENIA-10](MEM-E3-POWIADOMIENIA-10-polityka-p45-zawsze-gdy-uczestnik-oznaczony-wtedy-.md) |
| Kod roboczy | MEM-E0-POWIADOMIENIA-01 |
| Źródło | mapa modułów, karta powiadomienia; ADR 0001, 0004, 0019 |

## Cel

Szkielet modułu powiadomienia: struktura, publiczny kontrakt, testy architektury — część epiku E0 „Szkielet end-to-end i platforma (etap 1)”.

## Kontekst

- Moduł **powiadomienia** — kluczowe pytanie: Czy wiadomość dotarła i do kogo nie wolno pisać?
- Czego moduł nie wie (nie wolno tego wymagać): sens biznesowy wiadomości poza kategorią
- Projekt modułu: model procesów modułu powiadomienia
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- schemat bazy i rola dla modułu powiadomienia (ADR 0004) (źródło: mapa modułów, karta powiadomienia; ADR 0001, 0004, 0019)
- warstwy Onion: Domain / Application / Infrastructure / API, zależności tylko do środka (ADR 0019) (źródło: mapa modułów, karta powiadomienia; ADR 0001, 0004, 0019)
- publiczny kontrakt modułu (tylko kontrakty eksportowane) (źródło: mapa modułów, karta powiadomienia; ADR 0001, 0004, 0019)
- test architektury: moduł nie importuje wnętrza innych modułów (ADR 0001) i kierunek warstw (ADR 0019) (źródło: mapa modułów, karta powiadomienia; ADR 0001, 0004, 0019)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E3-POWIADOMIENIA-01, MEM-E3-POWIADOMIENIA-02, MEM-E3-POWIADOMIENIA-03)

## Kryteria akceptacji

- [ ] Moduł ma warstwy Onion i publiczny kontrakt; Domain nie zależy od Application/Infrastructure/API; inne moduły importują tylko kontrakt. *Sprawdzenie: test architektury (fixture łamiący regułę failuje)*
- [ ] Rola bazy modułu nie czyta cudzych schematów. *Sprawdzenie: test integracyjny uprawnień*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
