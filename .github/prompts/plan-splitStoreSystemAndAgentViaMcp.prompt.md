## Plan: Split Store System and Agent via MCP

نوصي بفصل الذكاء الاصطناعي إلى خدمة مستقلة ASP.NET Core (Agent) مع إبقاء منطق المتجر والبيانات في ECommersAI. يقوم ECommersAI بدور MCP Server ويعرض Tools عبر Streamable HTTP. يقوم Agent بدور MCP Client ويستهلك هذه الأدوات لإدارة الاستدلال والردود. هذا يحقق عزلًا معماريًا واضحًا، مع الحفاظ على التوافق مع تدفق WhatsApp الحالي.

**Execution Checklist (واضح وسريع)**

1. إنشاء مشروعين داخل الحل: `ECommerceAI.Agent` و`ECommerceAI.Contracts`.
2. نقل DTOs الخاصة بالأدوات إلى `ECommerceAI.Contracts` وتثبيت أسماء الأدوات النهائية.
3. في `ECommersAI`: إضافة MCP Server عبر Streamable HTTP مع API Key middleware.
4. تنفيذ 4 tools وربطها بالخدمات الحالية: `search_products`, `convert_currency`, `get_chat_history`, `append_message`.
5. إضافة tenant validation على `traderId` + correlation logging لكل tool call.
6. في `ECommerceAI.Agent`: بناء MCP Client (`IMcpToolClient`) واستهلاك tools من `ECommersAI`.
7. نقل منطق agent orchestration إلى المشروع الجديد وإزالة أي اعتماد مباشر على خدمات المتجر.
8. في `ECommersAI`: استبدال `IAgentService` المحلي بـ `IAgentGateway` يستدعي Agent API.
9. تحديث `AIController` و`WhatsAppService` ليعملوا بنفس السلوك الحالي مع forwarding خارجي.
10. تطبيق resiliency (timeout/retry/circuit-breaker) بين الخدمتين.
11. تنظيف `ECommersAI`: إزالة Semantic Kernel registrations المحلية بعد نجاح التحويل.
12. تشغيل اختبار E2E كامل + اختبارات أمان API Key وعزل التجار + اختبارات failure modes.

**Steps**

1. Phase 1 - Define Boundaries and Contracts
2. تثبيت حدود المسؤوليات:
3. ECommersAI: بيانات، منطق متجر، exchange rates، messages history، webhook intake.
4. Agent Service: orchestration, prompt policy, tool-calling decisions, response generation.
5. تعريف عقود الأدوات MCP (Tool Schemas) كـ DTOs مشتركة في مكتبة Contracts مستقلة (مثلا `ECommerceAI.Contracts`) لتجنب ازدواجية النماذج. _blocks all later phases_
6. الأدوات الأساسية في النطاق الأول:
7. `search_products` (query, traderId, optional topK)
8. `convert_currency` (amount, fromCurrency, toCurrency)
9. `get_chat_history` (traderId, customerPhone, limit)
10. `append_message` (traderId, customerPhone, role, content, metadata)
11. (اختياري حسب الحاجة) `get_trader_profile` لإعداد السياق التجاري.

12. Phase 2 - Implement MCP Server in ECommersAI
13. إضافة MCP Server endpoint(s) في ECommersAI عبر Streamable HTTP.
14. تنفيذ handlers لكل Tool وربطها بالخدمات الحالية:
15. `search_products` -> `IProductService` (بدون EF داخل الكنترولر).
16. `convert_currency` -> `IExchangeRateService`.
17. `get_chat_history` + `append_message` -> `IMessageService`.
18. إضافة طبقة تفويض/تحقق `traderId` لمنع cross-tenant access. _depends on step 5_
19. إضافة API Key auth خدمة-إلى-خدمة (header ثابت + config rotation policy). _parallel with step 18_
20. إضافة logging + correlationId لكل استدعاء Tool لسهولة التتبع.

21. Phase 3 - Create Agent Project (MCP Client)
22. إنشاء مشروع جديد ASP.NET Core باسم `ECommerceAI.Agent` داخل الحل.
23. نقل مكونات الذكاء الاصطناعي من `ECommersAI/Features/AI` إلى المشروع الجديد:
24. `AgentService`, plugin/tool adapters, AI options, orchestration logic.
25. إزالة الاعتماد المباشر على `IProductService` و`IExchangeRateService` داخل agent plugins واستبدالها بطبقة MCP Client (`IMcpToolClient`).
26. تهيئة MCP Client عبر Streamable HTTP مع API Key وإعدادات base URL من config. _depends on phase 2_
27. إبقاء voice/embedding قرار معماري:
28. embedding generation يمكن أن يبقى داخل Agent.
29. أي عملية تحتاج بيانات متجر تتم عبر MCP tools فقط.

30. Phase 4 - Rewire ECommersAI to External Agent
31. في ECommersAI، استبدال الاستدعاء المحلي لـ `IAgentService` بعميل خارجي `IAgentGateway` يستدعي `ECommerceAI.Agent` endpoint.
32. تحديث `AIController` و`WhatsAppService` ليحافظا على نفس تدفق الإدخال الحالي لكن مع forwarding إلى Agent.
33. تثبيت contract موحد للطلب/الاستجابة بين ECommersAI وAgent لضمان backward compatibility.
34. إضافة timeout/retry/circuit-breaker بسياسة واضحة (مثلا Polly) لمنع تعليق webhook requests. _parallel after step 31_

35. Phase 5 - Cleanup and De-coupling Hardening
36. إزالة تسجيلات Semantic Kernel ومكونات AI من `ECommersAI/Program.cs` بعد اكتمال التحويل.
37. حذف/ترحيل الملفات القديمة في `ECommersAI/Features/AI` التي لم يعد لها استخدام.
38. إبقاء DTOs المشتركة في مشروع Contracts فقط وتحديث المراجع.
39. مراجعة appsettings: فصل إعدادات MCP Server (في ECommersAI) عن إعدادات MCP Client/LLM (في Agent).

40. Phase 6 - Verification and Rollout
41. اختبارات تكامل لأدوات MCP في ECommersAI (happy path + unauthorized + tenant isolation).
42. اختبار end-to-end:
43. WhatsApp webhook -> ECommersAI -> Agent -> MCP tools -> Agent response -> ECommersAI outbound message.
44. اختبارات failure modes:
45. MCP timeout، tool unavailable، invalid traderId، invalid currency.
46. مراقبة القياسات: latency p95 بين الخدمتين ومعدل أخطاء tools.
47. اعتماد rollout تدريجي عبر feature flag: `UseExternalAgent=true` مع fallback مؤقت إلى local path أثناء الانتقال.

**Relevant files**

- `ECommersAI/Program.cs` — إضافة MCP server wiring، auth، logging، ثم إزالة AI المحلي لاحقا.
- `ECommersAI/Controllers/AIController.cs` — التحويل من local `IAgentService` إلى `IAgentGateway` خارجي.
- `ECommersAI/Services/WhatsAppService.cs` — إبقاء ingestion محليًا مع forward للـ Agent.
- `ECommersAI/Services/ProductService.cs` — إعادة استخدامه خلف Tool `search_products`.
- `ECommersAI/Services/ExchangeRateService.cs` — إعادة استخدامه خلف Tool `convert_currency`.
- `ECommersAI/Services/MessageService.cs` — إعادة استخدامه خلف Tools history/messages.
- `ECommersAI/Features/AI/services/AgentService.cs` — مرجع للمنطق الذي سينتقل إلى مشروع Agent.
- `ECommersAI/Features/AI/Plugins/InventoryPlugin.cs` — تحويله من direct service dependency إلى MCP tool calls.
- `ECommersAI/Features/AI/Plugins/PricingPlugin.cs` — تحويله من direct service dependency إلى MCP tool calls.
- `ECommerceAI.sln` — إضافة مشروع `ECommerceAI.Agent` ومشروع contracts.

**Verification**

1. Build الحل بعد إضافة المشاريع الجديدة: `dotnet build ECommerceAI.sln`.
2. تشغيل ECommersAI ثم اختبار MCP tools عبر HTTP client مع API Key صالح/غير صالح.
3. تشغيل Agent والتحقق أنه يستدعي `search_products` و`convert_currency` عبر MCP client فعليا (مع logs).
4. تنفيذ سيناريو محادثة كامل من `POST /api/ai/chat` والتحقق من تخزين history في ECommersAI.
5. اختبار WhatsApp ingestion path للتأكد من عدم وجود كسر سلوكي.
6. قياس زمن الاستجابة وإضافة حدود timeout مناسبة قبل اعتماد الإنتاج.

**Decisions**

- النقل: Streamable HTTP.
- Agent runtime: ASP.NET Core (.NET).
- WhatsApp webhook يبقى في ECommersAI.
- المصادقة بين الخدمتين: API Key.
- chat history source of truth: ECommersAI عبر MCP tools.
- in scope: فصل agent بالكامل + MCP server/client + عقود الأدوات الأساسية.
- out of scope: استبدال قاعدة البيانات، إعادة تصميم business domain، أو multi-agent orchestration.

**Further Considerations**

1. Tool versioning recommendation: إضافة `toolVersion` في metadata من البداية لتفادي كسر التوافق لاحقًا.
2. Idempotency recommendation: في `append_message` استخدم messageId اختياري لمنع التكرار وقت retries.
3. Security recommendation: تقييد MCP endpoints على شبكة داخلية/VPN حتى مع وجود API Key.
