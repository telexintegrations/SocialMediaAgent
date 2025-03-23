using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialMediaAgent.Models.Request;
using SocialMediaAgent.Repositories.Interfaces;
using SocialMediaAgent.Services; // Ensure to include this namespace for PostSchedulingService
using System;

namespace SocialMediaAgent.Controllers
{
    [Route("")]
    [ApiController]
    public class TelexController : ControllerBase
    {
        private readonly ITelexService _telexService;
        private readonly PostSchedulingService _postSchedulingService; // Inject the PostSchedulingService

        public TelexController(ITelexService telex, PostSchedulingService postSchedulingService)
        {
            _telexService = telex;
            _postSchedulingService = postSchedulingService; // Inject the PostSchedulingService here
        }

        [HttpGet]
        public IActionResult Post()
        {
            // Send message to Telex
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

            var response = await _telexService.BingTelex(telexRequest);
            if (response)
            {
                return StatusCode(202, "Social Media content sent to Telex successfully");
            }

            return StatusCode(400, "Unable to send message to Telex");
        }

        // New Endpoint for scheduling posts
        [HttpPost("schedule-post")]
        public async Task<IActionResult> SchedulePost(ScheduledPostRequest scheduledPostRequest)
        {
            if (scheduledPostRequest == null || string.IsNullOrEmpty(scheduledPostRequest.PostContent))
            {
                return BadRequest("Post content is required.");
            }


            // Call the SchedulePost method from PostSchedulingService
            var result = await _postSchedulingService.SchedulePost(scheduledPostRequest);

            if (result)
            {
                return StatusCode(202, "Post has been scheduled successfully.");
            }

            return StatusCode(400, "Unable to schedule the post.");
        }

    }
}
        // TODO: implement service to send direct message to Telex channel
