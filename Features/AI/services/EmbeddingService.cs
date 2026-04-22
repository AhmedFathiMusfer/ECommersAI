using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ECommersAI.Features.AI.Options;
using Microsoft.Extensions.Options;

namespace ECommersAI.Features.AI.Embedding
{
    public class EmbeddingService : IEmbeddingService
    {
        private readonly HttpClient _httpClient;
        private readonly EmbeddingAIOption _options;
        private readonly ILogger<EmbeddingService> _logger;

        public EmbeddingService(HttpClient httpClient, IOptions<EmbeddingAIOption> options, ILogger<EmbeddingService> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_options.ApiKey))
                {
                    return BuildDeterministicEmbedding(text, _options.Dimensions > 0 ? _options.Dimensions : 1536);
                }

                using var request = new HttpRequestMessage(HttpMethod.Post, _options.BaseUrl)
                {
                    Content = JsonContent.Create(new
                    {
                        input = text,
                        model = _options.Model,
                        dimensions = _options.Dimensions
                    })
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
                using var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                using var stream = await response.Content.ReadAsStreamAsync();
                using var json = await JsonDocument.ParseAsync(stream);
                var embeddingArray = json.RootElement.GetProperty("data")[0].GetProperty("embedding");

                var result = new float[embeddingArray.GetArrayLength()];
                var index = 0;
                foreach (var value in embeddingArray.EnumerateArray())
                {
                    result[index++] = value.GetSingle();
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating embedding for text: {Text}", text);
                return BuildDeterministicEmbedding(text, _options.Dimensions > 0 ? _options.Dimensions : 1536);
            }
        }

        private static float[] BuildDeterministicEmbedding(string input, int size)
        {
            var normalized = input ?? string.Empty;
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(normalized));
            var vector = new float[size];

            for (var i = 0; i < size; i++)
            {
                var b = bytes[i % bytes.Length];
                vector[i] = (b / 255f) - 0.5f;
            }

            return vector;
        }
    }
}
