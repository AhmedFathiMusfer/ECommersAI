using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommersAI.Services.Interfaces
{
    public interface IAIService
    {
        Task<string> TranscribeVoiceAsync(string mediaUrl);
        Task<float[]> GenerateEmbeddingAsync(string text);
        Task<string> GenerateReplyAsync(string prompt, IEnumerable<string> productHints);
    }
}
