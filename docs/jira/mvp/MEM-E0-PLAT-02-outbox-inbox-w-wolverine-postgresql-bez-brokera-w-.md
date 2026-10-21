# MEM-E0-PLAT-02: Outbox/Inbox w Wolverine (PostgreSQL, bez brokera) w osobnym workerze + metryka wieku kolejki

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E0](epiki/E0-szkielet-end-to-end-i-platforma-etap-1.md) Szkielet end-to-end i platforma (etap 1) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Platforma, -, MEM-E0-PLAT-02 |
| Moduł | - |
| Warstwa | infrastruktura |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-01](MEM-E0-PLAT-01-szkielet-modularnego-monolitu-net-14-modulow-kontr.md) |
| Blokuje | - |
| Kod roboczy | MEM-E0-PLAT-02 |
| Źródło | projekt systemu T2; SD6, QS7, QS12 |

## Cel

Outbox/Inbox w Wolverine (PostgreSQL, bez brokera) w osobnym workerze + metryka wieku kolejki — część epiku E0 „Szkielet end-to-end i platforma (etap 1)”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Outbox/Inbox w Wolverine (PostgreSQL, bez brokera) w osobnym workerze + metryka wieku kolejki (źródło: projekt systemu T2; SD6, QS7, QS12)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Outbox/Inbox w Wolverine (PostgreSQL, bez brokera) w osobnym workerze + metryka wieku kolejki. *Sprawdzenie: test/skrypt w CI lub ręcznie na środowisku*
- [ ] Fitness function SD6: każdy handler zdarzenia ma test idempotentności (podwójne dostarczenie). *Sprawdzenie: CI — konwencja testów + test architektury wyszukujący handlery bez testu*
- [ ] Fitness function SD6: wiek outbox < 10 min. *Sprawdzenie: monitoring — alert*
- [ ] Scenariusz QS7: 0 żywych artefaktów z niedostępnym źródłem po kontroli dziennej; unieważnienie widoczne ≤ 1 min. *Sprawdzenie: test wydajności / monitoring*
- [ ] Scenariusz QS12: wykrycie ≤ 5 min; kolejka outbox starsza niż 10 min = alert. *Sprawdzenie: test wydajności / monitoring*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
