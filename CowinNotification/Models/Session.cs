using Newtonsoft.Json;
using System.Collections.Generic;

namespace CowinNotification.Models
{
    public class Session
    {
        [JsonProperty("session_id")]
        public string Id { get; set; }

        public string Date { get; set; }

        [JsonProperty("available_capacity")]
        public int AvailableCapacity { get; set; }

        [JsonProperty("available_capacity_dose1")]
        public int AvailableCapacityDose1 { get; set; }

        [JsonProperty("available_capacity_dose2")]
        public int AvailableCapacityDose2 { get; set; }

        [JsonProperty("min_age_limit")]
        public int AgeLimit { get; set; }

        public string Vaccine { get; set; }
        public List<string> Slots { get; set; }
    }
}