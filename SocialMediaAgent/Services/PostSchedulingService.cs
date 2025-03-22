using SocialMediaAgent.Models.Request;
using SocialMediaAgent.Repositories.Interfaces;

namespace SocialMediaAgent.Services
{
    public class PostSchedulingService
    {
        private readonly ITelexService _telexService;

        public PostSchedulingService(ITelexService telexService)
        {
            _telexService = telexService;
        }

        public async Task<bool> SchedulePost(ScheduledPostRequest scheduledPostRequest)
        {
            if (scheduledPostRequest.ScheduledTime <= DateTime.Now)
            {
                // Handle invalid scheduled time (can't schedule in the past)
                return false;
            }

            var delay = scheduledPostRequest.ScheduledTime - DateTime.Now;
            await Task.Delay(delay); // Wait for the scheduled time

            var result = await _telexService. SendMessageToTelex(scheduledPostRequest.PostContent);
            return result;
        }
    }
}
