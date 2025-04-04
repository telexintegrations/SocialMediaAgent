using System.Text;
using System.Text.Json;
using SocialMediaAgent.Models.Request;
using SocialMediaAgent.Models.Response;
using SocialMediaAgent.Repositories.Implementation;
using SocialMediaAgent.Repositories.Interfaces;

namespace SocialMediaAgent.Utils{
    public class CommandPallete{

        public static Dictionary<string, Func<string ,IGroqService?, HttpClient, TelexRequest?, Task<bool>>> Commands = new()
        {
            {"generate", GeneratePost},
            {"commands", SendCommands}
        };
        public static async Task<bool> GeneratePost(string telexPingUrl, IGroqService? groqService, HttpClient httpClient, TelexRequest? telexRequest)
        {
            try{
                var platform = telexRequest!.Settings.FirstOrDefault(x => x.Label.ToLower() == "platform")?.Default;
                var tone = telexRequest.Settings.FirstOrDefault(x => x.Label.ToLower() == "tone")?.Default ?? "neutral";
                var style = telexRequest.Settings.FirstOrDefault(x => x.Label.ToLower() == "style")?.Default ?? "standard";
                var audience = telexRequest.Settings.FirstOrDefault(x => x.Label.ToLower() == "audience")?.Default ?? "general";
                var postPurpose = telexRequest.Settings.FirstOrDefault(x => x.Label.ToLower() == "postpurpose")?.Default ?? "informational";

                var fullPrompt = FormatPromptWithPlatform(telexRequest.Message, platform!, tone, style, audience, postPurpose);
                var groqResponse = await groqService!.GenerateSocialMediaPost(new GroqPromptRequest{ Prompt = fullPrompt});
                TelexMessageResponse telexMessageResponse = new();
                
                if(groqResponse.ToLower().Contains("failed"))
                {                    
                    telexMessageResponse.event_name = "AI Content Generated";
                    telexMessageResponse.message = "Unable to generate content at this time, try again later.\n\n #️⃣SocialMediaAgent";
                    telexMessageResponse.status = "failed";
                }
                
                telexMessageResponse.event_name = "AI Content Generated";
                telexMessageResponse.message = $"{groqResponse}\n\n #️⃣SocialMediaAgent";
                telexMessageResponse.status = "success";

                var webhookUrl = telexPingUrl + telexRequest.channel_id;
                var clientResponse = await Client.PostToTelex(httpClient, telexMessageResponse, webhookUrl);
                return clientResponse.IsSuccessStatusCode ? true : false;
                
            }catch(Exception ex)
            {
                CustomLogger.WriteToFile(ex.Message, telexRequest);
                return false;
            }
            
        }

        public static async Task<bool> SendErrorMessage(string telexPingUrl,IGroqService? groqService, HttpClient httpClient, TelexRequest? telexRequest)
        {
            try{
                TelexMessageResponse telexMessageResponse = new(){
                    event_name = telexRequest!.Settings.First().Label,
                    message = telexRequest!.Message,
                    status = "error"
                };
                var webhookUrl = telexPingUrl + telexRequest.channel_id;
                var clientResponse = await Client.PostToTelex(httpClient, telexMessageResponse, webhookUrl);
                return clientResponse.IsSuccessStatusCode ? true : false;
            }catch(Exception ex)
            {
                CustomLogger.WriteToFile(ex.Message, telexRequest);
                return false;
            }
        }
        public static async Task<bool> SendCommands(string telexPingUrl, IGroqService? groqService, HttpClient httpClient, TelexRequest? telexRequest)
        {
            try{
                TelexMessageResponse telexMessageResponse = new(){
                    event_name = "Commands",
                    message = @$"Hello there!
                    Here are the list of available commands:
                                       
                    {string.Join("\n", Commands.Keys)}
                    #️⃣SocialMediaAgent",
                    status = "success"
                };

                var webhookUrl = telexPingUrl + telexRequest.channel_id;
                var response = await Client.PostToTelex(httpClient, telexMessageResponse, webhookUrl);
                return response.IsSuccessStatusCode ? true : false;
            }catch(Exception ex)
            {
                CustomLogger.WriteToFile(ex.Message, telexRequest);
                return false;
            }
        }

        private static string FormatPromptWithPlatform(string keyword, string platform, string tone, string style, string audience, string postPurpose)
        {
            var lowerPlatform = platform?.Trim().ToLower();

            var instructions = lowerPlatform switch
            {

                "twitter" => $"Format this as a Twitter post (max 280 characters, concise, 2-3 relevant hashtags). Tone: {tone}, Style: {style}, Audience: {audience}, Purpose: {postPurpose}",
                "instagram" => $"Format this as an Instagram caption (use emojis, spaced lines, and 5-10 hashtags). Tone: {tone}, Style: {style}, Audience: {audience}, Purpose: {postPurpose}",
                "linkedin" => $"Format for LinkedIn with a professional tone, short headline, paragraph, and 1-3 hashtags. Tone: {tone}, Style: {style}, Audience: {audience}, Purpose: {postPurpose}",
                "facebook" => $"Format this for Facebook with a short, friendly message and emojis. Tone: {tone}, Style: {style}, Audience: {audience}, Purpose: {postPurpose}",
                "tiktok" => $"Format this for TikTok, using an energetic tone, emojis, and 3-5 hashtags. Optionally suggest a video idea. Tone: {tone}, Style: {style}, Audience: {audience}, Purpose: {postPurpose}",
                 "Discord" => "Format for Discord with a professional tone, short headline, use emojis, paragraph, and 1-3 hashtags.Tone: {tone}, Style: {style}, Audience: {audience}, Purpose: {postPurpose}",
                _ => $"Generate a generic social media post. Tone: {tone}, Style: {style}, Audience: {audience}, Purpose: {postPurpose}"
            };
            return $"{keyword}\n\n{instructions}";
        }
    }
}