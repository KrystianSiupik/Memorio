# MEM-E0-PLAT-10: Obserwowalność: OpenTelemetry, Prometheus/Loki/Grafana, monitor zewnętrzny, alert

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E0](epiki/E0-szkielet-end-to-end-i-platforma-etap-1.md) Szkielet end-to-end i platforma (etap 1) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Platforma, -, MEM-E0-PLAT-10 |
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
| Kod roboczy | MEM-E0-PLAT-10 |
| Źródło | projekt systemu T10; SD14, QS12 |

## Cel

Obserwowalność: OpenTelemetry, Prometheus/Loki/Grafana, monitor zewnętrzny, alert — część epiku E0 „Szkielet end-to-end i platforma (etap 1)”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Obserwowalność: OpenTelemetry, Prometheus/Loki/Grafana, monitor zewnętrzny, alert (źródło: projekt systemu T10; SD14, QS12)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Obserwowalność: OpenTelemetry, Prometheus/Loki/Grafana, monitor zewnętrzny, alert. *Sprawdzenie: test/skrypt w CI lub ręcznie na środowisku*
- [ ] Fitness function SD1: moduł nie importuje wnętrza innego modułu (tylko contracts). *Sprawdzenie: CI — test architektury (NetArchTest/ArchUnitNET)*
- [ ] Fitness function SD2: konwencje modułów (domain bez infrastruktury). *Sprawdzenie: CI — test architektury*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
