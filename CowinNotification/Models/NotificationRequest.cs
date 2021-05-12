using System.Collections.Generic;

namespace CowinNotification.Models
{
    public class NotificationRequest
    {
        public IReadOnlyCollection<NotificationData> NotificationData { get; set; }
    }
}