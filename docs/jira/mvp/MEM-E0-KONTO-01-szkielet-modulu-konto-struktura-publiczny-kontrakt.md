# MEM-E0-KONTO-01: Szkielet modułu konto: struktura, publiczny kontrakt, testy architektury

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E0](epiki/E0-szkielet-end-to-end-i-platforma-etap-1.md) Szkielet end-to-end i platforma (etap 1) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, konto, MEM-E0-KONTO-01 |
| Moduł | konto |
| Warstwa | szkielet modułu |
| Rodzaj | szkielet modułu |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-01](MEM-E0-PLAT-01-szkielet-modularnego-monolitu-net-14-modulow-kontr.md); [MEM-E0-PLAT-03](MEM-E0-PLAT-03-postgresql-postgis-schemat-i-rola-bazy-na-modul-im.md) |
| Blokuje | [MEM-E1-KONTO-01](MEM-E1-KONTO-01-zarejestruj-konto-c1.md); [MEM-E1-KONTO-02](MEM-E1-KONTO-02-zaakceptuj-regulamin-c2.md); [MEM-E1-KONTO-03](MEM-E1-KONTO-03-zlec-usuniecie-konta-c3.md); [MEM-E1-KONTO-04](MEM-E1-KONTO-04-usun-konto-definitywnie-c4.md); [MEM-E1-KONTO-05](MEM-E1-KONTO-05-cofnij-usuniecie-konta-c5.md); [MEM-E1-KONTO-06](MEM-E1-KONTO-06-zapytanie-skutki-usuniecia-konta-ile-zdjec-z-ilu-a.md); [MEM-E1-KONTO-07](MEM-E1-KONTO-07-polityka-p1-zawsze-gdy-okno-usuniecia-konta-minie-.md); [MEM-E1-KONTO-08](MEM-E1-KONTO-08-zdarzenie-integracyjne-konto-usuniete-e7-w-kontrak.md) |
| Kod roboczy | MEM-E0-KONTO-01 |
| Źródło | mapa modułów, karta konto; ADR 0001, 0004, 0019 |

## Cel

Szkielet modułu konto: struktura, publiczny kontrakt, testy architektury — część epiku E0 „Szkielet end-to-end i platforma (etap 1)”.

## Kontekst

- Moduł **konto** — kluczowe pytanie: Czy ta osoba jest aktywnym użytkownikiem i czy jej konto ma zostać?
- Czego moduł nie wie (nie wolno tego wymagać): co dokładnie wypadnie przy usunięciu (pyta inne konteksty)
- Projekt modułu: model procesów modułu konto
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- schemat bazy i rola dla modułu konto (ADR 0004) (źródło: mapa modułów, karta konto; ADR 0001, 0004, 0019)
- warstwy Onion: Domain / Application / Infrastructure / API, zależności tylko do środka (ADR 0019) (źródło: mapa modułów, karta konto; ADR 0001, 0004, 0019)
- publiczny kontrakt modułu (tylko kontrakty eksportowane) (źródło: mapa modułów, karta konto; ADR 0001, 0004, 0019)
- test architektury: moduł nie importuje wnętrza innych modułów (ADR 0001) i kierunek warstw (ADR 0019) (źródło: mapa modułów, karta konto; ADR 0001, 0004, 0019)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E1-KONTO-01, MEM-E1-KONTO-02, MEM-E1-KONTO-03)

## Kryteria akceptacji

- [ ] Moduł ma warstwy Onion i publiczny kontrakt; Domain nie zależy od Application/Infrastructure/API; inne moduły importują tylko kontrakt. *Sprawdzenie: test architektury (fixture łamiący regułę failuje)*
- [ ] Rola bazy modułu nie czyta cudzych schematów. *Sprawdzenie: test integracyjny uprawnień*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
