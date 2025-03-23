using System.Text;
using System.Text.Json;
using SocialMediaAgent.Models.Request;
using SocialMediaAgent.Models.Response;
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

        //public async Task<bool> BingTelex(TelexRequest telexRequest)
        //{
        //    if(string.IsNullOrEmpty(telexRequest.Settings.Select(m => m.Default).First()))
        //    {
        //        return false;
        //    }

        //    try{
        //        WriteToFile(telexRequest);
        //        var groqResponse = await _groqService.GenerateSocialMediaPost(new GroqPromptRequest{ Prompt = telexRequest.Message});
        //        TelexMessageResponse telexMessageResponse = new();

        //        if(groqResponse.ToLower().Contains("failed")) //not ideal, fix this.
        //        {                    
        //            telexMessageResponse.event_name = "AI Content Generated";
        //            telexMessageResponse.message = "Unable to generate content at this time, try again later";
        //            telexMessageResponse.status = "failed";
        //            telexMessageResponse.username = "AI-Content Generator";
        //        }

        //        telexMessageResponse.event_name = "AI Content Generated";
        //        telexMessageResponse.message = groqResponse;
        //        telexMessageResponse.status = "success";
        //        telexMessageResponse.username = "SMI Team";

        //        var jsonPayload = JsonSerializer.Serialize(telexMessageResponse);
        //        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        //        var response = await _httpClient.PostAsync($"{telexRequest.Settings[0].Default}", content);
        //        return response.IsSuccessStatusCode ? true : false;               
        //    }
        //    catch(Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        return false;
        //    }
        //}

        public async Task<bool> BingTelex(TelexRequest telexRequest)
        {
            if (telexRequest == null || string.IsNullOrEmpty(telexRequest.Message))
            {
                return false;
            }

            WriteToFile(telexRequest);

            var channelUrl = telexRequest.Settings.FirstOrDefault()?.Default ?? _telexWebhookUrl;
            var platform = telexRequest.Settings.FirstOrDefault(x => x.Label.ToLower() == "platform")?.Default;

            if (string.IsNullOrEmpty(platform))
            {

                WriteToFile(new TelexRequest
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
                // Prompt user for platform
                var promptResponse = new TelexMessageResponse
                {
                    event_name = "Platform Selection Needed",
                    message = "To continue, please go to the app's settings and select a platform (Twitter, Instagram, LinkedIn, Facebook, or TikTok) for your post formatting. Once you've selected a platform, we can tailor the content accordingly.#SMI_DEVS",
                    status = "error",
                    username = "SMI Team"
                };

                var promptPayload = JsonSerializer.Serialize(promptResponse);
                var promptContent = new StringContent(promptPayload, Encoding.UTF8, "application/json");
                var promptResult = await _httpClient.PostAsync(channelUrl, promptContent);

                WriteToFile(new TelexRequest
                {
                    Message = "Promt to tell user to select platform sent to telex ",
                    Settings = new List<Settings>
                    {
                        new Settings
                        {
                            Label = "Channel URL",
                            Type = "text",
                            Required = true,
                            Default = channelUrl,
                            Options = new List<string>()
                        }
                    }
                });
                return promptResult.IsSuccessStatusCode;
            }

            try
            {
                // Format prompt with platform-specific instructions
                var fullPrompt = await FormatPromptWithPlatform(telexRequest.Message, platform);
                var groqResponse = await _groqService.GenerateSocialMediaPost(new GroqPromptRequest { Prompt = fullPrompt });

                var telexMessage = new TelexMessageResponse
                {
                    event_name = "AI Content Generated",
                    message = $"{groqResponse}\n#groq",
                    status = "success",
                    username = "SMI Team"
                };

                var jsonPayload = JsonSerializer.Serialize(telexMessage);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(channelUrl, content);

                WriteToFile(new TelexRequest
                {
                    Message = $"Logging Generated response :{groqResponse}",
                    Settings = new List<Settings>()
                {
                    new Settings()
                    {
                       Label = "channelId",
                       Type = "text",
                       Required = true,
                       Default = channelUrl,
                    },
                    new Settings()
                    {
                       Label = "Platform",
                       Type = "text",
                       Required = true,
                       Default = platform,
                    }
                }
                });
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                WriteToFile(new TelexRequest
                {
                    Message = $"Exeption:{ex.Message}",
                    Settings = new List<Settings>()
                    {
                        new Settings()
                        {
                            Label = "Error",
                            Type = "text",
                            Required = false,
                            Default = "Exception during BingTelex",
                            Options = new List<string>()
                        }
                    }
                });
                return false;
            }
        }

        public async Task<string> FormatPromptWithPlatform(string keyword, string platform)
        {
            var lowerPlatform = platform?.Trim().ToLower();

            var instructions = lowerPlatform switch
            {
                "twitter" => "Format this as a Twitter post (max 280 characters, concise, 2-3 relevant hashtags).",
                "instagram" => "Format this as an Instagram caption (use emojis, spaced lines, and 5-10 hashtags).",
                "linkedin" => "Format for LinkedIn with a professional tone, short headline, paragraph, and 1-3 hashtags.",
                "facebook" => "Format this for Facebook with a short, friendly message and emojis.",
                "tiktok" => "Format this for TikTok, using an energetic tone, emojis, and 3-5 hashtags. Optionally suggest a video idea.",
                _ => "Generate a generic social media post."
            };
            return $"{keyword}\n\n{instructions}";
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
