# MEM-E0-PLAT-21: Pule połączeń: rozmiary pul, max_connections, alert na zużycie, test obciążenia

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E0](epiki/E0-szkielet-end-to-end-i-platforma-etap-1.md) Szkielet end-to-end i platforma (etap 1) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Platforma, -, MEM-E0-PLAT-21 |
| Moduł | - |
| Warstwa | infrastruktura |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P1 |
| Estymacja | 2–4 h |
| Pewność | średnia |
| Status | do zrobienia |
| Zależy od | [MEM-E0-PLAT-03](MEM-E0-PLAT-03-postgresql-postgis-schemat-i-rola-bazy-na-modul-im.md); [MEM-E0-PLAT-09](MEM-E0-PLAT-09-etap-1-vps-compose-nginx-wal-g-comiesieczny-test-o.md); [MEM-E0-PLAT-10](MEM-E0-PLAT-10-obserwowalnosc-opentelemetry-prometheus-loki-grafa.md) |
| Blokuje | - |
| Kod roboczy | MEM-E0-PLAT-21 |
| Źródło | luka po PLAT-03: rola na moduł = pula na moduł, pule i `max_connections` na wartościach domyślnych (2026-10-25) |

## Cel

Suma wszystkich pul połączeń (każda rola modułu, messaging, scheduler) we wszystkich procesach (API, worker) nigdy nie przekracza limitu serwera PostgreSQL, a zbliżanie się do limitu jest widoczne, zanim użytkownik zobaczy błąd.

## Kontekst

- D-128 (rola na moduł), D-133 (Wolverine), D-155 (Quartz): każda rola to osobna pula Npgsql w każdym procesie
- Dziś bez ustawień: `Maximum Pool Size` = 100 na pulę (domyślnie Npgsql), `max_connections` = 100 (domyślnie PostgreSQL). Teoretyczna suma pul przekracza limit serwera, a rośnie z każdym modułem z bazą
- Objawy przekroczenia: `FATAL: sorry, too many clients already` (53300) po stronie serwera, `The connection pool has been exhausted` po stronie aplikacji
- Rate limiting w nginx (logowanie, 10/min na IP) nie chroni pul

## Zakres

- Zasada wymiarowania w `docs/decisions.md`: suma (Max Pool Size × procesy) ≤ `max_connections` − rezerwa (administrator, backup, restore-test, monitoring, migracje)
- Jawne `Maximum Pool Size` i `Timeout` w connection stringach wszystkich środowisk (`deploy/*.env.example`, `deploy/local/*.env`, compose lokalny)
- `max_connections` (i pamięć: `shared_buffers`, `work_mem`) w konfiguracji PostgreSQL etapu 1, dobrane do RAM serwera
- Kontrola w CI: skrypt liczy sumę sufitów pul z plików env i porównuje z `max_connections` minus rezerwa
- Metryka i alert: zużycie połączeń serwera (`pg_stat_activity` / `numbackends` względem `max_connections`), alert powyżej 80%; metryki pul Npgsql w OpenTelemetry
- Krótki test obciążenia lokalnie: pula modułu wyczerpana nie blokuje innych modułów; błąd puli widoczny w logach i metrykach

## Poza zakresem

- PgBouncer (wyzwalacz: suma pul nie mieści się w rozsądnym `max_connections` albo wiele instancji API w etapie 2; połączenia Wolverine wtedy poza trybem transaction, bo advisory locki sesji)
- limit współbieżności żądań w API (`ConcurrencyLimiter`): decyzja po teście obciążenia

## Kryteria akceptacji

- [ ] Każdy connection string ma jawny `Maximum Pool Size`, a suma sufitów mieści się w `max_connections` minus rezerwa. *Sprawdzenie: CI — skrypt kontroli*
- [ ] Alert na zużycie połączeń > 80% działa i ma test reguły. *Sprawdzenie: CI — promtool test rules*
- [ ] Wyczerpanie puli jednego modułu nie blokuje innych modułów. *Sprawdzenie: test obciążenia lokalnie na serwerze etapu 1*

## Ryzyka i pytania

- Rozmiar RAM serwera etapu 1 nieznany (dostawca VPS otwarty), więc `max_connections` do potwierdzenia po wyborze maszyny
