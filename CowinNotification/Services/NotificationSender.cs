using CowinNotification.Contracts;
using CowinNotification.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace CowinNotification.Services
{
    public class NotificationSender : INotificationSender
    {
        private readonly IConfigurationRoot _config;

        public NotificationSender(IConfigurationRoot config)
        {
            _config = config;
        }

        public async Task SendSMS(IReadOnlyDictionary<string, List<AvailableCenterAndSlots>> availableCenter, string phoneNumber, StringBuilder stringBuilderLog)
        {
            try
            {
                TwilioClient.Init(
                    _config.GetSection("TwilioAccountSid").Value,
                    _config.GetSection("TwilioAuthToken").Value);

                var messageBody = GetSMSBody(availableCenter);
                int length = 0;

                while (length <= messageBody.Length)
                {
                    var count = length + 1500 > messageBody.Length
                        ? messageBody.Length - length - 1
                        : 1500;

                    var smsMessage = await MessageResource.CreateAsync(
                           body: messageBody.Substring(length, count),
                           from: new Twilio.Types.PhoneNumber(_config.GetSection("TwilioPhoneNumber").Value),
                           to: new Twilio.Types.PhoneNumber($"+91{ phoneNumber }")
                       );

                    if (smsMessage.Sid != null)
                        stringBuilderLog.AppendLine($"SMS sent to + 91{ phoneNumber }");

                    length += 1500;
                }
            }
            catch (Exception ex)
            {
                stringBuilderLog.AppendLine(ex.Message);
            }
        }

        public async Task SendEmail(
            IReadOnlyDictionary<string, List<AvailableCenterAndSlots>> availableCenter,
            string email,
            string executingDirectory,
            StringBuilder stringBuilderLog)
        {
            try
            {
                var emailBody = File.ReadAllText(Path.Combine(executingDirectory, "EmailTemplate.html"));

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("GetVaccine@gmail.com", "Be Safe Keep Safe");
                    mail.To.Add(email);
                    mail.Subject = "Cowin Vaccine Available";
                    mail.Body = emailBody.Replace("##RealData##", GetEmailBodyRows(availableCenter));
                    mail.IsBodyHtml = true;
                    using (SmtpClient smtp = new SmtpClient(_config.GetSection("EmailSmtpAddress").Value, Convert.ToInt32(_config.GetSection("EmailPortNumber").Value)))
                    {
                        smtp.Credentials = new NetworkCredential(_config.GetSection("EmailFromAddress").Value, _config.GetSection("EmailPassword").Value);
                        smtp.EnableSsl = Convert.ToBoolean(_config.GetSection("EmailEnableSSL").Value);
                        await smtp.SendMailAsync(mail);
                        stringBuilderLog.AppendLine($"Email sent to { email }");
                    }
                }
            }
            catch (Exception ex)
            {
                stringBuilderLog.AppendLine(ex.Message);
            }
        }

        private string GetSMSBody(IReadOnlyDictionary<string, List<AvailableCenterAndSlots>> availableCenter)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("Vaccination Slots available");

            foreach (var date in availableCenter)
            {
                stringBuilder.Append(" on-> ");
                stringBuilder.Append(date.Key);
                stringBuilder.Append(" at ");

                foreach (AvailableCenterAndSlots center in date.Value)
                {
                    stringBuilder.Append(center.CenterName);
                    stringBuilder.Append(" left ");
                    stringBuilder.Append($"{center.VaccineName}-{center.AvailableCapacity}");
                    stringBuilder.Append(", ");
                }
            }
            return stringBuilder.ToString();
        }

        private string GetEmailBodyRows(IReadOnlyDictionary<string, List<AvailableCenterAndSlots>> availableCenter)
        {
            var stringBuilder = new StringBuilder();

            foreach (var date in availableCenter)
            {
                foreach (AvailableCenterAndSlots center in date.Value)
                {
                    stringBuilder.AppendLine("<tr>");
                    stringBuilder.AppendLine($"<td>{date.Key}</td>");
                    stringBuilder.AppendLine($"<td>{center.CenterName}</td>");
                    stringBuilder.AppendLine($"<td>{center.AgeLimit}</td>");
                    stringBuilder.AppendLine($"<td>{center.VaccineName}</td>");
                    stringBuilder.AppendLine($"<td>{center.AvailableCapacity}</td>");
                    stringBuilder.AppendLine($"<td>{center.FeeType}</td>");
                    stringBuilder.AppendLine($"<td>{ string.Join(',', center.Slots)}</td>");
                    stringBuilder.AppendLine("</tr>");
                }
            }
            return stringBuilder.ToString();
        }
    }
}