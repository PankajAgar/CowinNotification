[![License: Unlicense](https://img.shields.io/badge/license-Unlicense-blue.svg)](http://unlicense.org/)
[![c-sharpcorner: c-sharpcorner](https://www.c-sharpcorner.com/Images/McnIcon.ico)](https://www.c-sharpcorner.com/blogs/send-email-using-gmail-smtp)
[![twilio: twilio](https://assets.twilio.com/public_assets/console-js/2.11.1/images/favicons/Twilio_64.png)](https://www.twilio.com/console/phone-numbers/verified)

# Cowin Notification

A simple console application that sends SMS/Email notification when vaccination slots are available for requested pin codes using .NET Core and Twilio SMS integration and Email Smtp server. This can be easily integrated with Windows Task Scheduler or Azure to send SMS/Email notification.

## Configuration

1. Create a Twilio trial account and get the `TwilioAccountSid`, `TwilioAuthToken` and `TwilioPhoneNumber`.
2. Create a Gmail account or use existing for sending emails, Or you can use other smtp server.
3. Update the same in `appsettings.json`
4. By default event logging is enable you can set it to false

## Notification Json

1. Update `notificationRequest.json` with following property.
2. PinCodes -> List of pin codes to receive notification
3. Phone to receive notification on message.
4. Email to receive notification on email.
5. AgeLimit to filter response by age limit (not required).
6. FeeType to filter response by fee type (Paid/Free) (not required).

## Task Scheduler

To Create the task scheduler follow the below steps

1. For those who have want to build .net project, build using release and update `CowinScheduler.xml` `Command` property to the new path of CowinNotification.exe in release folder.
2. For those who want to use binary files update `CowinScheduler.xml` `Command` property to the new path `{ReplaceDirectory}/BinaryForScheduler/CowinNotification.exe`.
3. Create task scheduler by importing the  `CowinScheduler.xml` file in Task scheduler
4. For logging check event viewer with source type CowinLog