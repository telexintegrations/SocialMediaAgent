using SocialMediaAgent.Models.Response;
using SocialMediaAgent.Repositories.Interfaces;

namespace SocialMediaAgent.Repositories.Implementation
{
    public class TelexRepository : ITelex
    {
        private readonly IConfiguration _configuration;

        public TelexRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<TelexConfig> GetTelexConfig()
        {
            var telexConfig = _configuration.GetSection("TelexConfig").Get<TelexConfig>();
            return telexConfig;
        }
    }
}
