# MEM-E0-PLAT-20: Dead letters: runbook, skrypt przeglądu i ponowienia, test replayu

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E0](epiki/E0-szkielet-end-to-end-i-platforma-etap-1.md) Szkielet end-to-end i platforma (etap 1) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Platforma, -, MEM-E0-PLAT-20 |
| Moduł | - |
| Warstwa | infrastruktura |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P1 |
| Estymacja | 2–3 h |
| Pewność | wysoka |
| Status | do zrobienia |
| Zależy od | [MEM-E0-PLAT-02](MEM-E0-PLAT-02-outbox-inbox-w-wolverine-postgresql-bez-brokera-w-.md); [MEM-E0-PLAT-09](MEM-E0-PLAT-09-etap-1-vps-compose-nginx-wal-g-comiesieczny-test-o.md); [MEM-E0-PLAT-10](MEM-E0-PLAT-10-obserwowalnosc-opentelemetry-prometheus-loki-grafa.md) |
| Blokuje | - |
| Kod roboczy | MEM-E0-PLAT-20 |
| Źródło | luka po PLAT-02 i PLAT-10: alert `MessageDeadLettered` nie ma procedury obsługi (2026-10-25) |

## Cel

Gdy przyjdzie alert `MessageDeadLettered`, osoba na dyżurze wie, co zrobić: przegląda martwe wiadomości, decyduje i ponawia je jedną komendą, a alert prowadzi prosto do instrukcji.

## Kontekst

- Reguły: ADR 0006 (outbox, Wolverine), D-137 (ponowienia 1 s / 10 s / 1 min, potem dead letters), SD6 (idempotentne handlery), QS12 (zablokowana wiadomość wykryta)
- Wolverine 6.51.3: tabela `wolverine.wolverine_dead_letters` z kolumną `replayable`; agent trwałości w workerze przenosi oznaczone wiersze z powrotem do kolejki przychodzącej (`IDeadLetters.MarkDeadLetterEnvelopesAsReplayableAsync`, `EditAndReplayAsync`)
- Zmiany w schemacie `wolverine` tylko rolą `memorio_messaging` (D-134)

## Zakres

- `deploy/messaging/dead-letters.sh` na serwerze etapu 1 (styl skryptów z `deploy/backup/`):
  - `list`: zestawienie według typu zdarzenia, handlera i typu wyjątku, z liczbą i najstarszą datą
  - `show <id>`: pełny wyjątek i treść jednej wiadomości
  - `replay <id>|--all`: oznaczenie `replayable = true` i czekanie, aż wiadomość zniknie z dead letters
  - `discard <id> --reason "..."`: świadome usunięcie, z wpisem do logu operacji
- `docs/runbooks/dead-letters.md`: kroki (zobacz, przeczytaj wyjątek, zdecyduj: bug / chwilowa awaria / złe dane / wiadomość bez znaczenia, ponów, potwierdź licznik 0)
- `runbook_url` w adnotacji alertu `MessageDeadLettered` (i na wzór w pozostałych alertach z runbookiem)
- Test w `Memorio.Messaging.Tests`: handler pada, wiadomość trafia do dead letters, replay, handler ją obsługuje, licznik dead letters wraca do 0

## Poza zakresem

- panel www do dead letters
- automatyczny replay bez decyzji człowieka

## Kryteria akceptacji

- [ ] Martwa wiadomość ponowiona skryptem jest obsłużona przez handler, a licznik dead letters wraca do 0. *Sprawdzenie: test na Testcontainers w CI + ręcznie na lokalnym serwerze etapu 1*
- [ ] `list` i `show` pokazują typ, handler, wyjątek i treść bez ręcznego SQL. *Sprawdzenie: ręcznie na lokalnym serwerze etapu 1*
- [ ] Alert `MessageDeadLettered` ma `runbook_url` do `docs/runbooks/dead-letters.md`. *Sprawdzenie: CI — promtool check rules + test reguł*
- [ ] Skrypt przechodzi shellcheck. *Sprawdzenie: CI — job stage1*

## Ryzyka i pytania

- Kanał alertów i dyżur wciąż otwarte (HS-006): runbook zakłada, że alert do kogoś dociera
