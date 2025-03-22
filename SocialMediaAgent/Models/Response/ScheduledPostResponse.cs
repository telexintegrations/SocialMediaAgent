namespace SocialMediaAgent.Models.Response
{
    public class ScheduledPostResponse
    {
        public string PostContent { get; set; }
        public string Platform { get; set; }
        public DateTime ScheduledTime { get; set; }
        public bool IsPosted { get; set; }
    }
}
