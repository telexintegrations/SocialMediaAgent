using SocialMediaAgent.Models.Request;
using SocialMediaAgent.Models.Response;

namespace SocialMediaAgent.Repositories.Interfaces
{
    public interface ITelexService
    {
        Task<TelexConfig> GetTelexConfig();
        Task<bool> SendMessageToTelex(string channelId, GroqPromptRequest promptRequest);
        Task<bool> BingTelex(Func<string, IGroqService?, HttpClient, TelexRequest?, Task<bool>> function, TelexRequest request);
        Task<bool> RoutePrompt(TelexRequest request);
    }
}
