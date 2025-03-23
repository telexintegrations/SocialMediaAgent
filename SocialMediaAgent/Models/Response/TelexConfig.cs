using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SocialMediaAgent.Models.Response
{
    public class TelexConfig
    {
        public Data data { get; set; } = new Data();
    }

    public class Data
    {
        public Date date { get; set; } = new Date();
        public Descriptions descriptions { get; set; } = new Descriptions();
        public string integration_category { get; set; }
        public string integration_type { get; set; }
        public bool is_active { get; set; }
        public List<string> key_features { get; set; } = new List<string>();
        public List<Setting> settings { get; set; } = new List<Setting>();
        public string target_url { get; set; }
        //public string tick_url { get; set; }
        //public string website    { get; set; }
    }

    public class Date
    {
        public string created_at { get; set; }
        public string updated_at { get; set; }
    }

    public class Descriptions
    {
        public string app_description { get; set; }
        public string app_logo { get; set; }
        public string app_name { get; set; }
        public string app_url { get; set; }
        public string background_color { get; set; }
    }

    public class Setting
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("required")]
        public bool Required { get; set; }

        [JsonProperty("default")]
        public string Default { get; set; }

        [JsonProperty("options")]
        public List<string> Options { get; set; } = new List<string>();
    }
}
