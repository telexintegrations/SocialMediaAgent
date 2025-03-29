using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Channels;
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
                telexMessageResponse.message = $"{groqResponse} \n\n #️⃣SocialMediaAgent";
                telexMessageResponse.status = "success";
                telexMessageResponse.username = "Social Media Agent";

                var jsonPayload = JsonSerializer.Serialize(telexMessageResponse);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var channelUrl = $"https://ping.telex.im/v1/webhooks/{channelId}";
                var response = await _httpClient.PostAsync($"{channelUrl}", content);
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

            CustomLogger.WriteToFile("Telex request received with channel_id: " + telexRequest.ChannelId, telexRequest);

            var trimmedMessasge = Regex.Replace(telexRequest.Message, @"<\/?p>", "", RegexOptions.IgnoreCase).Trim();
            var channelId = telexRequest.ChannelId;
            var channelUrl = $"https://ping.telex.im/v1/webhooks/{channelId}";

            var trimmedMessasge = RemoveTags(telexRequest.Message);
            var splittedMessage = trimmedMessasge.Split(' ', 2);
            string cmd = splittedMessage.First();
            try
            {
                CustomLogger.WriteToFile("Telex call to api with request ", telexRequest);

                if (CommandPallete.Commands.TryGetValue(cmd, out var function) == false)
                {
                    telexRequest.Settings.First().Label = "Command needed";
                    telexRequest.Message = @"Hello, keyword command not specified.
                    type /commands to see the list of avaliable commands.
                    
                    #️⃣SocialMediaAgent";

                    var response = await CommandPallete.SendErrorMessage(_groqService, _httpClient, telexRequest);
                    CustomLogger.WriteToFile("Command not selected ", telexRequest);
                    return response;
                }

                var platform = telexRequest.Settings.FirstOrDefault(x => x.Label.ToLower() == "platform")?.Default;

                if (string.IsNullOrEmpty(platform))
                {
                    telexRequest.Settings.First().Label = "Platform Selection Needed";
                    telexRequest.Message = "To continue, please go to the app's settings and select a platform (Twitter, Instagram, LinkedIn, Facebook, or TikTok) for your post formatting. Once you've selected a platform, we can tailor the content accordingly.\n\n #️⃣SocialMediaAgent";
                    var response = await CommandPallete.SendErrorMessage(_groqService, _httpClient, telexRequest);

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

        public async Task<bool> RoutePrompt(TelexRequest telexRequest)
        {
            var trimmedMessasge = RemoveTags(telexRequest.Message);
            if (CommandPallete.Commands.Keys.Any(trimmedMessasge.Contains))
            {
                var bingReponse = await BingTelex(telexRequest);
                return bingReponse;
            }

            trimmedMessasge = $@"prompt: {trimmedMessasge} +  guideline: if the prompt requires generation of a social media content, just greet me and tell me to use 
            /generate-post command for post generation or use use /commands to see the list of commands for interaction else interact with me normally and tell me what you can do as a Social Media Agent";
            var groqResponse = await _groqService.GenerateSocialMediaPost(new GroqPromptRequest { Prompt = trimmedMessasge });
            if (groqResponse.Contains("failed")) //TODO :: add param statuscode to this method for better check rather than the .contains(failed)
            {
                return false;
            }

            var channelUrl = $"https://ping.telex.im/v1/webhooks/{telexRequest.ChannelId}";
            var telexMessageResponse = new TelexMessageResponse()
            {
                event_name = "AI Content Generated",
                message = $"{groqResponse}\n\n #️⃣SocialMediaAgent",
                status = "success"
            };

            var clientResponse = await Client.PostToTelex(_httpClient, telexMessageResponse, channelUrl);
            return clientResponse.IsSuccessStatusCode ? true : false;
        }


        public async Task<TelexConfig> GetTelexConfig()
        {
            var telexConfig = _configuration.GetSection("TelexConfig").Get<TelexConfig>();
            return telexConfig;
        }
    }
}
