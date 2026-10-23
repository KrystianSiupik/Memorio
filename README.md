# Memorio

Map-based app for shared memories and albums. Backend: .NET modular monolith with Domain-Driven
Design and Clean (Onion) Architecture; app: Flutter (iOS, Android, web) in `app/`, see `app/README.md`.
Product, architecture decisions and tickets live in `docs/`; the patterns we use are in
`docs/architecture/patterns.md`.

## Commands

```bash
dotnet build Memorio.slnx                          # warnings are errors
dotnet test --solution Memorio.slnx                # all tests, including architecture tests
dotnet format Memorio.slnx --verify-no-changes     # formatting (dotnet format to fix)
dotnet run --project src/Host/Memorio.Api          # API host (needs the local database, below)
dotnet run --project src/Host/Memorio.Worker       # worker: delivers integration events to the modules
```

Requires the .NET SDK pinned in `global.json`. Tests run on Microsoft.Testing.Platform (xUnit v3);
`Memorio.Database.Tests`, `Memorio.Messaging.Tests` and `PrivateBucketTests` in `Memorio.Storage.Tests`
also need Docker (they start PostgreSQL + PostGIS or Garage with Testcontainers).

## Configuration, health, container

Configuration comes only from environment variables (`Section__Key`), validated at startup; an invalid
or missing variable stops the host and the log names the variable, never its value. Required:

| Variable | Host | Meaning |
|---|---|---|
| `AppVersion__Minimum` | API | oldest app version the API serves (`major.minor.patch`) |
| `Messaging__ConnectionString` | API, worker | PostgreSQL connection of the role `memorio_messaging` (outbox, inbox, queue) |
| `Storage__Endpoint` | API, worker | S3 API address of the media bucket the hosts call (http(s)) |
| `Storage__PublicEndpoint` | API, worker | optional: address the apps reach the bucket at, used in signed addresses |
| `Storage__Region`, `Storage__Bucket` | API, worker | signing region of the provider, bucket name |
| `Storage__AccessKey`, `Storage__SecretKey` | API, worker | bucket credentials (secrets) |
| `Account__ConnectionString` | API, worker | PostgreSQL connection of the role `memorio_account` (accounts, sign-in) |
| `Account__Google__ClientIds__0`, `__1`, ... | API, worker | optional: our Google app ids (ID token audiences); none: Google sign-in off |
| `Account__Apple__ClientIds__0`, ... | API, worker | optional: our Apple app ids (bundle id, services id); none: Apple sign-in off |
| `Database__MigrateOnStartup` | API, worker | optional, default `true`: apply the modules' migrations at start |

`dotnet run` takes development values from `Properties/launchSettings.json` (local database on port
5432, local bucket on 3900). `GET /health` answers while the process lives; `GET /ready` also checks
dependencies (503 when one fails: the database or the bucket). Both hosts need the database to start:
Wolverine creates its tables then.

```bash
docker build -t memorio-api .                          # one image per host for every environment
docker build --target worker -t memorio-worker .
docker compose --profile app up -d --build              # both on the local database: API :8080, worker :8081
```

## Database (PostgreSQL + PostGIS)

One database, one schema and one role per module (ADR 0004): module `AiNarration` has schema
`ai_narration` and role `memorio_ai_narration`, which owns its schema and has no rights on any other
(SD4, checked by `Memorio.Database.Tests`). `public` holds only PostGIS. `db/bootstrap.sql` creates
all of it; it is idempotent and runs once per environment and again after a module is added.

```bash
docker compose up -d --wait postgres     # local database, bootstrapped on first start (localhost:5432)
tools/db-bootstrap.sh                    # apply db/bootstrap.sql again
```

`POSTGRES_PORT=55432` when Windows reserves port 5432 (then also in `Messaging__ConnectionString`);
`POSTGRES_PASSWORD` sets the owner password (default `memorio_local`). Locally every Memorio role has the
password `memorio_local` (`db/local-passwords.sql`); deployed environments set them from their secrets.

## Messaging: outbox, worker, inbox (ADR 0006)

Modules talk later through integration events (`IIntegrationEvent`, in the publisher's `Contracts`),
with Wolverine and PostgreSQL only, no broker. The event is stored in the outbox in the publisher's own
transaction (`IDbContextOutbox`); the API sends it to the PostgreSQL queue `integration_events`; only
the worker receives it, through the durable inbox (a message id is handled once), and gives every
module handler its own durable queue, so a failing module is retried alone (1 s, 10 s, 1 min, then dead
letters) and the others go on. Delivery is at least once: every module policy is idempotent (SD6, see
`src/Modules/README.md`). Wolverine's tables live in the schemas `wolverine` and `wolverine_queues` of
the role `memorio_messaging`; a module role may only add outbox rows.

The worker publishes backlog metrics (meter `Memorio.Messaging`, sampled every 15 s):
`memorio.messaging.pending.oldest_age` and `.pending.count` per `stage` (`outbox`, `queue`, `inbox`) and
`memorio.messaging.dead_letters.count`. Alert when an age passes 10 minutes or a dead letter appears
(QS12; export and alert rules come with MEM-E0-PLAT-10).

### OpenStreetMap places

Place suggestions come from an OSM import into `places.osm_places`: named places (`place`, `tourism`,
`natural`, `historic`, `leisure`, `amenity` objects; closed ways as their centroid). The table holds
OSM data unchanged and apart from the authors' own places, so ODbL applies to it alone. Each import
replaces the previous one:

```bash
# put an extract (e.g. from download.geofabrik.de) into ./osm (not committed)
docker compose --profile tools run --rm osm-import /data/poland-latest.osm.pbf
docker compose --profile tools run --rm osm-import /samples/sample.osm   # synthetic sample, 3 places
```

In Git Bash on Windows prefix these with `MSYS_NO_PATHCONV=1`, otherwise the container path is rewritten.

## Accounts and sign-in (ADR 0010)

The Account module keeps accounts with ASP.NET Core Identity in its schema. `POST /v1/auth/register`,
`/login`, `/refresh` and `/external/{google|apple}` answer with bearer tokens (access 1 h, refresh 14
days); every other endpoint needs `Authorization: Bearer <access token>`. A new account always needs
the declaration of being at least 16. Apple and Google sign-in: the app sends the ID token from the
native sign-in; the API checks it against the provider's keys and our app ids. `/account/delete` is the
public page on deleting an account. The app keeps the tokens in secure storage and renews them on 401.

```bash
dotnet ef migrations add <Name> --project src/Modules/Account/Memorio.Modules.Account \
  --output-dir Infrastructure/Persistence/Migrations   # after `dotnet tool restore`
```

## Object storage (ADR 0005)

Photos and other media live in one private S3-compatible bucket in the EU (stage 1: the VPS provider's
object storage; stage 2: S3). Nothing in it is public: modules hand out signed addresses through
`IObjectStorage` (SharedKernel), valid for 5 minutes (`ObjectStorageRules.SignatureLifetime`: a
requirement, not configuration; SD5: at most 300 s), signed for a whole gallery at once, after the
Access module allowed it. The app uploads to the temporary zone `uploads/` with a signed PUT (its
content type is signed); the module checks the upload and moves it to its final key
(`PromoteUploadAsync`); whatever stays in `uploads/` expires after a day.

```bash
docker compose up -d --wait garage            # local S3-compatible server (Garage), :3900
docker compose run --rm storage-bootstrap     # bucket memorio-media: upload zone rule + anonymous access check
```

`tools/storage/bootstrap.sh` prepares the bucket of any environment the same way (idempotent, in the
`amazon/aws-cli` image, with `S3_ENDPOINT`, `S3_BUCKET` and the `AWS_*` credentials);
`tools/storage/check-private.sh` is the SD5 check that the bucket refuses anonymous reads, run after
every bootstrap and daily.

## API contract (v1)

Every public endpoint is under `/v1` and changes only in backward-compatible ways (ADR 0017).
The contract is committed as `api/openapi.v1.json`; the Dart client in `app/lib/core/api/generated` is
generated from it. After an API change:

```bash
UPDATE_OPENAPI_SNAPSHOT=1 dotnet test --solution Memorio.slnx   # rewrite api/openapi.v1.json, review the diff
tools/generate-app-code.sh                                   # regenerate the Dart client and other generated code (needs dart on PATH)
tools/check-api-breaking.sh version/0.1                        # breaking changes vs version/0.1 (oasdiff, Docker)
```

The app sends `X-App-Version`; below `AppVersion:Minimum` the API answers 426 (`App.UpdateRequired`,
RFC 7807 problem details) and the app shows the update screen.

## Layout

```
src/
  BuildingBlocks/
    Memorio.SharedKernel/   Result + Error, Entity, AggregateRoot, IDomainEvent, IIntegrationEvent,
                            IObjectStorage; no business logic
    Memorio.Modularity/     IModule: how the host registers a module (endpoints under /v1)
  Host/
    Memorio.Hosting/        shared by both hosts: ModuleCatalog (the only place that knows every module),
                            configuration errors, health endpoints, messaging (Wolverine, backlog metrics),
                            object storage (S3 adapter)
    Memorio.Api/            ASP.NET Core API host (/v1)
    Memorio.Worker/         worker host: integration event delivery (only /health and /ready)
  Modules/<Module>/
    Memorio.Modules.<Module>/            the module's inside: Domain, Application, Infrastructure, Api
    Memorio.Modules.<Module>.Contracts/  its public contract (read ports, DTOs, integration events)
tests/
  Memorio.ArchitectureTests/  module and layer rules, each proven by a fixture that breaks it
  Memorio.SharedKernel.Tests/
  Memorio.Api.Tests/
  Memorio.Database.Tests/     schema and role per module (SD4) against db/bootstrap.sql
  Memorio.Messaging.Tests/    outbox → queue → inbox → module handlers, backlog metric (ADR 0006)
  Memorio.Storage.Tests/      signed addresses (SD5), upload zone, private bucket on Garage (ADR 0005)
  Memorio.Testing/            PostgreSQL + PostGIS test container with the bootstrap applied
db/bootstrap.sql              PostGIS, messaging role and schemas, module schemas and roles
tools/osm-import/             OSM import (osm2pgsql flex style, synthetic sample)
tools/storage/                media bucket: bootstrap and SD5 check scripts, local Garage configuration
```

Modules (from the module map): Access, Account, AiNarration, Album, Archive, Limits, Moderation,
Notifications, PlaceProfiles, Places, Plans, Provenance, Publishing, Succession. PlaceProfiles and
Publishing are outside the MVP and stay empty until their tickets.

## Architecture rules (enforced in CI)

- A module uses another module only through its `Contracts` project (compiler: a module references
  other modules' Contracts only; test SD1). Contracts never expose a module's inside.
- Inside a module: Domain ← Application ← Infrastructure ← Api; dependencies point inwards only (SD2).
- Domain knows no framework (HTTP, EF Core, Wolverine, Quartz, DI); Application knows no HTTP or
  persistence framework.
- The only public types inside a module are its `IModule` entry point and its message handlers (public
  static `*Handler` classes in `Infrastructure.Messaging`, D-135) and its generated EF Core migrations
  (D-152); everything else is `internal`.
- Every message handler has an idempotency test in its module's tests (SD6).
- Every endpoint requires a signed-in account; anonymous ones are explicit and on the allowlist in
  `AnonymousEndpointsTests` (SD10).
- Business failures are returned as `Result` / `Error` (Validation, NotFound, Conflict, Forbidden,
  Failure), mapped to HTTP only at the API edge.
