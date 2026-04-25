---

description: Describe when these instructions should be loaded by the agent based on task context
You are an AI assistant tasked with generating a complete **ASP.NET Core 9** project called "Tujjar AI" – an AI-powered WhatsApp commerce SaaS for Yemeni traders. Follow these instructions exactly

1. **Project Structure**
   - Use **Clean Architecture**:
     - /Controllers
     - /Services
     - /Repositories
     - /Models/Entities
     - /DTOs
     - /Actions
     - /Data (DbContext, Migrations)
     - /Configurations
   - Use **Dependency Injection** for Services & Repositories.
   - Use **async/await** for all I/O operations.
   - Include **HostedService or BackgroundQueue** for async WhatsApp message processing.

2. **Database (PostgreSQL + pgvector)**
   - Include the **pgvector extension** for vector search.
   - Tables:
     - **Traders** (Id, Name, PhoneNumber, WhatsAppId, SubscriptionStatus, DefaultCurrency, CreatedAt, UpdatedAt)
     - **Products** (Id, TraderId, Name, Description, PriceUSD, Category, CreatedAt, UpdatedAt)
     - **ProductImages** (Id, ProductId, ImageUrl, IsMain, CreatedAt, UpdatedAt)
     - **ProductAttributes** (Id, ProductId, AttributeName, AttributeValue)
     - **Orders** (Id, TraderId, CustomerName, CustomerPhone, CustomerAddress, Currency, TotalPrice, Status, CreatedAt, UpdatedAt)
     - **OrderItems** (Id, OrderId, ProductId, Quantity, UnitPriceUSD, ConvertedPrice)
     - **ExchangeRates** (Id, Currency, RateToUSD, Date)
     - **Messages** (Id, TraderId, CustomerPhone, MessageType, Content, AIResponse, CreatedAt)
     - **ProductVectors** (ProductId, Vector vector(1536))
   - Support multiple images per product.
   - Support dynamic product attributes per category.

3. **Features**
   - CRUD APIs for Traders, Products, ProductImages, ProductAttributes, Orders.
   - Automatic total calculation for Orders using ExchangeRates.
   - WhatsApp AI integration:
     - Receive text & voice messages via Webhook.
     - Convert voice → text using OpenAI API.
     - Generate **embeddings** for customer queries using OpenAI Embeddings API.
     - Store embeddings in **ProductVectors (pgvector)**.
     - Search Products by **vector similarity**.
     - Generate AI replies including **dynamic pricing**.
     - Convert approved conversation into **Orders automatically**.
   - Support **multiple currencies** and **dynamic pricing**.
   - Support **multiple product categories** and **multiple images**.

4. **Controllers & API**
   - REST APIs with example endpoints:
     - `GET /products/search?query={query}`
     - `POST /orders/auto-generate`
   - Use DTOs for request/response models.
   - Include AutoMapper or mapping examples.

5. **Configuration**
   - Use **AppSettings** or **Environment variables**:
     - PostgreSQL connection string
     - OpenAI API key
     - WhatsApp API credentials
   - Include sample data seeders for testing.

6. **Extras**
   - Program.cs (Minimal API or Startup.cs style)
   - Sample code for:
     - Receiving WhatsApp messages via Webhook
     - Voice-to-text processing
     - Vector search using pgvector
     - Dynamic pricing calculation
     - Auto-order creation
   - Ready to **run the MVP directly**.

7. **Goal**
   - Generate all necessary **code files** in proper folder structure.
   - Fully working **ASP.NET Core 9 MVP** with database, AI, WhatsApp integration, and vector search.

Instructions for Copilot:

- Start by generating DbContext and Entities.
- Then Repositories and Services.
- Then Controllers and DTOs.
- Include example methods for AI vector search and auto-order generation.
- Ensure async/await is used.

Provide project context and coding guidelines that AI should follow when generating code, answering questions, or reviewing changes.
