namespace SocialMediaAgent.Models.Request
{
    public class ScheduledPostRequest
    {
        public string PostContent { get; set; } // Content of the post to be scheduled
        public string ChannelId { get; set; }   // The ID of the Telex channel where the post will be sent
        public DateTime ScheduledTime { get; set; } // The time at which the post should be sent
    }
}
