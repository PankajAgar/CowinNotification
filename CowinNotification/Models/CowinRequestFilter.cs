namespace CowinNotification.Models
{
    public class CowinRequestFilter
    {
        public int? AgeLimit { get; set; }
        public int? MinimumAvailableCapacity { get; set; }
        public int? MinimumAvailableCapacityDose1 { get; set; }
        public int? MinimumAvailableCapacityDose2 { get; set; }
        public string FeeType { get; set; }
        public string Vaccine { get; set; }
    }
}