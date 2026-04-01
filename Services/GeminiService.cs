using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ECommersAI.Configurations.Options;
using ECommersAI.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace ECommersAI.Services
{
    public class GeminiService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly GeminiOptions _options;

        public GeminiService(HttpClient httpClient, IOptions<GeminiOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public Task<string> TranscribeVoiceAsync(string mediaUrl)
        {
            if (string.IsNullOrWhiteSpace(mediaUrl))
            {
                return Task.FromResult(string.Empty);
            }

            // MVP placeholder: wire WhatsApp media download + multipart upload to Gemini audio endpoint.
            return Task.FromResult($"Transcribed voice from {mediaUrl}");
        }

        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            // if (string.IsNullOrWhiteSpace(_options.ApiKey))
            // {
            return BuildDeterministicEmbedding(text, _options.EmbeddingDimensions > 0 ? _options.EmbeddingDimensions : 1536);
            //  }

            using var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl.TrimEnd('/')}/embeddings")
            {
                Content = JsonContent.Create(new
                {
                    input = text,
                    model = _options.EmbeddingModel,
                    dimensions = _options.EmbeddingDimensions
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

        public async Task<string> GenerateReplyAsync(string prompt, IEnumerable<string> productHints)
        {
            var hints = string.Join("; ", productHints ?? Enumerable.Empty<string>());

            if (string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                return $"I found these products: {hints}. Replying to: {prompt}";
            }

            using var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl.TrimEnd('/')}/chat/completions")
            {
                Content = JsonContent.Create(new
                {
                    model = _options.Model,
                    messages = new object[]
                    {
                        new { role = "system", content = "You are an Arabic/English commerce assistant for Yemeni traders. Return concise product guidance with pricing." },
                        new { role = "user", content = $"Customer prompt: {prompt}\nCandidate products: {hints}" }
                    },
                    temperature = 0.3
                })
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
            using var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var json = await JsonDocument.ParseAsync(stream);
            return json.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? string.Empty;
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