namespace SocialMediaAgent.Services.Implementation
{
    public interface ITelexService
    {
        Task<bool> SendMessageToTelex(string message);
    }
}
