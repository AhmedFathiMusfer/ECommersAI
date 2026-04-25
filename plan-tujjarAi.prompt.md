## Completed Implementation Plan: ASP.NET Core 9 ECommersAI MVP

This document finalizes the execution plan for the current workspace and aligns it with the required Tujjar AI architecture and features.

## Current Status (Already Implemented)

1. Project skeleton exists and builds as ASP.NET Core 9 Web API.
2. Core entities are present:
   - Trader
   - Product
   - ProductImage
   - ProductAttribute
   - Order
   - OrderItem
   - ExchangeRate
   - Message
   - ProductVector
3. DbContext is present with pgvector extension and relationships.
4. Repository layer exists for all entities with DI registration in Program.cs.
5. PostgreSQL and pgvector packages are already referenced in the csproj.

## Remaining Work to Complete MVP

### Phase 4: Services and Business Logic

1. Create service contracts in Services/Interfaces:
   - ITraderService
   - IProductService
   - IOrderService
   - IExchangeRateService
   - IMessageService
   - IAIService
   - IWhatsAppService
2. Implement async service classes in Services:
   - TraderService
   - ProductService
   - OrderService
   - ExchangeRateService
   - MessageService
   - OpenAIService
   - WhatsAppService
3. Add business rules:
   - Dynamic pricing by currency from ExchangeRates
   - Order total calculation from order items
   - AI-assisted product search + response generation
   - Auto-order conversion from approved message flow

### Phase 5: DTOs and Mapping

1. Create DTO folders:
   - DTOs/Trader
   - DTOs/Product
   - DTOs/Order
   - DTOs/Message
   - DTOs/Common
2. Add request/response DTOs for all CRUD and custom actions.
3. Add AutoMapper profile in Configurations/MappingProfile.cs.
4. Register AutoMapper in Program.cs.

### Phase 6: Controllers and Endpoints

1. Create REST controllers:
   - TradersController
   - ProductsController
   - ProductImagesController
   - ProductAttributesController
   - OrdersController
   - MessagesController
   - WhatsAppWebhookController
2. Required custom endpoints:
   - GET /products/search?query={query}&traderId={id}&currency={currency}
   - POST /orders/auto-generate
   - POST /webhooks/whatsapp
3. Use DTO-only input/output in all controller actions.

### Phase 7: Background Processing

1. Add BackgroundQueue model:
   - IBackgroundTaskQueue
   - BackgroundTaskQueue
2. Add hosted service:
   - WhatsAppMessageHostedService
3. Flow:
   - Webhook receives message
   - Message queued
   - Hosted worker processes message
   - AI response stored in Messages
   - Optional order auto-generation triggered

### Phase 8: Configurations and Secrets

1. Add strongly typed options in Configurations/Options:
   - OpenAIOptions
   - WhatsAppOptions
2. Extend appsettings.json and appsettings.Development.json:
   - ConnectionStrings:DefaultConnection
   - OpenAI:ApiKey, Model, EmbeddingModel
   - WhatsApp:VerifyToken, AccessToken, PhoneNumberId, BaseUrl
3. Support environment variable overrides for production secrets.

### Phase 9: AI and Vector Features

1. In OpenAIService implement:
   - TranscribeVoiceAsync
   - GenerateEmbeddingAsync
   - GenerateReplyAsync
2. In ProductService implement:
   - UpsertProductVectorAsync(productId)
   - SearchByVectorAsync(query, traderId, topK)
3. Add repository method for pgvector similarity query.
4. Persist embeddings in ProductVectors (vector(1536)).

### Phase 10: Startup and Runtime Readiness

1. Update Program.cs:
   - AddControllers
   - Swagger/OpenAPI setup
   - Register services/repositories/options/hosted service
   - Apply migrations on startup (dev mode)
2. Add EF Core migration scripts and README run section.
3. Seed minimum demo data for Traders, Products, ExchangeRates.

## Proposed Folder Layout (Final)

- ECommersAI/
  - Actions/
    - AutoGenerateOrderAction.cs
  - Configurations/
    - MappingProfile.cs
    - Options/
      - OpenAIOptions.cs
      - WhatsAppOptions.cs
  - Controllers/
    - TradersController.cs
    - ProductsController.cs
    - ProductImagesController.cs
    - ProductAttributesController.cs
    - OrdersController.cs
    - MessagesController.cs
    - WhatsAppWebhookController.cs
  - DTOs/
    - Common/
    - Trader/
    - Product/
    - Order/
    - Message/
  - Data/
    - ApplicationDbContext.cs
    - Migrations/
  - Models/
    - Entities/
  - Repositories/
  - Services/
    - Interfaces/
    - Implementations/
    - Background/
      - IBackgroundTaskQueue.cs
      - BackgroundTaskQueue.cs
      - WhatsAppMessageHostedService.cs
  - Program.cs
  - appsettings.json

## API Checklist

1. Traders CRUD
2. Products CRUD
3. ProductImages CRUD
4. ProductAttributes CRUD
5. Orders CRUD
6. Product semantic search endpoint
7. Auto-order generation endpoint
8. WhatsApp webhook endpoint

## Acceptance Criteria

1. All repository and service methods are async.
2. All APIs use DTOs (no entity exposure).
3. OpenAI and WhatsApp settings are loaded via options.
4. Background queue handles asynchronous WhatsApp processing.
5. Vector search returns relevant products per trader.
6. Auto-generated orders include currency conversion and total.
7. Application starts cleanly with Swagger and database migrations.

## Verification Plan

1. dotnet restore
2. dotnet ef migrations add InitialCreate
3. dotnet ef database update
4. dotnet build
5. dotnet run
6. Test endpoints via Swagger and ECommersAI.http
7. Simulate WhatsApp webhook payloads
8. Validate vector search and auto-order flow

## Notes

1. Keep naming as ECommersAI in code to match current project and avoid namespace breakage.
2. Prefer adding new layers incrementally to keep compile and tests green at each phase.
3. For local testing, allow mock implementations for AI and WhatsApp clients behind interfaces.
