namespace SocialMediaAgent.Models.Request
{
    public class ScheduledPostRequest
    {
        public string PostContent { get; set; } // The content of the post
        public DateTime ScheduledTime { get; set; } // The time the post should be published
        public string Platform { get; set; } // The platform the post is targeted for (e.g., Twitter, Instagram)
    }
}
