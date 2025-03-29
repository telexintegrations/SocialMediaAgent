using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace SocialMediaAgent.Models.Request
{
    public class TelexRequest
    {
        public string Message { get; set; }
        public List<Settings> Settings { get; set; }

        [JsonPropertyName("channel_id")]
        public string ChannelId { get; set; }

        //[JsonPropertyName("thread_id")]
        //public string? ThreadId { get; set; }

        //[JsonPropertyName("org_id")]
        //public string? OrganizationId { get; set; }
    }

    public class Settings
    {
        public string Label { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool Required { get; set; }
        public string Default { get; set; } = "https://ping.telex.im/v1/webhooks/019591f5-53c5-7867-bc9e-009094a2bce2";
        public List<string> Options { get; set; }
    }
}