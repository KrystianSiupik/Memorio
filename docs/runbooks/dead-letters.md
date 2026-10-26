# Runbook: dead letters (MEM-E0-PLAT-20)

Alert `MessageDeadLettered`: an integration event failed in a module's handler on every attempt (1 s,
10 s, 1 min; D-137) and now waits in `wolverine.wolverine_dead_letters`. Nothing is lost and nothing
repeats by itself: the event waits for a person's decision. The other modules handled it already (each
handler has its own queue), so only this module's reaction to it is missing.

All commands run on the application server as root in `/opt/memorio/deploy`.

## 1. See what failed

```bash
./messaging/dead-letters.sh list
```

The first table groups the dead letters by **event**, **handler** (the module's queue) and **exception**,
with the count and the oldest; the second lists them with their ids. Many messages with one exception in
one handler are usually one cause.

## 2. Read the exception

```bash
./messaging/dead-letters.sh show <id>
```

The exception's type and message and the event (ids and facts only, never content of users). For the
stack trace, search Loki (Grafana, *Explore*) for the handler's log around `sent_at`.

## 3. Decide

| What the exception says | Cause | Do |
|---|---|---|
| Timeout, connection refused, 503, `NpgsqlException` while the database or a provider was down | a passing outage, over by now | replay (step 4) |
| A bug in the handler (`NullReferenceException`, a broken rule, a missing case) | the module's code | fix it, release (`tools/deploy/deploy.sh`), then replay. A replay before the fix only fails again |
| The event points at something that does not exist or no longer applies (deleted account, album gone) and the handler should have treated that as done | the handler is not idempotent enough (SD6) | fix the handler to accept it, release, replay |
| The event itself is wrong (bad data from a bug in the publishing module, fixed since) | the publisher | fix the data the event was about if needed, then discard (step 5) |
| The event no longer matters (a module that reacted to it was removed) | — | discard (step 5) |

Not sure: leave it. A dead letter waits without harm; the alert stays on until the count is 0.

## 4. Replay

```bash
./messaging/dead-letters.sh replay <id>     # one
./messaging/dead-letters.sh replay --all    # all of them, e.g. after an outage
```

The script marks them, waits until the worker takes them back (seconds), then waits 90 s, the time the
handler's attempts take. It reports success only when none came back. Handlers are idempotent (SD6), so a
replay of an event that was in fact handled before does no harm.

## 5. Discard (only on purpose)

```bash
./messaging/dead-letters.sh discard <id> --reason "duplicate of 4f1c…, fixed by MEM-…"
```

Deletes it and writes who, what and why to `/var/lib/memorio/operations.log`. The module will never react
to that event, so say in the reason what was done instead.

## 6. Confirm

`./messaging/dead-letters.sh list` shows nothing, and in Grafana `memorio_messaging_dead_letters_count`
is 0 (the alert resolves within a few minutes). Write down what failed, the cause and what was done.
