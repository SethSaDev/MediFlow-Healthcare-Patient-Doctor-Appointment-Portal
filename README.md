# Clinic Booking - Clean Architecture Core

[![CI](https://github.com/SethSaDev/clinic-booking-clean-architecture/actions/workflows/ci.yml/badge.svg)](https://github.com/SethSaDev/clinic-booking-clean-architecture/actions/workflows/ci.yml)
![.NET 8](https://img.shields.io/badge/.NET-8-512BD4) ![MediatR](https://img.shields.io/badge/CQRS-MediatR%2012-6E4AFF) ![EF Core 8](https://img.shields.io/badge/EF%20Core-8-512BD4)

The domain and application layers of a clinic appointment system, built with Clean Architecture, CQRS (MediatR) and DDD.
Patients register, sign in and book 30-minute slots with doctors. The booking rules (overlaps, doctor availability, time off) live in the application core and are unit-tested without a database.

> **Scope:** this repository holds the **Domain** and **Application** layers plus their tests. The API and Infrastructure (EF Core configuration, JWT issuing, email/SMS providers) plug in through the interfaces in `Application/Common/Interfaces`.

## Layers

```text
src/
├── MediFlow.Domain/         Entities, enums, value objects (EmailAddress, PhoneNumber, Address),
│                            domain events, Result<T>. No infrastructure dependencies.
└── MediFlow.Application/    Commands/handlers, validators, MediatR pipeline behaviors,
                             ports: IApplicationDbContext, ICurrentUserService, IEmailService,
                             ISmsService, IJwtTokenGenerator
tests/
└── MediFlow.UnitTests/      xUnit + Moq + EF Core InMemory
```

## MediatR pipeline

Every command passes through four behaviors, in this order:

| Behavior | What it does |
|---|---|
| `AuthorizationBehavior` | Enforces `[Authorize(Roles = ...)]` on the request type |
| `ValidationBehavior` | Runs all FluentValidation validators and fails fast with a `ValidationException` |
| `LoggingBehavior` | Logs request name and duration (never the payload, which can hold passwords or clinical data) |
| `PerformanceBehavior` | Warns on requests slower than 500 ms |

## Use cases

| Command | Rules |
|---|---|
| `RegisterPatientCommand` | Unique email (`ConflictException`), BCrypt password hash, assigns the *Patient* role |
| `LoginCommand` | BCrypt verification, refresh-token rotation, JWT via `IJwtTokenGenerator` |
| `BookAppointmentCommand` | Doctor must be available. Rejects overlapping slots and dates inside a doctor's time-off block. Writes an audit-log entry and sends email/SMS confirmation |

## Tests

```bash
dotnet test
```

- **Value objects:** email validation and case-insensitive equality, phone normalization and length rules
- **Booking handler**, against an in-memory `IApplicationDbContext`:
  - books a slot and sends confirmation
  - rejects overlapping slots, but allows back-to-back slots
  - rejects bookings during doctor leave
  - returns not-found for unavailable doctors

These tests caught two defects that would have failed every booking in production: the doctor's `User` navigation wasn't loaded before the confirmation email used it, and the audit snapshot serialized an entity graph with reference cycles.

## Roadmap

- `Infrastructure` project: EF Core configurations and migrations for PostgreSQL, JWT generator, SendGrid/Twilio adapters
- `API` project: minimal APIs over the MediatR commands, ProblemDetails mapping for the application exceptions
- Queue management and consultation notes (entities already modelled in the domain)

## Author

**Seth Sanders** - Senior Software Engineer (.NET, Angular/React, Azure)
[github.com/SethSaDev](https://github.com/SethSaDev)
