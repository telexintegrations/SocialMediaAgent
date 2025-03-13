using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialMediaAgent.Models.Request;
using SocialMediaAgent.Repositories.Interfaces;

namespace SocialMediaAgent.Controllers
{
    [Route("")]
    [ApiController]
    public class TelexController : ControllerBase
    {
        private readonly ITelexService _telexService;

        public TelexController(ITelexService telex)
        {
            _telexService = telex;
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
            var result = await _telexService.GetTelexConfig();
            return Ok(result);
        }

        [HttpPost("BingTelex")]
        public async Task<ActionResult> BingTelex(string channelId, GroqPromptRequest promptRequest)
        {
            var response = await _telexService.SendMessageToTelex(channelId, promptRequest);
            if(response)
            {
                return Ok("Social Media content sent to telex succesfully");
            }

            return StatusCode(400, "Unable to send message to telex");
        }
    }
}
