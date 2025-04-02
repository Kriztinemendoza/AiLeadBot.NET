
using System.Net.Http.Headers;
using System.Text.Json;

namespace AiLeadBot.Services
{
    public class ElevenLabsService
    {
        private readonly HttpClient _http;

        public ElevenLabsService(HttpClient http)
        {
            _http = http;
        }

        public async Task<string> GenerateAudioUrlAsync(string text)
        {
            var voiceId = Environment.GetEnvironmentVariable("ELEVEN_VOICE_ID");
            var apiKey = Environment.GetEnvironmentVariable("ELEVEN_API_KEY");

            var requestBody = new
            {
                text = text,
                model_id = "eleven_monolingual_v1",
                voice_settings = new { stability = 0.5, similarity_boost = 0.8 }
            };

            var req = new HttpRequestMessage(HttpMethod.Post, $"https://api.elevenlabs.io/v1/text-to-speech/{voiceId}");
            req.Headers.Add("xi-api-key", apiKey);
            req.Content = new StringContent(JsonSerializer.Serialize(requestBody));
            req.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _http.SendAsync(req);
            var audioBytes = await response.Content.ReadAsByteArrayAsync();

            var audioPath = Path.Combine("wwwroot/audio", $"{Guid.NewGuid()}.mp3");
            await File.WriteAllBytesAsync(audioPath, audioBytes);
            var baseUrl = Environment.GetEnvironmentVariable("BASE_URL");

            return $"{baseUrl}/{audioPath.Replace("wwwroot/", "")}";
        }
    }
}
