using Newtonsoft.Json;
using System.Collections.Generic;

namespace CowinNotification.Models
{
    public class VaccineCenter
    {
        [JsonProperty("center_id")]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        [JsonProperty("state_name")]
        public string State { get; set; }

        [JsonProperty("district_name")]
        public string District { get; set; }

        [JsonProperty("block_name")]
        public string Block { get; set; }

        public int Pincode { get; set; }
        public int Lat { get; set; }

        [JsonProperty("@long")]
        public int Long { get; set; }

        public string From { get; set; }
        public string To { get; set; }

        [JsonProperty("fee_type")]
        public string FeeType { get; set; }

        public IReadOnlyCollection<Session> Sessions { get; set; }
    }
}