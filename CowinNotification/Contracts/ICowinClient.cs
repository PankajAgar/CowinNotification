using CowinNotification.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CowinNotification.Contracts
{
    public interface ICowinClient
    {
        public Task<IReadOnlyCollection<AvailableCenterAndSlots>> GetAvailableCentersByPinCodeAsync(CowinRequestFilter cowinRequest, int pinCode);

        public Task<IReadOnlyCollection<AvailableCenterAndSlots>> GetAvailableCentersByDistrictAsync(CowinRequestFilter cowinRequest, string state, string district);
    }
}