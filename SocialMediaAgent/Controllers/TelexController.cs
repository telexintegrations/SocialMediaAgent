using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialMediaAgent.Repositories.Interfaces;

namespace SocialMediaAgent.Controllers
{
    [Route("")]
    [ApiController]
    public class TelexController : ControllerBase
    {
        private readonly ITelex _telex;

        public TelexController(ITelex telex)
        {
            _telex = telex;
        }
        [HttpGet]
        public IActionResult Post()
        {
            // Send message to Telex
            return Ok("This Api is Active");
        }

        [HttpGet("integration.json")]
        public async Task<IActionResult> GetTelexConfig()
        {
            var result = await _telex.GetTelexConfig();
            return Ok(result);
        }
    }
}
