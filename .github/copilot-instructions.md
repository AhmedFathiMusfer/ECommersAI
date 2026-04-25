# Project Guidelines

## Stack and Scope

- ASP.NET Core Web API targeting .NET 9 in a single backend project: ECommersAI.
- PostgreSQL is required for local development; vector search depends on pgvector.
- Follow existing Clean Architecture folders: Controllers, Services, Repositories, Models/Entities, DTOs, Data, Configurations, Actions.

## Architecture

- Keep controllers thin: validate input, call services, return HTTP results.
- Put business orchestration in services (pricing, AI flow, message handling, order generation).
- Keep data access in repositories and DbContext configuration; avoid embedding EF query logic directly in controllers.
- Use DTOs for request/response boundaries; map entities with AutoMapper (MappingProfile).
- For long-running work (WhatsApp and AI processing), use the background queue/hosted service instead of blocking request threads.

## Build and Run

- Start database: docker compose up -d postgres
- Restore: dotnet restore ECommerceAI.sln
- Build: dotnet build ECommerceAI.sln
- Run API: dotnet run --project ECommersAI/ECommersAI.csproj
- Live reload: dotnet watch --project ECommersAI/ECommersAI.csproj

## Database and Migrations

- Local DB container uses image pgvector/pgvector:pg16 and default postgres credentials from docker-compose.yml.
- App startup applies migrations and seeds data automatically (Database.MigrateAsync + SeedData.EnsureSeededAsync).
- For schema changes:
  - dotnet ef migrations add <MigrationName> --project ECommersAI --startup-project ECommersAI
  - dotnet ef database update --project ECommersAI --startup-project ECommersAI
- When adding/changing entities, update relationships and vector configuration in ApplicationDbContext.

## AI and Vector Conventions

- Embeddings are 1536 dimensions; keep ProductVector column type as vector(1536) and model/output dimensions aligned.
- OpenAI service supports deterministic fallback behavior when API key is missing; keep local-dev behavior non-breaking.
- Keep OpenAI and WhatsApp credentials in configuration (OpenAI and WhatsApp sections), preferably via environment variables/secrets.

## Code Conventions

- Use async/await for all I/O and database operations.
- Register dependencies through DI in Program.cs; keep interfaces in Services/Interfaces and repository abstractions consistent.
- Preserve existing route style (lowercase/plural resource routes with guid constraints).
- Keep pricing logic currency-aware and use ExchangeRate services/repositories for conversions rather than ad-hoc math in controllers.

## Validation and Testing

- There are currently no test projects in this workspace; add tests for service-level business logic when introducing non-trivial behavior.
- Before finishing substantial changes, at minimum run dotnet build and validate impacted endpoints.
