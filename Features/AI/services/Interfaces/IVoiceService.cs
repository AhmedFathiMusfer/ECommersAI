namespace ECommersAI.Features.AI.Voice
{
    public interface IVoiceService
    {
        Task<string> TranscribeVoiceAsync(string mediaUrl);
    }
}
