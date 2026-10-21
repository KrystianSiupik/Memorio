# Memorio

Map-based app for shared memories and albums. Backend: .NET modular monolith with Domain-Driven
Design and Clean (Onion) Architecture; app: Flutter (iOS, Android, web) in `app/`, see `app/README.md`.
Product, architecture decisions and tickets live in `docs/`.

## Commands

```bash
dotnet build Memorio.slnx                          # warnings are errors
dotnet test --solution Memorio.slnx                # all tests, including architecture tests
dotnet format Memorio.slnx --verify-no-changes     # formatting (dotnet format to fix)
dotnet run --project src/Host/Memorio.Api          # API host
```

Requires the .NET SDK pinned in `global.json`. Tests run on Microsoft.Testing.Platform (xUnit v3).

## Configuration, health, container

Configuration comes only from environment variables (`Section__Key`), validated at startup; an invalid
or missing variable stops the API and the log names the variable, never its value. Required:

| Variable | Meaning |
|---|---|
| `AppVersion__Minimum` | oldest app version the API serves (`major.minor.patch`) |

`dotnet run` takes development values from `Properties/launchSettings.json`. `GET /health` answers while
the process lives; `GET /ready` also checks dependencies (503 when one fails).

```bash
docker build -t memorio-api .                                     # one image for every environment
docker run -p 8080:8080 -e AppVersion__Minimum=0.1.0 memorio-api
```

## API contract (v1)

Every public endpoint is under `/v1` and changes only in backward-compatible ways (ADR 0017).
The contract is committed as `api/openapi.v1.json`; the Dart client in `app/lib/core/api/generated` is
generated from it. After an API change:

```bash
UPDATE_OPENAPI_SNAPSHOT=1 dotnet test --solution Memorio.slnx   # rewrite api/openapi.v1.json, review the diff
tools/generate-api-client.sh                                   # regenerate the Dart client (needs dart on PATH)
tools/check-api-breaking.sh version/0.1                        # breaking changes vs version/0.1 (oasdiff, Docker)
```

The app sends `X-App-Version`; below `AppVersion:Minimum` the API answers 426 (`App.UpdateRequired`,
RFC 7807 problem details) and the app shows the update screen.

## Layout

```
src/
  BuildingBlocks/
    Memorio.SharedKernel/   Result + Error, Entity, AggregateRoot, IDomainEvent; no business logic
    Memorio.Modularity/     IModule: how the host registers a module (endpoints under /v1)
  Host/Memorio.Api/         ASP.NET Core host; ModuleCatalog is the only place that knows every module
  Modules/<Module>/
    Memorio.Modules.<Module>/            the module's inside: Domain, Application, Infrastructure, Api
    Memorio.Modules.<Module>.Contracts/  its public contract (read ports, DTOs, integration events)
tests/
  Memorio.ArchitectureTests/  module and layer rules, each proven by a fixture that breaks it
  Memorio.SharedKernel.Tests/
  Memorio.Api.Tests/
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
- The only public type inside a module is its `IModule` entry point; everything else is `internal`.
- Business failures are returned as `Result` / `Error` (Validation, NotFound, Conflict, Forbidden,
  Failure), mapped to HTTP only at the API edge.
