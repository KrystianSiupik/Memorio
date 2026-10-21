# MEM-E0-PLAT-04: Magazyn obiektów w UE: bucket prywatny, podpisy 5 min partiami, strefa tymczasowa uploadu

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E0](epiki/E0-szkielet-end-to-end-i-platforma-etap-1.md) Szkielet end-to-end i platforma (etap 1) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Platforma, -, MEM-E0-PLAT-04 |
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
| Kod roboczy | MEM-E0-PLAT-04 |
| Źródło | projekt systemu T4; SD5, QS4 |

## Cel

Magazyn obiektów w UE: bucket prywatny, podpisy 5 min partiami, strefa tymczasowa uploadu — część epiku E0 „Szkielet end-to-end i platforma (etap 1)”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Magazyn obiektów w UE: bucket prywatny, podpisy 5 min partiami, strefa tymczasowa uploadu (źródło: projekt systemu T4; SD5, QS4)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Magazyn obiektów w UE: bucket prywatny, podpisy 5 min partiami, strefa tymczasowa uploadu. *Sprawdzenie: test/skrypt w CI lub ręcznie na środowisku*
- [ ] Fitness function SD5: TTL podpisu ≤ 300 s; bucket bez publicznego odczytu. *Sprawdzenie: CI + monitoring — test konfiguracji + codzienny skrypt sprawdzający politykę bucketu*
- [ ] Scenariusz QS4: media ≤ 5 min (podpis), wyszukiwanie: od razu (następne zapytanie). *Sprawdzenie: test wydajności / monitoring*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
