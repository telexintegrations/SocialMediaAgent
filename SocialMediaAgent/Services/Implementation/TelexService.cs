
using System.Text;
using System.Text.Json;

namespace SocialMediaAgent.Services.Implementation
{
    public class TelexService : ITelexService
    {
        private readonly HttpClient _httpClient;
        private readonly string _telexWebhookUrl;

        public TelexService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _telexWebhookUrl = configuration["TelexConfig:data:target_url"];
        }
        public async Task<bool> SendMessageToTelex(string message)
        {
            var telexPayload = new
            {
                event_name = "AI Content Generated",
                message = message,
                status = "success",
                username = "AI-Content Generator"
            };

            var jsonPayload = JsonSerializer.Serialize(telexPayload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_telexWebhookUrl, content);
            return response.IsSuccessStatusCode;
        }
    }
}
