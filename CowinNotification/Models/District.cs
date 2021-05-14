using Newtonsoft.Json;

namespace CowinNotification.Models
{
    public class District
    {
        [JsonProperty("district_id")]
        public int Id { get; set; }

        [JsonProperty("district_name")]
        public string Name { get; set; }
    }
}