# MEM-E0-NARRACJA-AI-01: Szkielet modułu narracja-ai: struktura, publiczny kontrakt, testy architektury

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E0](epiki/E0-szkielet-end-to-end-i-platforma-etap-1.md) Szkielet end-to-end i platforma (etap 1) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, narracja-ai, MEM-E0-NARRACJA-AI-01 |
| Moduł | narracja-ai |
| Warstwa | szkielet modułu |
| Rodzaj | szkielet modułu |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-01](MEM-E0-PLAT-01-szkielet-modularnego-monolitu-net-14-modulow-kontr.md); [MEM-E0-PLAT-03](MEM-E0-PLAT-03-postgresql-postgis-schemat-i-rola-bazy-na-modul-im.md) |
| Blokuje | [MEM-E2-NARRACJA-AI-01](MEM-E2-NARRACJA-AI-01-popros-ai-o-szkic-opisu-c15.md); [MEM-E6-NARRACJA-AI-01](MEM-E6-NARRACJA-AI-01-udziel-zgody-na-ai-dla-albumu-c80.md); [MEM-E6-NARRACJA-AI-02](MEM-E6-NARRACJA-AI-02-odwolaj-zgode-na-ai-c81.md); [MEM-E6-NARRACJA-AI-03](MEM-E6-NARRACJA-AI-03-zamow-podsumowanie-okresu-c82.md); [MEM-E6-NARRACJA-AI-04](MEM-E6-NARRACJA-AI-04-zatwierdz-i-zapisz-podsumowanie-c83.md); [MEM-E6-NARRACJA-AI-05](MEM-E6-NARRACJA-AI-05-dodaj-podsumowanie-do-albumu-c84.md); [MEM-E6-NARRACJA-AI-06](MEM-E6-NARRACJA-AI-06-zapytanie-materialy-dozwolone-do-podsumowania-zgod.md); [MEM-E6-NARRACJA-AI-07](MEM-E6-NARRACJA-AI-07-zdarzenie-integracyjne-zgoda-na-ai-odwolana-e81-w-.md) |
| Kod roboczy | MEM-E0-NARRACJA-AI-01 |
| Źródło | mapa modułów, karta narracja-ai; ADR 0001, 0004, 0019 |

## Cel

Szkielet modułu narracja-ai: struktura, publiczny kontrakt, testy architektury — część epiku E0 „Szkielet end-to-end i platforma (etap 1)”.

## Kontekst

- Moduł **narracja-ai** — kluczowe pytanie: Jaką opowieść wolno opowiedzieć z tych materiałów, nie zmyślając?
- Czego moduł nie wie (nie wolno tego wymagać): trwałość artefaktu; dokładne nazwy i imiona po stronie dostawcy
- Projekt modułu: model procesów modułu narracja-ai
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- schemat bazy i rola dla modułu narracja-ai (ADR 0004) (źródło: mapa modułów, karta narracja-ai; ADR 0001, 0004, 0019)
- warstwy Onion: Domain / Application / Infrastructure / API, zależności tylko do środka (ADR 0019) (źródło: mapa modułów, karta narracja-ai; ADR 0001, 0004, 0019)
- publiczny kontrakt modułu (tylko kontrakty eksportowane) (źródło: mapa modułów, karta narracja-ai; ADR 0001, 0004, 0019)
- test architektury: moduł nie importuje wnętrza innych modułów (ADR 0001) i kierunek warstw (ADR 0019) (źródło: mapa modułów, karta narracja-ai; ADR 0001, 0004, 0019)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E2-NARRACJA-AI-01, MEM-E6-NARRACJA-AI-01, MEM-E6-NARRACJA-AI-02)

## Kryteria akceptacji

- [ ] Moduł ma warstwy Onion i publiczny kontrakt; Domain nie zależy od Application/Infrastructure/API; inne moduły importują tylko kontrakt. *Sprawdzenie: test architektury (fixture łamiący regułę failuje)*
- [ ] Rola bazy modułu nie czyta cudzych schematów. *Sprawdzenie: test integracyjny uprawnień*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
