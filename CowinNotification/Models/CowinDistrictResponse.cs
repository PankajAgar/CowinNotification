using Newtonsoft.Json;
using System.Collections.Generic;

namespace CowinNotification.Models
{
    public class CowinDistrictResponse
    {
        public IReadOnlyCollection<District> Districts { get; set; }

        [JsonProperty("ttl")]
        public int TotalRecords { get; set; }
    }
}