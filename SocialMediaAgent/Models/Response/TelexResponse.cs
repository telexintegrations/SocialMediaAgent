namespace SocialMediaAgent.Models.Response
{
    public class TelexMessageResponse
    {
        public string event_name{get; set;}
        public string message{get; set;}
        public string status{get; set;}
        public string username{get; set;} = "Social Media Agent";
    }
}