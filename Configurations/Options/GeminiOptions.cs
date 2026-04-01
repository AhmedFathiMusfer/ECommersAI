namespace ECommersAI.Configurations.Options
{
    public class GeminiOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = "gemini-2.0-flash";
        public string EmbeddingModel { get; set; } = "gemini-embedding-001";
        public int EmbeddingDimensions { get; set; } = 1536;
        public string AudioModel { get; set; } = "gemini-2.0-flash";
        public string BaseUrl { get; set; } = "https://generativelanguage.googleapis.com/v1beta/openai";
        public int RateLimitRetryCount { get; set; } = 3;
        public int InitialRetryDelayMs { get; set; } = 1200;
    }
}