namespace ECommerce.Agent.Configurations.Options;

public sealed class OpenAIOptions
{
    public const string SectionName = "OpenAI";

    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ChatModel { get; set; } = "gpt-4o-mini";
}
