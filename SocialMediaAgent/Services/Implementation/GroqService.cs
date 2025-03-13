using SocialMediaAgent.Models.Request;
using SocialMediaAgent.Services.Interfaces;
using System.Text;
using System.Text.Json;

public class GroqService : IGroqService
{
    private readonly HttpClient _httpClient;
    private readonly string _groqApiKey;
    private readonly string _groqApiUrl;

    public GroqService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _groqApiKey = configuration["GroqConfig:ApiKey"];
        _groqApiUrl = configuration["GroqConfig:ApiUrl"];
    }

    public async Task<string> GenerateSocialMediaPost(GroqPromptRequest promptRequest)
    {
        if (string.IsNullOrEmpty(promptRequest.Prompt))
            throw new ArgumentException("Prompt cannot be empty.", nameof(promptRequest.Prompt));

        var requestData = new
        {
            model = "llama-3.3-70b-versatile",
            messages = new[]
            {
                new 
                { 
                    role = "user", 
                    content = promptRequest.Prompt 
                }
            }
        };

        // Serialize JSON
        var jsonRequest = JsonSerializer.Serialize(requestData);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        // Create HttpRequestMessage
        var request = new HttpRequestMessage(HttpMethod.Post, _groqApiUrl)
        {
            Content = content
        };

        // Add authorization header
        request.Headers.Add("Authorization", $"Bearer {_groqApiKey}");

        var response = await _httpClient.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Groq API request failed: {response.StatusCode} - {responseContent}");
        }

        var jsonResponse = JsonDocument.Parse(responseContent);
        var contentText = jsonResponse.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return contentText ?? "No content generated.";
    }
}
