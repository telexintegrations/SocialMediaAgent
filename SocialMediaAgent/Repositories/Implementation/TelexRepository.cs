using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using SocialMediaAgent.Models.Request;
using SocialMediaAgent.Models.Response;
using SocialMediaAgent.Repositories.Interfaces;
using SocialMediaAgent.Utils;

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
            try
            {
                var groqResponse = await _groqService.GenerateSocialMediaPost(promptRequest);
                TelexMessageResponse telexMessageResponse = new();
                if (groqResponse.ToLower().Contains("failed"))
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> BingTelex(TelexRequest telexRequest)
        {
            if (telexRequest == null || string.IsNullOrEmpty(telexRequest.Message))
            {
                return false;
            }

            var channelUrl = telexRequest.Settings.FirstOrDefault()?.Default ?? _telexWebhookUrl;
            telexRequest.Settings.First().Default = channelUrl;

            var trimmedMessasge = Regex.Replace(telexRequest.Message, @"<\/?p>", "", RegexOptions.IgnoreCase).Trim();
            var splittedMessage = trimmedMessasge.Split(' ', 2);
            string cmd = splittedMessage.First();
            try
            {
                CustomLogger.WriteToFile("Telex call to api with request ",telexRequest);

                if(CommandPallete.Commands.TryGetValue(cmd, out var function) == false)
                {
                    telexRequest.Settings.First().Label = "Command needed";
                    telexRequest.Message = @"Hello, keyword command not specified.
                    type /commands to see the list of avaliable commands. #SMA_DEVS";

                    var response = await CommandPallete.SendErrorMessage( _groqService, _httpClient, telexRequest);
                    CustomLogger.WriteToFile("Command not selected ", telexRequest);
                    return response;
                }

                var platform = telexRequest.Settings.FirstOrDefault(x => x.Label.ToLower() == "platform")?.Default;

                if (string.IsNullOrEmpty(platform))
                {
                    telexRequest.Settings.First().Label = "Platform Selection Needed";
                    telexRequest.Message = "To continue, please go to the app's settings and select a platform (Twitter, Instagram, LinkedIn, Facebook, or TikTok) for your post formatting. Once you've selected a platform, we can tailor the content accordingly.#SMA_DEVS";
                    var response = await CommandPallete.SendErrorMessage( _groqService, _httpClient, telexRequest);

                    CustomLogger.WriteToFile("platform not selceted", new TelexRequest
                    {
                        Message = "Logging Platform not provided.",
                        Settings = new List<Settings>()
                        {
                            new Settings
                            {
                            Label = "Platform",
                            Type = "text",
                            Required = false,
                            Default = "Platform not provided.",
                            Options = new List<string>()
                            }
                        }
                    });

                    return response;
                }

                var _isSuccessful = await function(_groqService, _httpClient, telexRequest);                
                return _isSuccessful;
            }
            catch (Exception ex)
            {
                CustomLogger.WriteToFile(ex.Message, telexRequest);
                return false;
            }
        }


        public async Task<TelexConfig> GetTelexConfig()
        {
            var telexConfig = _configuration.GetSection("TelexConfig").Get<TelexConfig>();
            return telexConfig;
        }
    }
}
