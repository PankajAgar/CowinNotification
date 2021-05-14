using Newtonsoft.Json;

namespace CowinNotification.Models
{
    public class State
    {
        [JsonProperty("state_id")]
        public int Id { get; set; }

        [JsonProperty("state_name")]
        public string Name { get; set; }
    }
}