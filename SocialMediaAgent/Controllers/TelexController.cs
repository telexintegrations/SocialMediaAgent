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
        public async Task<ActionResult> BingTelex(TelexRequest telexRequest)
        {
            if(telexRequest == null)
            {
                return StatusCode(400, "payload required");
            }
            if (telexRequest.Message.Contains("#groq") || telexRequest.Message.Contains("#SMI_DEVS"))
            {
                return StatusCode(400, "Message Already processed.");
            }

            var response = await _telexService.BingTelex(telexRequest);
            if(response)
            {
                return StatusCode(202,"Social Media content sent to telex succesfully");
            }

            return StatusCode(400, "Unable to send message to telex");
        }

        //TODO:: implement service to send direct message to telex channel
    }
}
