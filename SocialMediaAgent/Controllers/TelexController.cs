using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialMediaAgent.Models.Request;
using SocialMediaAgent.Repositories.Interfaces;
using SocialMediaAgent.Services.Interfaces;  // Make sure you're using the correct interface from Services

namespace SocialMediaAgent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TelexController : ControllerBase
    {
        private readonly ITelexService _telexService;

        // Constructor injects ITelexService
        public TelexController(ITelexService telexService)
        {
            _telexService = telexService;
        }

        // Endpoint to test if the API is active
        [HttpGet]
        public IActionResult Post()
        {
            return Ok("This API is Active");
        }

        // Endpoint to retrieve Telex configuration
        [HttpGet("integration.json")]
        public async Task<IActionResult> GetTelexConfig()
        {
            var result = await _telexService.GetTelexConfig();
            return Ok(result);
        }

        // POST method to send content to Telex
        [HttpPost("BingTelex")]
        public async Task<ActionResult> BingTelex(string channelId, [FromBody] GroqPromptRequest promptRequest)
        {
            // Ensure you send the content to Telex using the service
            var response = await _telexService.SendMessageToTelex(channelId, promptRequest);

            if (response)
            {
                return Ok("Social Media content sent to Telex successfully.");
            }

            // Return a detailed error message when failing
            return StatusCode(500, "Unable to send message to Telex. Please check the service and try again.");
        }
    }
}
