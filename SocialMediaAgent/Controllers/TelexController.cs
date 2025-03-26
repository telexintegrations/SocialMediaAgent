using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialMediaAgent.Models.Request;
using SocialMediaAgent.Repositories.Interfaces;
using SocialMediaAgent.Services;
using System;

namespace SocialMediaAgent.Controllers
{
    [Route("")]
    [ApiController]
    public class TelexController : ControllerBase
    {
        private readonly ITelexService _telexService;
        private readonly PostSchedulingService _postSchedulingService;

        public TelexController(ITelexService telex, PostSchedulingService postSchedulingService)
        {
            _telexService = telex;
            _postSchedulingService = postSchedulingService;
        }

        [HttpGet]
        public IActionResult Post()
        {
            return Ok("This API is Active");
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
            if (telexRequest == null)
            {
                return StatusCode(400, "Payload required");
            }
            if (telexRequest.Message.Contains("#groq") || telexRequest.Message.Contains("#SMI_DEVS"))
            {
                return StatusCode(400, "Message Already processed.");
            }

            var response = await _telexService.BingTelex(telexRequest);
            if (response)
            {
                return StatusCode(202, "Message has been sent to Telex successfully");
            }

            return StatusCode(400, "Unable to send message to Telex");
        }

        [HttpPost("schedule-post")]
        public async Task<IActionResult> SchedulePost(ScheduledPostRequest scheduledPostRequest)
        {
            if (scheduledPostRequest == null || string.IsNullOrEmpty(scheduledPostRequest.PostContent))
            {
                return BadRequest("Post content is required.");
            }

            var result = await _postSchedulingService.SchedulePost(scheduledPostRequest);

            if (result)
            {
                return StatusCode(202, "Post has been scheduled successfully.");
            }

            return StatusCode(400, "Unable to schedule the post.");
        }

    }
}
