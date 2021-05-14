using Newtonsoft.Json;
using System.Collections.Generic;

namespace CowinNotification.Models
{
    public class CowinStateResponse
    {
        public IReadOnlyCollection<State> States { get; set; }

        [JsonProperty("ttl")]
        public int TotalRecords { get; set; }
    }
}