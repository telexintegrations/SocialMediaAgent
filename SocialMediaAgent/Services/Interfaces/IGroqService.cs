using SocialMediaAgent.Models.Request;

namespace SocialMediaAgent.Services.Interfaces
{
    public interface IGroqService
    {
        Task<string> GenerateSocialMediaPost(GroqPromptRequest request);
    }
}
