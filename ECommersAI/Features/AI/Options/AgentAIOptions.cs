namespace ECommersAI.Features.AI.Options
{
    public class AgentAIOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = "gemini-2.0-flash";
        public string BaseUrl { get; set; } = "https://generativelanguage.googleapis.com/v1beta/openai";
        public int RateLimitRetryCount { get; set; } = 3;
        public int InitialRetryDelayMs { get; set; } = 1200;
    }
}
