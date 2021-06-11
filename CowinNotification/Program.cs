using CowinNotification.Contracts;
using CowinNotification.Models;
using CowinNotification.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CowinNotification
{
    internal class Program
    {
        private static async Task Main()
        {
            if (!EventLog.SourceExists("CowinLog"))
                EventLog.CreateEventSource("CowinLog", "CowinLog");

            var eventLog = new EventLog
            {
                Source = "CowinLog"
            };

            var enableLog = false;

            try
            {
                StringBuilder stringBuilderLog = new StringBuilder();
                var executingDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).ToString();

                try
                {
                    var configuration = new ConfigurationBuilder()
                   .SetBasePath(executingDirectory)
                   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                   .AddEnvironmentVariables()
                   .Build();

                    enableLog = Convert.ToBoolean(configuration.GetSection("EnableLog").Value);

                    var serviceProvider = new ServiceCollection()
                        .AddSingleton<ICowinClient, CowinClient>()
                        .AddSingleton<INotificationSender, NotificationSender>()
                        .AddSingleton(configuration)
                        .BuildServiceProvider();

                    await Start(serviceProvider.GetService<ICowinClient>(), serviceProvider.GetService<INotificationSender>(), executingDirectory, stringBuilderLog);
                }
                catch (Exception ex)
                {
                    stringBuilderLog.AppendLine(ex.Message);
                }

                if (enableLog)
                    eventLog.WriteEntry(stringBuilderLog.ToString(), EventLogEntryType.Information, 1001);
            }
            catch (Exception ex)
            {
                if (enableLog)
                    eventLog.WriteEntry(ex.Message, EventLogEntryType.Error, 1002);
            }
        }

        private static async Task Start(
            ICowinClient cowinClient,
            INotificationSender notificationSender,
            string executingDirectory,
            StringBuilder stringBuilderLog)
        {
            stringBuilderLog.AppendLine("--------------------------------------------");
            stringBuilderLog.AppendLine(DateTime.Now.ToString());

            var path = Path.Combine(executingDirectory, "notificationRequest.json");
            var notificationRequest = JsonConvert.DeserializeObject<NotificationRequest>(File.ReadAllText(path));

            foreach (var notificationData in notificationRequest.NotificationData)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(notificationData.Phone) && string.IsNullOrWhiteSpace(notificationData.Email))
                    {
                        stringBuilderLog.AppendLine("Email and phone both are empty.");
                    }

                    var cowinResponse = await GetAvailableCentersAsync(cowinClient, notificationData, stringBuilderLog);

                    if (cowinResponse.Count > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(notificationData.Phone))
                            await notificationSender.SendSMS(cowinResponse, notificationData.Phone, stringBuilderLog);

                        if (!string.IsNullOrWhiteSpace(notificationData.Email))
                            await notificationSender.SendEmail(cowinResponse, notificationData.Email, executingDirectory, stringBuilderLog);
                    }
                }
                catch (Exception ex)
                {
                    stringBuilderLog.AppendLine(ex.Message);
                }
            }
        }

        private static async Task<IReadOnlyCollection<AvailableCenterAndSlots>> GetAvailableCentersAsync(ICowinClient cowinClient, NotificationData notificationData, StringBuilder stringBuilderLog)
        {
            var cowinRequest = new CowinRequestFilter
            {
                AgeLimit = notificationData.AgeLimit,
                MinimumAvailableCapacity = notificationData.MinimumAvailableCapacity,
                MinimumAvailableCapacityDose1 = notificationData.MinimumAvailableCapacityDose1,
                MinimumAvailableCapacityDose2 = notificationData.MinimumAvailableCapacityDose2,
                FeeType = notificationData.FeeType,
                Vaccine = notificationData.Vaccine
            };
            var response = new List<AvailableCenterAndSlots>();

            if (string.Equals(notificationData.SearchType, "PinCodes", StringComparison.InvariantCultureIgnoreCase))
            {
                if (notificationData.PinCodes.Count == 0)
                {
                    stringBuilderLog.AppendLine("No pin codes to search");
                }
                else
                {
                    foreach (var pinCode in notificationData.PinCodes)
                    {
                        response.AddRange(await cowinClient.GetAvailableCentersByPinCodeAsync(cowinRequest, pinCode));
                    }
                }
            }
            else if (string.Equals(notificationData.SearchType, "DistrictName", StringComparison.InvariantCultureIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(notificationData.StateName) || string.IsNullOrWhiteSpace(notificationData.DistrictName))
                {
                    stringBuilderLog.AppendLine("StateName and DistrictName both are required");
                }
                else
                {
                    response.AddRange(await cowinClient.GetAvailableCentersByDistrictAsync(cowinRequest, notificationData.StateName, notificationData.DistrictName));
                }
            }
            else
            {
                stringBuilderLog.AppendLine($"Invalid search type - {notificationData.SearchType}");
            }

            return response;
        }
    }
}