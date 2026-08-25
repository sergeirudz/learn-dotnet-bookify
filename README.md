# Bookify - Clean Architecture

## Commands
```shell

dotnet ef migrations add Create_Database --project ../Bookify.Infrastructure
cd ../Bookify.Infrastructure && dotnet ef migrations add Create_Database --startup-project ../Bookify.Api

```

# Architecture

## Goals

| Goal | Description |
|---|---|
| **Maintainability** | Code should be easy to understand, modify, and extend |
| **Testability** | Components should be verifiable in isolation |
| **Loose coupling** | Minimize dependencies between components |

---

## Design Principles

| Principle | Description |
|---|---|
| **Separation of Concerns** | Separate core domain from system, database, and user interface concerns |
| **Encapsulation** | Hide information from other components; isolate components from external influence |
| **Dependency Inversion** | Depend on abstractions at compile time, implementations at runtime. Interface + Implementation separately (Strategy pattern) |
| **Explicit Dependencies** | Components should honestly declare their dependencies. Improves testability |
| **Single Responsibility** | One responsibility = one reason to change |
| **DRY** | Don't repeat yourself |
| **Persistence Ignorance** | Design domain entities to work regardless of the underlying persistence technology |
| **Bounded Context** | Group related entities and behaviors together |

---

## Architecture Styles

### Onion Architecture

Layers radiate inward toward the Domain:

- **Outer**: Presentation, Infrastructure
- **Middle**: Application
- **Core**: Domain

### Hexagonal Architecture (Ports & Adapters)

- **Core** (Domain + Application) defines ports (interfaces)
- **Adapters** (Infrastructure) implement those ports
- Swappable adapters for DB, messaging, UI, etc.

---

## Layers

### Domain Layer

| Component | Notes |
|---|---|
| **Entities** | Core business objects with identity |
| **Value Objects** | Immutable objects without identity |
| **Domain Events** | Facts that happened in the domain |
| **Domain Services** | Business logic that doesn't naturally fit in an entity |
| **Interfaces** | Abstractions for repository contracts, etc. |
| **Enums** | Strongly-typed constants |
| **Exceptions** | Domain-specific errors |
| **Unit of Work** | Chains multiple repository operations into a single transaction; `SaveChangesAsync` persists all or none |

> `public sealed class Apartment` — `sealed` may provide performance benefits in some scenarios

### Application Layer

- **Orchestrates** the domain — takes entities and tells them what to do
- **Contains business logic** — defines use cases
- **CQRS** (Command Query Responsibility Segregation) with MediatR
    - Separate read models from write models
    - Single responsibility per handler
- **Cross-cutting concerns**
    - Logging
    - Validation
- **Exceptions** — application-specific
- **DI Configuration** — register services

### Infrastructure Layer

| Component | Examples |
|---|---|
| **Databases** | SQL, NoSQL, ORM config |
| **Message Brokers** | RabbitMQ, Kafka, Azure Service Bus |
| **Email Providers** | SendGrid, SMTP |
| **Storage Providers** | Blob storage, S3 |
| **Identity Providers** | Auth0, Azure AD, IdentityServer |
| **System Clock** | `IDateTimeProvider` abstraction |

### Presentation Layer

- Single entry point to the system
- REST API (.NET, gRPC, SPA)
- API Endpoints / Controllers
- Middleware (exception handling, logging, etc.)
- DI setup

---

## Authorization Strategies

| Strategy | Description |
|---|---|
| **Role-based** | Check if user has a specific role (e.g., `Admin`, `User`) |
| **Permission-based** | Check granular permissions (e.g., `bookings:read`, `bookings:write`) |
| **Resource-based** | Check if user has access to a specific resource (e.g., can only edit own booking) |