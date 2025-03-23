using System.Text;
using System.Text.Json;
using SocialMediaAgent.Models.Request;
using SocialMediaAgent.Models.Response;
using SocialMediaAgent.Utils;
using SocialMediaAgent.Repositories.Interfaces;

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

        //TO send direct message to telex.
        public async Task<bool> SendMessageToTelex(string channelId, GroqPromptRequest promptRequest)
        {
            try{
                var groqResponse = await _groqService.GenerateSocialMediaPost(promptRequest);
                TelexMessageResponse telexMessageResponse = new();
                if(groqResponse.ToLower().Contains("failed"))
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
                var response = await _httpClient.PostAsync($"{_telexWebhookUrl}/{channelId}", content);
                return response.IsSuccessStatusCode ? true : false;                  
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<TelexMessageResponse> BingTelex(TelexRequest telexRequest)
        {
            Dictionary<string, Func<TelexRequest, Task<bool>>> CommandPallete = new()
            {
                {"/generate-post", GeneratePost}
            };

            if(string.IsNullOrEmpty(telexRequest.Message))
            {
                return new TelexMessageResponse(){
                    event_name = "Quality check",
                    message = "Cannot send an empty message",
                    status = "failed",
                };
            }

            if(string.IsNullOrEmpty(telexRequest.Settings.Select(m => m.Default).First()))
            {
                return new TelexMessageResponse(){
                    event_name = "Quality check",
                    message = "Channel webhook should be provided in app settings :)",
                    status = "failed",
                };
            }

            try{
                //WriteToFile(telexRequest); TODO:: put this after operation
                var stringParts = telexRequest.Message.Split(' ', 2);
                var cmd = stringParts[0];

                if(CommandPallete[cmd] == null)
                {
                    return new TelexMessageResponse(){
                        event_name = "Quality check",
                        message = @$"Kindly provide command before inputting your prompt.
                        The list of avaliable commands are;
                        {CommandPallete.Select(dictionary => dictionary.Key).ToList()}",
                        status = "failed",
                    };
                }

                telexRequest.Message = stringParts[1];

                var _isGenerated =  await CommandPallete[cmd].Invoke(telexRequest);
                LogTelexResponse.WriteToFile(telexRequest);
                // var response = new TelexMessageResponse();
                return _isGenerated ? new TelexMessageResponse(){event_name = "AI Generated Content", message = "to be put"}
                : new TelexMessageResponse(){event_name = "Error bot", message = "An error occurred, try again later"};
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new TelexMessageResponse(){event_name = "Error bot", message = "We are currently unavaliable, check back later"};
            }
        }
 
        private async Task<bool> GeneratePost(TelexRequest telexRequest)
        {
            var groqResponse = await _groqService.GenerateSocialMediaPost(new GroqPromptRequest{ Prompt = telexRequest.Message});
            TelexMessageResponse telexMessageResponse = new();
            
            if(groqResponse.ToLower().Contains("failed")) //not ideal, fix this.
            {                    
                telexMessageResponse.event_name = "AI Content Generated";
                telexMessageResponse.message = "Unable to generate content at this time, try again later";
                telexMessageResponse.status = "failed";
            }
            
            telexMessageResponse.event_name = "AI Content Generated";
            telexMessageResponse.message = groqResponse;
            telexMessageResponse.status = "success";

            // var jsonPayload = JsonSerializer.Serialize(telexMessageResponse);
            // var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            // var response = await _httpClient.PostAsync($"{telexRequest.Settings[0].Default}", content);

            // return response.IsSuccessStatusCode ? true : false;

            return true;
        }

        public async Task<TelexConfig> GetTelexConfig()
        {
            var telexConfig = _configuration.GetSection("TelexConfig").Get<TelexConfig>();
            return telexConfig;
        }

    }
}
