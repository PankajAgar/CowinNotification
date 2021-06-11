using System.Collections.Generic;

namespace CowinNotification.Models
{
    public class AvailableCenterAndSlots
    {
        public string Date { get; set; }
        public int PinCode { get; set; }
        public string CenterName { get; set; }
        public int AgeLimit { get; set; }
        public string VaccineName { get; set; }
        public int AvailableCapacity { get; set; }
        public int AvailableCapacityDose1 { get; set; }
        public int AvailableCapacityDose2 { get; set; }
        public string FeeType { get; set; }
        public List<string> Slots { get; set; }
    }
}