using SocialMediaAgent.Models.Request;
using SocialMediaAgent.Models.Response;
using SocialMediaAgent.Repositories.Interfaces;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SocialMediaAgent.Repositories.Implementation
{
    public class TelexRepository : ITelexService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly string _telexWebhookUrl;
        private readonly IGroqService _groqService;

        public TelexRepository(HttpClient httpClient, IConfiguration configuration, IGroqService groqService)
        {
            _httpClient = httpClient;
            _telexWebhookUrl = configuration["TelexConfig:data:target_url"];
            _configuration = configuration;
            _groqService = groqService;
        }

        // Send direct message to Telex
        public async Task<bool> SendMessageToTelex(string channelId, GroqPromptRequest promptRequest)
        {
            var groqResponse = await _groqService.GenerateSocialMediaPost(promptRequest);
            TelexMessageResponse telexMessageResponse = new();

            // Check for any failure in content generation
            if (groqResponse.ToLower().Contains("failed"))
            {
                telexMessageResponse.event_name = "AI Content Generated";
                telexMessageResponse.message = "Unable to generate content at this time, try again later";
                telexMessageResponse.status = "failed";
                telexMessageResponse.username = "AI-Content Generator";
            }
            else
            {
                telexMessageResponse.event_name = "AI Content Generated";
                telexMessageResponse.message = groqResponse;
                telexMessageResponse.status = "success";
                telexMessageResponse.username = "SMI Team";
            }

            var jsonPayload = JsonSerializer.Serialize(telexMessageResponse);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            // Send message to Telex immediately
            var response = await _httpClient.PostAsync($"{_telexWebhookUrl}/{channelId}", content);
            return response.IsSuccessStatusCode;
        }

        // New method for handling scheduled posts
        public async Task<bool> SendMessageToTelexWithScheduling(string channelId, GroqPromptRequest promptRequest, DateTime scheduledTime)
        {
            if (scheduledTime <= DateTime.Now)
            {
                // If scheduled time is in the past, send immediately
                return await SendMessageToTelex(channelId, promptRequest);
            }
            else
            {
                // Calculate delay until the scheduled time
                var delay = scheduledTime - DateTime.Now;
                await Task.Delay(delay);  // Wait until scheduled time to send the post

                // Once the time arrives, send the post
                return await SendMessageToTelex(channelId, promptRequest);
            }
        }

        public async Task<bool> BingTelex(TelexRequest telexRequest)
        {
            if (string.IsNullOrEmpty(telexRequest.Settings.Select(m => m.Default).First()))
            {
                return false;
            }

            try
            {
                WriteToFile(telexRequest);
                var groqResponse = await _groqService.GenerateSocialMediaPost(new GroqPromptRequest { Prompt = telexRequest.Message });
                TelexMessageResponse telexMessageResponse = new();

                if (groqResponse.ToLower().Contains("failed")) // not ideal, fix this.
                {
                    telexMessageResponse.event_name = "AI Content Generated";
                    telexMessageResponse.message = "Unable to generate content at this time, try again later";
                    telexMessageResponse.status = "failed";
                    telexMessageResponse.username = "AI-Content Generator";
                }

                telexMessageResponse.event_name = "AI Content Generated";
                telexMessageResponse.message = groqResponse;
                telexMessageResponse.status = "success";
                telexMessageResponse.username = "SMI Team";

                var jsonPayload = JsonSerializer.Serialize(telexMessageResponse);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{telexRequest.Settings[0].Default}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<TelexConfig> GetTelexConfig()
        {
            var telexConfig = _configuration.GetSection("TelexConfig").Get<TelexConfig>();
            return telexConfig;
        }

        public bool Test(TelexRequest req)
        {
            try
            {
                WriteToFile(req);
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // Utility method to write request info to a log file
        public static void WriteToFile(TelexRequest req)
        {
            string logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            string filepath = Path.Combine(logDirectory, "log.txt");

            if (!File.Exists(filepath))
            {
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(DateTime.Now + " :: " + req.Settings[0].Default);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(DateTime.Now + " :: " + req.Settings[0].Default);
                }
            }
        }
    }
}
