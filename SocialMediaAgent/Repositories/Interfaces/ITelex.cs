using SocialMediaAgent.Models.Response;

namespace SocialMediaAgent.Repositories.Interfaces
{
    public interface ITelex
    {
        Task<TelexConfig> GetTelexConfig();
    }
}
