# MEM-E0-PLAT-09: Etap 1: VPS, Compose, nginx, WAL-G, comiesięczny test odtworzenia

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E0](epiki/E0-szkielet-end-to-end-i-platforma-etap-1.md) Szkielet end-to-end i platforma (etap 1) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Platforma, -, MEM-E0-PLAT-09 |
| Moduł | - |
| Warstwa | infrastruktura |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-01](MEM-E0-PLAT-01-szkielet-modularnego-monolitu-net-14-modulow-kontr.md) |
| Blokuje | [MEM-E0-PLAT-18](MEM-E0-PLAT-18-sekrety-etapu-1-env-poza-repo-sops-age-do-odtworze.md) |
| Kod roboczy | MEM-E0-PLAT-09 |
| Źródło | projekt systemu T9; SD13, QS2 |

## Cel

Etap 1: VPS, Compose, nginx, WAL-G, comiesięczny test odtworzenia — część epiku E0 „Szkielet end-to-end i platforma (etap 1)”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Etap 1: VPS, Compose, nginx, WAL-G, comiesięczny test odtworzenia (źródło: projekt systemu T9; SD13, QS2)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E0-PLAT-18)

## Kryteria akceptacji

- [ ] Etap 1: VPS, Compose, nginx, WAL-G, comiesięczny test odtworzenia. *Sprawdzenie: test/skrypt w CI lub ręcznie na środowisku*
- [ ] Fitness function SD13: test odtworzenia PITR raz w miesiącu. *Sprawdzenie: harmonogram operacyjny — skrypt + raport*
- [ ] Scenariusz QS2: RPO ≤ 15 min, RTO ≤ 8 h, test odtworzenia co miesiąc. *Sprawdzenie: test wydajności / monitoring*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
