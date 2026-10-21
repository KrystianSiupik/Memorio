# MEM-E0-PLAT-08: Worker: Quartz.NET z magazynem w PostgreSQL; zadania idempotentne, gotowe na tryb klastra

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E0](epiki/E0-szkielet-end-to-end-i-platforma-etap-1.md) Szkielet end-to-end i platforma (etap 1) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Platforma, -, MEM-E0-PLAT-08 |
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
| Kod roboczy | MEM-E0-PLAT-08 |
| Źródło | projekt systemu T8; SD12 |

## Cel

Worker: Quartz.NET z magazynem w PostgreSQL; zadania idempotentne, gotowe na tryb klastra — część epiku E0 „Szkielet end-to-end i platforma (etap 1)”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Worker: Quartz.NET z magazynem w PostgreSQL; zadania idempotentne, gotowe na tryb klastra (źródło: projekt systemu T8; SD12)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Worker: Quartz.NET z magazynem w PostgreSQL; zadania idempotentne, gotowe na tryb klastra. *Sprawdzenie: test/skrypt w CI lub ręcznie na środowisku*
- [ ] Fitness function SD12: zadanie uruchomione dwa razy nie zmienia wyniku. *Sprawdzenie: CI — test z podwójnym uruchomieniem*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
