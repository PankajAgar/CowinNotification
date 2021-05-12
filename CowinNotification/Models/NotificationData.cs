using System.Collections.Generic;

namespace CowinNotification.Models
{
    public class NotificationData
    {
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? AgeLimit { get; set; }
        public string FeeType { get; set; }
        public List<int> PinCodes { get; set; }
    }
}