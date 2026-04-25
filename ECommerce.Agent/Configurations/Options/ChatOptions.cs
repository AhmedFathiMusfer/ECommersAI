namespace ECommerce.Agent.Configurations.Options;

public sealed class ChatOptions
{
    public const string SectionName = "Chat";

    public string SystemPrompt { get; set; } = "You are a helpful e-commerce assistant.";
}
