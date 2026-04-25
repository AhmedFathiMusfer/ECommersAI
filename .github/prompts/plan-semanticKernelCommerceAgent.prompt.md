## Plan: Semantic Kernel AI Agent for Commerce

Integrate Microsoft Semantic Kernel into the current ASP.NET Core 9 architecture by replacing direct chat generation with an SK-based orchestration service that auto-selects InventoryPlugin and PricingPlugin tools, keeps conversation continuity from Messages data, and exposes a dedicated POST /api/ai/chat endpoint with tenant-aware request context.

**Steps**

1. Phase 1: Dependency and contracts
2. Add Semantic Kernel NuGet packages and related OpenAI connector package in e:\ECommerceAI\ECommersAI\ECommersAI.csproj so the app can build Kernel-based services.
3. Extend AI service contracts by introducing a new chat-focused interface in Services/Interfaces (for example IAgentService) that supports trader-aware conversational execution with message history context.
4. Add new request/response DTOs for AI chat endpoint including TraderId, CustomerPhone, Message, and optional currency or locale hints. _parallel with step 3_
5. Phase 2: Plugin implementation
6. Create InventoryPlugin in Services/Plugins/InventoryPlugin.cs with a KernelFunction SearchProducts(string query) that: generates embedding via existing IAIService.GenerateEmbeddingAsync, executes pgvector cosine-distance query through EF Core on ProductVectors filtered by TraderId, returns top 5 concise items (name + USD price).
7. Introduce a product vector query path that avoids in-memory full-table scoring by adding a targeted method in ProductVectorRepository (or a dedicated query repository) using EF.Functions and pgvector operators for cosine distance ordering. _depends on 6_
8. Create PricingPlugin in Services/Plugins/PricingPlugin.cs with KernelFunction CalculateLocalPrice(double priceUsd, string currency) that uses IExchangeRateService and enforces supported currencies YER_OLD, YER_NEW, SAR, returning concise Arabic-formatted output.
9. Phase 3: Semantic Kernel orchestration service
10. Implement AIService.cs (new SK orchestrator) in Services with responsibilities: construct Kernel via DI-injected chat completion service, register InventoryPlugin and PricingPlugin instances, apply the required Arabic system prompt, enable automatic function calling, and return concise Arabic response.
11. Add context continuity logic in AIService: load recent conversation from Messages table using TraderId + CustomerPhone, include previous assistant/user turns in ChatHistory, then append current user message before invocation. _depends on 10_
12. Keep robust async error handling and ILogger logging around kernel execution and tool invocation failures, with safe fallback Arabic responses for transient API/database errors. _parallel with 10/11_
13. Phase 4: API and pipeline integration
14. Create AIController.cs with POST /api/ai/chat endpoint that validates request, calls the SK AI service, returns response DTO, and persists user/assistant turn through existing IMessageService.
15. Register all new services and plugin dependencies in Program.cs while preserving existing WhatsApp/background services.
16. Optionally route existing WhatsAppService reply generation through the same SK AI service so webhook and direct API chat share behavior. _depends on 10 and 15_
17. Phase 5: Prompt and configuration alignment
18. Add/confirm configurable model settings under OpenAI options for SK chat completion and embedding model consistency.
19. Set the exact system prompt content in the orchestrator and ensure explicit tool-use policy: InventoryPlugin for product search intents, PricingPlugin for currency/price intents, always Arabic, concise responses, and context carry-over.
20. Phase 6: verification
21. Build and run smoke tests: POST /api/ai/chat product query should invoke InventoryPlugin; price conversion query should invoke PricingPlugin; multi-turn follow-up should use previous product context.
22. Validate pgvector query performance path and output shape (top 5 name + PriceUSD), and verify Arabic formatting for each supported currency.
23. Confirm logs capture kernel start/end, selected tool, and failures without leaking secrets.

**Relevant files**

- e:\ECommerceAI\ECommersAI\Program.cs — add SK registrations and DI wiring.
- e:\ECommerceAI\ECommersAI\ECommersAI.csproj — add Semantic Kernel package references.
- e:\ECommerceAI\ECommersAI\Services\Interfaces\IAIService.cs — keep embedding/transcription utility role or split interface boundaries.
- e:\ECommerceAI\ECommersAI\Services\OpenAIService.cs — reuse embedding/transcription implementation for plugins where appropriate.
- e:\ECommerceAI\ECommersAI\Services\ProductService.cs — reference existing vector and search behavior; avoid duplicate logic.
- e:\ECommerceAI\ECommersAI\Services\ExchangeRateService.cs — reuse conversion logic for pricing plugin.
- e:\ECommerceAI\ECommersAI\Data\ApplicationDbContext.cs — ensure pgvector mapping remains correct and support query method needs.
- e:\ECommerceAI\ECommersAI\Repositories\ProductVectorRepository.cs — add cosine-distance top-k query entry point.
- e:\ECommerceAI\ECommersAI\Repositories\MessageRepository.cs — source for recent conversation context retrieval.
- e:\ECommerceAI\ECommersAI\Services\Plugins\InventoryPlugin.cs — new SK tool for product retrieval.
- e:\ECommerceAI\ECommersAI\Services\Plugins\PricingPlugin.cs — new SK tool for currency conversion.
- e:\ECommerceAI\ECommersAI\Services\AIService.cs — new SK orchestrator and prompt execution.
- e:\ECommerceAI\ECommersAI\Controllers\AIController.cs — new /api/ai/chat endpoint.
- e:\ECommerceAI\ECommersAI\DTOs\AI\ChatRequestDto.cs — request payload.
- e:\ECommerceAI\ECommersAI\DTOs\AI\ChatResponseDto.cs — response payload.

**Verification**

1. dotnet restore then dotnet build for e:\ECommerceAI\ECommerceAI.sln.
2. Run app and call POST /api/ai/chat with product intent, confirm tool invocation by logs and top-5 product response.
3. Call POST /api/ai/chat with currency intent using YER_OLD, YER_NEW, SAR and verify Arabic formatted conversion.
4. Execute two-turn conversation for same TraderId + CustomerPhone and verify second turn uses previous product context.
5. Regression-check existing endpoints products/search and webhooks/whatsapp remain functional.

**Decisions**

- API request shape: include TraderId and CustomerPhone alongside message.
- Context strategy: persist and retrieve history using Messages table keyed by TraderId + CustomerPhone.
- Included scope: Semantic Kernel chat endpoint, two plugins, DI wiring, pgvector query path, logging/error handling.
- Excluded scope: advanced memory summarization, streaming responses, authorization policy changes, frontend chat UI.

**Further Considerations**

1. If you want stronger tool routing determinism, add a lightweight intent pre-classifier before SK auto-calling; recommended only if model misroutes are observed.
2. If webhook traffic is high, reuse the same AIService in WhatsAppService and add per-customer history window limits to control token cost.
