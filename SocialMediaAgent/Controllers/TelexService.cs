using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SocialMediaAgent.Models.Request;  // Ensure you have the necessary models

namespace SocialMediaAgent.Services.Implementation
{
    // Implements the ITelexService interface
    public class TelexService : ITelexService
    {
        private readonly HttpClient _httpClient;
        private readonly string _telexWebhookUrl;

        // Constructor to inject HttpClient and IConfiguration
        public TelexService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _telexWebhookUrl = configuration["TelexConfig:data:target_url"];
        }

        //// Method to get Telex Config (optional, for diagnostics)
        //public async Task<TelexConfig> GetTelexConfig()
        //{
        //    // Simulating the retrieval of the TelexConfig from app settings.
        //    var telexConfig = new TelexConfig
        //    {
        //        // Map config properties to model as needed.
        //    };
        //    return await Task.FromResult(telexConfig);
        //}

        // Method to send generated content to Telex via webhook
        public async Task<bool> SendMessageToTelex(string channelId, GroqPromptRequest promptRequest)
        {
            // Create the message object
            var message = new
            {
                event_name = "AI Content Generated", // Customize as needed
                message = promptRequest.Prompt, // Content that we want to send to Telex
                status = "success", // You can change the status as per your needs
                username = "AI-Content Generator" // You can specify the username
            };

            // Serialize the message object to JSON
            var jsonMessage = JsonConvert.SerializeObject(message);
            var httpContent = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

            // Send the POST request to Telex's Webhook URL
            var response = await _httpClient.PostAsync(_telexWebhookUrl, httpContent);

            // Return whether the request was successful
            return response.IsSuccessStatusCode;
        }
    }
}
