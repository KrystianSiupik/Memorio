# MEM-E0-POCHODZENIE-01: Szkielet modułu pochodzenie: struktura, publiczny kontrakt, testy architektury

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E0](epiki/E0-szkielet-end-to-end-i-platforma-etap-1.md) Szkielet end-to-end i platforma (etap 1) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, pochodzenie, MEM-E0-POCHODZENIE-01 |
| Moduł | pochodzenie |
| Warstwa | szkielet modułu |
| Rodzaj | szkielet modułu |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-01](MEM-E0-PLAT-01-szkielet-modularnego-monolitu-net-14-modulow-kontr.md); [MEM-E0-PLAT-03](MEM-E0-PLAT-03-postgresql-postgis-schemat-i-rola-bazy-na-modul-im.md) |
| Blokuje | [MEM-E6-POCHODZENIE-01](MEM-E6-POCHODZENIE-01-uniewaznij-artefakty-pochodne-c31.md); [MEM-E6-POCHODZENIE-02](MEM-E6-POCHODZENIE-02-zarejestruj-artefakt-c154.md); [MEM-E6-POCHODZENIE-03](MEM-E6-POCHODZENIE-03-write-port-pochodzenie-dla-narracja-ai-zarejestruj.md); [MEM-E6-POCHODZENIE-04](MEM-E6-POCHODZENIE-04-polityka-p4-zawsze-gdy-tresc-usunieta-definitywnie.md); [MEM-E6-POCHODZENIE-05](MEM-E6-POCHODZENIE-05-polityka-p9-zawsze-gdy-wklady-wycofane-z-albumu-wt.md); [MEM-E6-POCHODZENIE-06](MEM-E6-POCHODZENIE-06-polityka-p10-zawsze-gdy-dostep-odebrany-wtedy-unie.md); [MEM-E6-POCHODZENIE-07](MEM-E6-POCHODZENIE-07-polityka-p12-zawsze-gdy-zgoda-na-ai-odwolana-wtedy.md); [MEM-E6-POCHODZENIE-08](MEM-E6-POCHODZENIE-08-polityka-p34-zawsze-gdy-archiwum-przyciete-wtedy-u.md) |
| Kod roboczy | MEM-E0-POCHODZENIE-01 |
| Źródło | mapa modułów, karta pochodzenie; ADR 0001, 0004, 0019 |

## Cel

Szkielet modułu pochodzenie: struktura, publiczny kontrakt, testy architektury — część epiku E0 „Szkielet end-to-end i platforma (etap 1)”.

## Kontekst

- Moduł **pochodzenie** — kluczowe pytanie: Z czego powstał ten artefakt i czy nadal wolno mu istnieć?
- Czego moduł nie wie (nie wolno tego wymagać): przyczyna niedostępności źródła; treść artefaktu
- Projekt modułu: model procesów modułu pochodzenie
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- schemat bazy i rola dla modułu pochodzenie (ADR 0004) (źródło: mapa modułów, karta pochodzenie; ADR 0001, 0004, 0019)
- warstwy Onion: Domain / Application / Infrastructure / API, zależności tylko do środka (ADR 0019) (źródło: mapa modułów, karta pochodzenie; ADR 0001, 0004, 0019)
- publiczny kontrakt modułu (tylko kontrakty eksportowane) (źródło: mapa modułów, karta pochodzenie; ADR 0001, 0004, 0019)
- test architektury: moduł nie importuje wnętrza innych modułów (ADR 0001) i kierunek warstw (ADR 0019) (źródło: mapa modułów, karta pochodzenie; ADR 0001, 0004, 0019)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E6-POCHODZENIE-01, MEM-E6-POCHODZENIE-02, MEM-E6-POCHODZENIE-03)

## Kryteria akceptacji

- [ ] Moduł ma warstwy Onion i publiczny kontrakt; Domain nie zależy od Application/Infrastructure/API; inne moduły importują tylko kontrakt. *Sprawdzenie: test architektury (fixture łamiący regułę failuje)*
- [ ] Rola bazy modułu nie czyta cudzych schematów. *Sprawdzenie: test integracyjny uprawnień*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
