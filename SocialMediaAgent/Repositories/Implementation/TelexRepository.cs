﻿using System.Text;
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
        private readonly string _telexPingUrl;
        private readonly IGroqService _groqService;
        public TelexRepository(HttpClient httpClient, IConfiguration configuration, IGroqService groqService)
        {
            _httpClient = httpClient;
            _telexWebhookUrl = configuration["TelexConfig:data:target_url"];
            _telexPingUrl = configuration.GetValue<string>("TelexPingBaseUrl");
            _configuration = configuration;
            _groqService = groqService;
        }

        //TO send direct message to telex. :: deprecated method
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
                var response = await _httpClient.PostAsync($"{_telexWebhookUrl}/{channelId}", content);
                return response.IsSuccessStatusCode ? true : false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> BingTelex(Func<string, IGroqService?, HttpClient, TelexRequest?, Task<bool>> function, TelexRequest telexRequest)
        {
            if (telexRequest == null || string.IsNullOrEmpty(telexRequest.Message))
            {
                return false;
            }
            try
            {
                CustomLogger.WriteToFile("Telex call to api with request ",telexRequest);

                var platform = telexRequest.Settings.FirstOrDefault(x => x.Label.ToLower() == "platform")?.Default;

                if (string.IsNullOrEmpty(platform))
                {
                    telexRequest.Settings.First().Label = "Platform Selection Needed";
                    telexRequest.Message = "To continue, please go to the app's settings and select a platform (Twitter, Instagram, LinkedIn, Facebook, Discord or TikTok) for your post formatting. Once you've selected a platform, we can tailor the content accordingly.\n\n #️⃣SocialMediaAgent";
                    var response = await CommandPallete.SendErrorMessage(_telexPingUrl, _groqService, _httpClient, telexRequest);

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
                            Default = "Platform not provided."
                            // Options = new List<string>()
                            }
                        }
                    });

                    return response;
                }

                var _isSuccessful = await function(_telexPingUrl, _groqService, _httpClient, telexRequest);                
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
            var splittedMessage = trimmedMessasge.Split(' ', 2);
            string cmd = splittedMessage.First();

            CustomLogger.WriteToFile($"Trace request:: '{telexRequest.channel_id}'",telexRequest);
            if(CommandPallete.Commands.TryGetValue(cmd, out var function) == true)
            {
                var bingReponse = await BingTelex(function, telexRequest);
                return bingReponse;                
            }
            
            trimmedMessasge = $@"prompt: {trimmedMessasge} +  guideline: if the prompt requires generation of a social media content, just greet me and tell me to use 
            /generate-post command for post generation or use use /commands to see the list of commands for interaction else interact with me normally";
            var groqResponse = await _groqService.GenerateSocialMediaPost(new GroqPromptRequest{Prompt = trimmedMessasge});
            if(groqResponse.Contains("failed")) //TODO :: add param statuscode to this method for better check rather than the .contains(failed)
            {
                return false;
            }
            // var webhookUrl = telexRequest.Settings.FirstOrDefault()?.Default ?? _telexWebhookUrl;

            var channelId = telexRequest?.channel_id ?? "0195dc8e-131b-7874-a3b6-a343cc0332f7";
            var webhookUrl = _telexPingUrl + channelId;
            var telexMessageResponse = new TelexMessageResponse(){
                event_name = "AI Content Generated",
                message = $"{groqResponse}\n\n #️⃣SocialMediaAgent",
                status = "success"
            };

            var clientResponse = await Client.PostToTelex(_httpClient, telexMessageResponse, webhookUrl);
            return clientResponse.IsSuccessStatusCode ? true : false;
        }


        public async Task<TelexConfig> GetTelexConfig()
        {
            var telexConfig = _configuration.GetSection("TelexConfig").Get<TelexConfig>();
            return telexConfig; 
        }

        private string RemoveTags(string message)
        {
            var trimmedMessasge = Regex.Replace(message, @"<\/?p>", "", RegexOptions.IgnoreCase).Trim();
            return trimmedMessasge;
        }
    }
}
