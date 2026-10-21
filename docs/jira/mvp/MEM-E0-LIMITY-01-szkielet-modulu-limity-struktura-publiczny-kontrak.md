# MEM-E0-LIMITY-01: Szkielet modułu limity: struktura, publiczny kontrakt, testy architektury

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E0](epiki/E0-szkielet-end-to-end-i-platforma-etap-1.md) Szkielet end-to-end i platforma (etap 1) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, limity, MEM-E0-LIMITY-01 |
| Moduł | limity |
| Warstwa | szkielet modułu |
| Rodzaj | szkielet modułu |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-01](MEM-E0-PLAT-01-szkielet-modularnego-monolitu-net-14-modulow-kontr.md); [MEM-E0-PLAT-03](MEM-E0-PLAT-03-postgresql-postgis-schemat-i-rola-bazy-na-modul-im.md) |
| Blokuje | [MEM-E3-LIMITY-01](MEM-E3-LIMITY-01-zarezerwuj-jednostki-z-puli-c150.md); [MEM-E2-LIMITY-01](MEM-E2-LIMITY-01-zwroc-jednostki-do-puli-c151.md); [MEM-E7-LIMITY-01](MEM-E7-LIMITY-01-ustaw-limit-puli-c152.md); [MEM-E3-LIMITY-02](MEM-E3-LIMITY-02-read-port-stan-puli-albumu-r6.md); [MEM-E2-LIMITY-02](MEM-E2-LIMITY-02-write-port-limity-dla-archiwum-zarezerwuj-zwroc-pr.md); [MEM-E3-LIMITY-03](MEM-E3-LIMITY-03-write-port-limity-dla-album-zarezerwuj-przestrzen-.md); [MEM-E2-LIMITY-03](MEM-E2-LIMITY-03-write-port-limity-dla-narracja-ai-zuzyj-limit-ai.md); [MEM-E7-LIMITY-02](MEM-E7-LIMITY-02-polityka-p27-zawsze-gdy-plan-aktywny-albo-przelacz.md) |
| Kod roboczy | MEM-E0-LIMITY-01 |
| Źródło | mapa modułów, karta limity; ADR 0001, 0004, 0019 |

## Cel

Szkielet modułu limity: struktura, publiczny kontrakt, testy architektury — część epiku E0 „Szkielet end-to-end i platforma (etap 1)”.

## Kontekst

- Moduł **limity** — kluczowe pytanie: Czy ta operacja zmieści się w puli i ile zostało?
- Czego moduł nie wie (nie wolno tego wymagać): skąd limit; kto płaci; czego dotyczy treść
- Projekt modułu: model procesów modułu limity
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- schemat bazy i rola dla modułu limity (ADR 0004) (źródło: mapa modułów, karta limity; ADR 0001, 0004, 0019)
- warstwy Onion: Domain / Application / Infrastructure / API, zależności tylko do środka (ADR 0019) (źródło: mapa modułów, karta limity; ADR 0001, 0004, 0019)
- publiczny kontrakt modułu (tylko kontrakty eksportowane) (źródło: mapa modułów, karta limity; ADR 0001, 0004, 0019)
- test architektury: moduł nie importuje wnętrza innych modułów (ADR 0001) i kierunek warstw (ADR 0019) (źródło: mapa modułów, karta limity; ADR 0001, 0004, 0019)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E3-LIMITY-01, MEM-E2-LIMITY-01, MEM-E7-LIMITY-01)

## Kryteria akceptacji

- [ ] Moduł ma warstwy Onion i publiczny kontrakt; Domain nie zależy od Application/Infrastructure/API; inne moduły importują tylko kontrakt. *Sprawdzenie: test architektury (fixture łamiący regułę failuje)*
- [ ] Rola bazy modułu nie czyta cudzych schematów. *Sprawdzenie: test integracyjny uprawnień*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
