using SocialMediaAgent.Models.Request;
using SocialMediaAgent.Models.Response;

namespace SocialMediaAgent.Repositories.Interfaces
{
    public interface ITelexService
    {
        Task<TelexConfig> GetTelexConfig();
        Task<bool> SendMessageToTelex(string channelId, GroqPromptRequest promptRequest);
        Task<bool> BingTelex(TelexRequest request);
        Task<bool> RoutePrompt(TelexRequest request);
    }
}
