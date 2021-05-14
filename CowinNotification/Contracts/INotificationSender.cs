using CowinNotification.Models;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CowinNotification.Contracts
{
    public interface INotificationSender
    {
        public Task SendSMS(IReadOnlyCollection<AvailableCenterAndSlots> availableCenters, string phoneNumber, StringBuilder stringBuilderLog);

        public Task SendEmail(IReadOnlyCollection<AvailableCenterAndSlots> availableCenters, string email, string executingDirectory, StringBuilder stringBuilderLog);
    }
}