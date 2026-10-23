# Modules

One folder per module (bounded context) from the module map. Each module is two projects:

```
Modules/<Module>/
  Memorio.Modules.<Module>/              the inside; every type internal except <Module>Module
    <Module>Module.cs                    IModule: registers services, maps endpoints under /v1
    Domain/                              aggregates, value objects, domain events, domain services
      <Aggregate>/                       <Aggregate>.cs, <Aggregate>Id.cs, errors, repository interface
    Application/                         use cases; depends on Domain only
      Commands/<Feature>/                command + handler + input validation, one unit of work
      Queries/<Query>/                   query definition only (handler in Infrastructure)
      Ports/                             interfaces for external services (AI, storage, clock...)
      Policies/                          IIntegrationEventPolicy<TEvent>: reactions to other modules' events
    Infrastructure/                      EF Core (repositories), Dapper (query handlers), adapters
      Messaging/                         public static <Event>Handler per integration event it reacts to
    Api/                                 Minimal API endpoints: input → command/query → DTO
  Memorio.Modules.<Module>.Contracts/    public: read ports, DTOs, integration events
```

Rules (checked by `tests/Memorio.ArchitectureTests`, ADR 0001 and 0019):

- Another module is used only through its `Contracts` project (never its inside).
- Inside a module: Domain ← Application ← Infrastructure ← Api.
- Domain knows no framework (HTTP, EF Core, Wolverine, Quartz, DI); Application knows no HTTP or
  persistence framework.
- Business failures are `Result` / `Error` (`Memorio.SharedKernel`), codes "<Module>.<Reason>";
  exceptions only for bugs.
- Aggregates: private constructor + static factory returning `Result`, strongly typed id, state
  changes only through methods that check invariants, domain events raised inside.
- Queries are read-only projections to DTOs with no business logic; they never return aggregates.
  Commands use EF Core, query handlers use Dapper with parameterized SQL on the module's own schema.
- Every query about someone else's content goes through the Access module (ADR 0011).
- Reacting to another module's integration event (ADR 0006): the policy implements
  `IIntegrationEventPolicy<TEvent>` in `Application/Policies` (internal, registered in `AddServices`);
  `Infrastructure/Messaging/<Event>Handler` is a public static class whose `Handle(TEvent, IServiceProvider,
  CancellationToken)` only resolves the policy and calls it (Wolverine runs public handlers only, D-135).
  The worker runs it at least once per event, in the module's own queue: the policy must be idempotent,
  and `tests/Modules/Memorio.Modules.<Module>.Tests` has `<Event>HandlerIdempotencyTests` delivering the
  same event twice (SD6, checked by the architecture tests).

Which pattern solves which problem: `docs/architecture/patterns.md`.

Reference implementation: `AiNarration` (Domain: `Pseudonymizer`, `AiGeneratedText`; Application:
`ITextGenerator` port, `AiTextService`; Infrastructure: Mistral adapter, prompts).

Tests of a module live in `tests/Modules/Memorio.Modules.<Module>.Tests` (the module project has
`InternalsVisibleTo` for it): domain without database or HTTP, adapters with fake handlers.
