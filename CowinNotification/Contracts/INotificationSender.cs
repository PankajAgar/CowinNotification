using CowinNotification.Models;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CowinNotification.Contracts
{
    public interface INotificationSender
    {
        public Task SendSMS(IReadOnlyDictionary<string, List<AvailableCenterAndSlots>> availableCenter, string phoneNumber, StringBuilder stringBuilderLog);

        public Task SendEmail(IReadOnlyDictionary<string, List<AvailableCenterAndSlots>> availableCenter, string email, string executingDirectory, StringBuilder stringBuilderLog);
    }
}