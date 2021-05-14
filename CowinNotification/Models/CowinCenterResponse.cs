using System.Collections.Generic;

namespace CowinNotification.Models
{
    public class CowinCenterResponse
    {
        public IReadOnlyCollection<VaccineCenter> Centers { get; set; }
    }
}