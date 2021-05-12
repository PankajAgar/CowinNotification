using CowinNotification.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CowinNotification.Contracts
{
    public interface ICowinClient
    {
        public Task<IReadOnlyDictionary<string, List<AvailableCenterAndSlots>>> FetchAvailableCenters(CowinRequest cowinRequest);
    }
}