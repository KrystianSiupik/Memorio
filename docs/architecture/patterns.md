# Pattern catalog

The patterns Memorio uses, each tied to the problem it solves here. When a problem below comes up,
use the listed pattern in the listed way. A pattern outside this catalog needs a reason recorded in
`docs/decisions.md` (and a row here once accepted). A pattern is never added "just in case".

## Domain

| Problem | Pattern | How in Memorio | Reference |
|---|---|---|---|
| Business rules that must always hold | **Aggregate** | One aggregate root per consistency unit (AG1–AG11); state changes only through methods that check invariants; no public setters; collections read-only | `SharedKernel/Domain/AggregateRoot.cs` |
| Values defined by their content | **Value Object** | Immutable `record`, validated on creation, equality by value | `AiGeneratedText`, `SensitiveName` |
| Identity of an aggregate | **Strongly typed id** | `readonly record struct <Aggregate>Id(Guid Value)`, never a raw `Guid` or string in signatures | ADR 0002 |
| Creating only valid objects | **Factory Method** | Private constructor + `static Create(...)` returning `Result<T>`; `CreateFrom(...)` to rebuild from storage | `AiGeneratedText.Create` |
| Something happened that others react to | **Domain Event** | `record` implementing `IDomainEvent` with `EventId`, raised inside the aggregate (`Raise`), collected on save | `AggregateRoot.Raise` |
| Logic that belongs to no single aggregate | **Domain Service** | Static or stateless class in `Domain/`, pure, no I/O | `Pseudonymizer` |
| Expected business failures | **Result** | `Result` / `Result<T>` with `Error` (Validation, NotFound, Conflict, Forbidden, Failure), code `"<Module>.<Reason>"`; exceptions only for bugs | `SharedKernel/Results` |

## Application

| Problem | Pattern | How in Memorio | Reference |
|---|---|---|---|
| One use case = one change | **Command + Handler** | `Application/Commands/<Feature>/`: command, handler, input validation together; handler returns `Result` | `src/Modules/README.md` |
| One business transaction | **Unit of Work** | One database transaction per command handler, explicit save; events stored in the same transaction (outbox) | ADR 0006 |
| Reads that must stay fast and simple | **CQRS (light)** | Query definition in `Application/Queries`, handler in `Infrastructure` as a read-only projection to a DTO, no business logic, never an aggregate. Write side: EF Core; read side: **Dapper** with parameterized SQL on the module's own schema (D-126) | ADR 0019 |
| Loading and saving aggregates | **Repository** | Interface per aggregate in `Domain/<Aggregate>/`, implementation in `Infrastructure` with EF Core; works on aggregate roots only. No generic repository over EF Core | ADR 0019 |
| External services (AI, storage, stores, clock) | **Ports & Adapters** | Interface in `Application/Ports`, implementation in `Infrastructure/<Provider>`; the domain never knows the provider | `ITextGenerator` → `MistralTextGenerator`; `IObjectStorage` → `S3ObjectStorage` |
| Time in rules and tests | **Clock abstraction** | Inject `TimeProvider` (backend) / `DateTime Function() now` (app); never `DateTime.Now` in domain or handlers | `AiTextService`, `OfflineQueue` |
| A module needs another module's answer now | **Read port / write port** | Interface in the owning module's `Contracts`, implemented inside it; carries only the effect, never the caller's reasons | ADR 0001, map §10 |
| A module reacts later to another module | **Integration event** + **Policy** | Event `record` implementing `IIntegrationEvent` in the publisher's `Contracts`, published through `IDbContextOutbox` in the publisher's unit of work. The reacting module implements `IIntegrationEventPolicy<TEvent>` (internal, `Application`) and a public static `<Event>Handler` in `Infrastructure/Messaging` that only resolves and calls it (D-135). Every policy is idempotent; its handler has `<Handler>IdempotencyTests` in the module tests (SD6) | ADR 0006, `tests/Memorio.Messaging.Tests` |

## Integration and reliability

| Problem | Pattern | How in Memorio | Reference |
|---|---|---|---|
| Saving data and publishing its event atomically | **Transactional Outbox / Inbox** | Wolverine with PostgreSQL storage and a PostgreSQL queue, no broker: the API stores events in the outbox, only the worker receives them, through the durable inbox (duplicates of a message id dropped); each module handler has its own durable queue; retries 1 s, 10 s, 1 min, then dead letters; backlog age metric per stage (D-133..D-137) | ADR 0006, `MemorioMessaging` |
| The same request arriving twice | **Idempotency Key** | Client UUID v7 per operation (offline queue), unique per effect on the server; a retry returns the first result | ADR 0016, D-113, D-118 |
| Long processes over days or weeks | **Process Manager** | Explicit state plus deadlines in Quartz.NET; jobs call aggregates, never change data directly | ADR 0012, succession, 30-day windows |
| Concurrent writes that can break a rule | **Optimistic concurrency** / **conditional update** | `version` column only on aggregates that need it (D-018, D-021); a conditional `UPDATE` for counters and pools (D-016, D-024) | agregaty AG1–AG11 |
| A foreign model (OpenStreetMap, app stores) | **Anti-Corruption Layer** | Translate at the adapter into our own types; foreign ids and terms never cross into the domain | Places, Plans (D-078) |
| Unreliable external services | **Retry + Circuit Breaker + Timeout** | `Microsoft.Extensions.Http.Resilience` standard handler, timeouts set per provider | `AiNarrationModule` |

## Edge and composition

| Problem | Pattern | How in Memorio | Reference |
|---|---|---|---|
| Wiring implementations to interfaces | **Composition Root** | Backend: `ModuleCatalog` + each `<Module>Module.AddServices`; app: `app/composition.dart` (provider overrides) | `ModuleCatalog.cs`, `composition.dart` |
| Behaviour shared by every request | **Middleware / Interceptor** | ASP.NET middleware and Dio interceptors (app version, sign-in and token refresh, later correlation id) | `AppVersionPolicy`, `AppVersionInterceptor`, `AuthInterceptor` |
| Errors over HTTP | **Problem Details** | RFC 7807 with extension `code`; mapping from `Error.Type` only at the API edge: `Problems.From(error)` (Memorio.Modularity) | D-102, D-153 |
| Contract between backend and app | **Contract-first client generation** | OpenAPI snapshot `api/openapi.v1.json`, Dart client generated and committed, oasdiff in CI | ADR 0017 |
| Screens depending only on interfaces | **Dependency Injection via providers** | Riverpod provider typed with a domain interface, default throws, overridden in `app/` and tests | `sessionRepositoryProvider` |
| Variants chosen at run time (e.g. one sender per operation kind) | **Strategy** | A map or interface of implementations registered in the composition root | `operationSendersProvider` |

## Avoid

- **Generic repository** over EF Core (`IRepository<T>`): hides the aggregate boundaries; write one repository per aggregate.
- **Anemic domain + service layer**: rules in handlers or services instead of aggregates.
- **Mediator for everything**: in-process calls inside a module are plain method calls; messages are for commands from the edge and for events between modules.
- **Dapper on the write side** or **EF Core change tracking in queries**: commands load and save aggregates with EF Core; queries read with Dapper (D-126).
- **SQL built by string concatenation**: values always go as Dapper parameters.
- **Shared database tables between modules**: each module owns its schema (ADR 0004); others go through `Contracts`.
- **Static singletons and service locators**: dependencies come through constructors or providers. The one exception: a module's message handler resolves its internal policy from the message scope, because Wolverine runs only public handlers (D-135).
- **Exceptions for business outcomes**: use `Result`.
