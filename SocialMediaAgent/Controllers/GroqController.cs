using Microsoft.AspNetCore.Mvc;
using SocialMediaAgent.Models.Request;
using SocialMediaAgent.Repositories.Interfaces;

namespace SocialMediaAgent.Controllers
{
    [Route("")]
    [ApiController]
    public class GroqController : ControllerBase
    {
        private readonly IGroqService _groqService;

        public GroqController(IGroqService groqService)
        {
            _groqService = groqService;
        }

        [HttpPost("generate-post")]
        public async Task<IActionResult> GeneratePost(GroqPromptRequest groqPrompt)
        {
            if (string.IsNullOrEmpty(groqPrompt.Prompt))
            {
                return BadRequest("Prompt is required.");
            }

            var postContent = await _groqService.GenerateSocialMediaPost(groqPrompt);

            if (string.IsNullOrEmpty(postContent))
            {
                return StatusCode(500, "Failed to generate content from Groq.");
            }

            return Ok(new { PostContent = postContent });
        }
    }
}
