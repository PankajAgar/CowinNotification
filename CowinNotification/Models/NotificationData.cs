using System.Collections.Generic;

namespace CowinNotification.Models
{
    public class NotificationData
    {
        public string SearchType { get; set; }
        public IReadOnlyCollection<int> PinCodes { get; set; }
        public string StateName { get; set; }
        public string DistrictName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? AgeLimit { get; set; }
        public int? MinimumAvailableCapacity { get; set; }
        public int? MinimumAvailableCapacityDose1 { get; set; }
        public int? MinimumAvailableCapacityDose2 { get; set; }
        public string FeeType { get; set; }
        public string Vaccine { get; set; }
    }
}