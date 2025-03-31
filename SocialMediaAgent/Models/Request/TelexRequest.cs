namespace SocialMediaAgent.Models.Request
{
    public class TelexRequest
    {
        public string Message{get; set;}
        public string channel_id{get; set;}
        public string Thread_id{get; set;}
        public string Org_id{get; set;}
        public List<Settings> Settings{get; set;}
        // public AuthSettings Auth_settings{get; set;} TODO:: remove from comment when auth is ready..
    }

    public class Settings{
        public string Label{get; set;} = string.Empty;
        public string Type{get; set;} = string.Empty;
        public bool Required{get; set;}
        public string Default{get; set;}
        public List<string> Options { get; set; }
    }

    public class AuthSettings{
        public string Integration_auth_credentials{get; set;}
        public string Telex_api_key{get; set;}
    }
}