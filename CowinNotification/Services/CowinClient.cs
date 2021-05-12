using CowinNotification.Contracts;
using CowinNotification.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace CowinNotification.Services
{
    public class CowinClient : ICowinClient
    {
        public async Task<IReadOnlyDictionary<string, List<AvailableCenterAndSlots>>> FetchAvailableCenters(CowinRequest cowinRequest)
        {
            var response = new Dictionary<string, List<AvailableCenterAndSlots>>();
            var url = "https://cdn-api.co-vin.in/api/v2/appointment/sessions/public/calendarByPin?pincode=" + cowinRequest.PinCode + "&date=" + DateTime.Now.ToString("dd-MM-yy") + "";

            using var client = new HttpClient();
            var apiResponse = await client.GetAsync(url);
            var apiResponseJson = await apiResponse.Content.ReadAsStringAsync();
            var cowinResponse = JsonConvert.DeserializeObject<CowinResponse>(apiResponseJson);

            foreach (var vaccineCenter in cowinResponse.Centers)
            {
                if (string.IsNullOrWhiteSpace(cowinRequest.FeeType) || vaccineCenter.FeeType == cowinRequest.FeeType)
                {
                    foreach (var session in vaccineCenter.Sessions)
                    {
                        if (session.AvailableCapacity > 0
                            && (cowinRequest.AgeLimit == null || session.AgeLimit == cowinRequest.AgeLimit.Value))
                        {
                            if (response.ContainsKey(session.Date))
                                response[session.Date].Add(
                                    new AvailableCenterAndSlots
                                    {
                                        CenterName = vaccineCenter.Name,
                                        AgeLimit = session.AgeLimit,
                                        VaccineName = session.Vaccine,
                                        FeeType = vaccineCenter.FeeType,
                                        AvailableCapacity = session.AvailableCapacity,
                                        Slots = session.Slots
                                    });
                            else
                                response.Add(
                                    session.Date,
                                    new List<AvailableCenterAndSlots>
                                    {
                                         new AvailableCenterAndSlots
                                         {
                                            CenterName = vaccineCenter.Name,
                                            AgeLimit = session.AgeLimit,
                                            VaccineName = session.Vaccine,
                                            FeeType = vaccineCenter.FeeType,
                                            AvailableCapacity = session.AvailableCapacity,
                                            Slots = session.Slots
                                         }
                                    });
                        }
                    }
                }
            }

            return response;
        }
    }
}