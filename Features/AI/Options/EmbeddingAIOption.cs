namespace ECommersAI.Features.AI.Options
{
    public class EmbeddingAIOption
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = "text-embedding-3-small";
        public string BaseUrl { get; set; } = "https://generativelanguage.googleapis.com/v1beta/openai/embeddings";
        public int Dimensions { get; set; } = 1536;
    }
}
