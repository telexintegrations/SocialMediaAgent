using SocialMediaAgent.Models.Request;

namespace SocialMediaAgent.Repositories.Interfaces
{
    public interface IGroqService
    {
        Task<string> GenerateSocialMediaPost(GroqPromptRequest request);
    }
}
