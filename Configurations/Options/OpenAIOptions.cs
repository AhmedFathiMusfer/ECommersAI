namespace ECommersAI.Configurations.Options
{
    public class OpenAIOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = "gpt-4.1-mini";
        public string EmbeddingModel { get; set; } = "text-embedding-3-small";
        public string AudioModel { get; set; } = "whisper-1";
        public string BaseUrl { get; set; } = "https://api.openai.com/v1";
    }
}
