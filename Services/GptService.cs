
using System.Net.Http.Headers;
using System.Text.Json;

namespace AiLeadBot.Services
{
    public class GptService
    {
        private readonly HttpClient _http;

        public GptService(HttpClient http)
        {
            _http = http;
        }

        public async Task<string> GetReplyAsync(string userMessage)
        {
            var request = new
            {
                model = "gpt-4",
                messages = new[] { new { role = "user", content = userMessage } }
            };

            var req = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
            req.Content = new StringContent(JsonSerializer.Serialize(request));
            req.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var res = await _http.SendAsync(req);
            var json = await res.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
        }
    }
}