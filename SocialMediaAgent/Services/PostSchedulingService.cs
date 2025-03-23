using SocialMediaAgent.Models.Request;
using SocialMediaAgent.Repositories.Interfaces;
using System.Threading.Tasks;

namespace SocialMediaAgent.Services
{
    public class PostSchedulingService
    {
        private readonly ITelexService _telexService;

        public PostSchedulingService(ITelexService telexService)
        {
            _telexService = telexService;
        }

        // This method schedules the post to be sent at a specific time
        public async Task<bool> SchedulePost(ScheduledPostRequest scheduledPostRequest)
        {
            // If the scheduled time is in the past, reject the request
            if (scheduledPostRequest.ScheduledTime <= DateTime.Now)
            {
                // Handle invalid scheduled time (can't schedule in the past)
                return false;
            }

            // Calculate the delay between now and the scheduled time
            var delay = scheduledPostRequest.ScheduledTime - DateTime.Now;

            // Wait until the scheduled time to send the post
            await Task.Delay(delay);

            // Create the GroqPromptRequest object, which contains the prompt
            var groqPromptRequest = new GroqPromptRequest
            {
                Prompt = scheduledPostRequest.PostContent // Set the content of the post
            };

            // Send the post to Telex using the ITelexService
            var result = await _telexService.SendMessageToTelex(scheduledPostRequest.ChannelId, groqPromptRequest);

            // Return the result of the send action (true or false)
            return result;
        }
    }
}
