namespace CowinNotification.Models
{
    public class CowinRequest
    {
        public int PinCode { get; set; }
        public int? AgeLimit { get; set; }
        public string FeeType { get; set; }
    }
}