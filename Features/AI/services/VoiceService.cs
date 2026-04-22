using ECommersAI.Features.AI.Options;
using Microsoft.Extensions.Options;

namespace ECommersAI.Features.AI.Voice
{
    public class VoiceService : IVoiceService
    {
        private readonly VoiceAIOptions _options;

        public VoiceService(IOptions<VoiceAIOptions> options)
        {
            _options = options.Value;
        }

        public Task<string> TranscribeVoiceAsync(string mediaUrl)
        {
            if (string.IsNullOrWhiteSpace(mediaUrl))
            {
                return Task.FromResult(string.Empty);
            }

            // Placeholder implementation for initial module setup.
            return Task.FromResult($"Transcribed voice from {mediaUrl}");
        }
    }
}
