using CowinNotification.Contracts;
using CowinNotification.Models;
using CowinNotification.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
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
                if (string.IsNullOrWhiteSpace(notificationData.Phone) && string.IsNullOrWhiteSpace(notificationData.Email))
                {
                    stringBuilderLog.AppendLine("Email sent and phone both are empty");
                }

                foreach (var pinCode in notificationData.PinCodes)
                {
                    var cowinRequest = new CowinRequest
                    {
                        PinCode = pinCode,
                        AgeLimit = notificationData.AgeLimit,
                        FeeType = notificationData.FeeType
                    };

                    var cowinResponse = await cowinClient.FetchAvailableCenters(cowinRequest);

                    if (cowinResponse.Count > 0)
                    {
                        stringBuilderLog.AppendLine($"Got response for {cowinRequest.PinCode}");

                        if (!string.IsNullOrWhiteSpace(notificationData.Phone))
                            await notificationSender.SendSMS(cowinResponse, notificationData.Phone, stringBuilderLog);

                        if (!string.IsNullOrWhiteSpace(notificationData.Email))
                            await notificationSender.SendEmail(cowinResponse, notificationData.Email, executingDirectory, stringBuilderLog);
                    }
                }
            }
        }
    }
}