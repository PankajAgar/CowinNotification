using System.Collections.Generic;

namespace CowinNotification.Models
{
    public class CowinResponse
    {
        public IReadOnlyCollection<VaccineCenter> Centers { get; set; }
    }
}