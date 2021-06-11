using CowinNotification.Contracts;
using CowinNotification.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CowinNotification.Services
{
    public class CowinClient : ICowinClient
    {
        private const string _cowinBaseURL = "https://cdn-api.co-vin.in/api/v2/";

        public async Task<IReadOnlyCollection<AvailableCenterAndSlots>> GetAvailableCentersByPinCodeAsync(CowinRequestFilter cowinRequest, int pinCode)
        {
            var url = $"{_cowinBaseURL}appointment/sessions/public/calendarByPin?pincode={pinCode}&date={DateTime.Now:dd-MM-yy}";
            return await GetAvailableCentersAsync(cowinRequest, url);
        }

        public async Task<IReadOnlyCollection<AvailableCenterAndSlots>> GetAvailableCentersByDistrictAsync(CowinRequestFilter cowinRequest, string state, string district)
        {
            var stateDetail = await GetStateAsync(state);
            if (stateDetail == null)
                throw new Exception($"No record found for state - {state}");

            var districtDetail = await GetDistrictAsync(stateDetail.Id, district);
            if (districtDetail == null)
                throw new Exception($"No record found for district - {district}");

            var url = $"{_cowinBaseURL}appointment/sessions/public/calendarByDistrict?district_id={districtDetail.Id}&date={DateTime.Now:dd-MM-yy}";
            return await GetAvailableCentersAsync(cowinRequest, url);
        }

        private async Task<State> GetStateAsync(string state)
        {
            var url = $"{_cowinBaseURL}admin/location/states";
            var cowinResponse = await GetCowinResponseAsync<CowinStateResponse>(url);
            return cowinResponse.States.FirstOrDefault(s => string.Equals(s.Name, state, StringComparison.InvariantCultureIgnoreCase));
        }

        private async Task<District> GetDistrictAsync(int stateId, string district)
        {
            var url = $"{_cowinBaseURL}admin/location/districts/{stateId}";
            var cowinResponse = await GetCowinResponseAsync<CowinDistrictResponse>(url);
            return cowinResponse.Districts.FirstOrDefault(s => string.Equals(s.Name, district, StringComparison.InvariantCultureIgnoreCase));
        }

        private async Task<IReadOnlyCollection<AvailableCenterAndSlots>> GetAvailableCentersAsync(CowinRequestFilter cowinRequest, string url)
        {
            var centerAndSlots = new List<AvailableCenterAndSlots>();
            var cowinResponse = await GetCowinResponseAsync<CowinCenterResponse>(url);

            foreach (var vaccineCenter in cowinResponse.Centers)
            {
                if (string.IsNullOrWhiteSpace(cowinRequest.FeeType) || string.Equals(vaccineCenter.FeeType, cowinRequest.FeeType, StringComparison.InvariantCultureIgnoreCase))
                {
                    foreach (var session in vaccineCenter.Sessions)
                    {
                        if (session.AgeLimit == (cowinRequest.AgeLimit ?? session.AgeLimit)
                            && session.AvailableCapacity >= (cowinRequest.MinimumAvailableCapacity ?? 1)
                            && ((cowinRequest.MinimumAvailableCapacityDose1 ?? 0) > 0 ? session.AvailableCapacityDose1 >= cowinRequest.MinimumAvailableCapacityDose1 : true
                            || (cowinRequest.MinimumAvailableCapacityDose2 ?? 0) > 0 ? session.AvailableCapacityDose2 >= cowinRequest.MinimumAvailableCapacityDose2 : true)
                            && (string.IsNullOrWhiteSpace(cowinRequest.Vaccine) || string.Equals(session.Vaccine, cowinRequest.Vaccine, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            centerAndSlots.Add(new AvailableCenterAndSlots
                            {
                                Date = session.Date,
                                PinCode = vaccineCenter.Pincode,
                                CenterName = vaccineCenter.Name,
                                AgeLimit = session.AgeLimit,
                                VaccineName = session.Vaccine,
                                FeeType = vaccineCenter.FeeType,
                                AvailableCapacity = session.AvailableCapacity,
                                AvailableCapacityDose1 = session.AvailableCapacityDose1,
                                AvailableCapacityDose2 = session.AvailableCapacityDose2,
                                Slots = session.Slots
                            });
                        }
                    }
                }
            }
            return centerAndSlots;
        }

        private async Task<T> GetCowinResponseAsync<T>(string url)
        {
            using var client = new HttpClient();
            var apiResponse = await client.GetAsync(url);
            var apiResponseJson = await apiResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(apiResponseJson);
        }
    }
}