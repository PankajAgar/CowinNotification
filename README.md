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

Update `notificationRequest.json` with following property.

1. `SearchType` to PinCodes or DistrictName 
2. `PinCodes` -> List of pin codes to receive 
3. `StateName` and `DistrictName` -> Get all centers for district
4. `Phone` to receive notification on message.
5. `Email` to receive notification on email.
6. `AgeLimit` to filter response by age limit (18/45)(not required).
7. `MinimumAvailableCapacity` to filter response minimum vaccine available (not required).
8. `MinimumAvailableCapacityDose1` to filter response minimum vaccine available for dose 1(not required).
9. `MinimumAvailableCapacityDose2` to filter response minimum vaccine available for dose 2(not required).
10. `FeeType` to filter response by fee type (Paid/Free) (not required).
11. `Vaccine` to filter response by vaccine (COVAXIN/COVISHIELD/SPUTNIK V) (not required).

## Task Scheduler

To Create the task scheduler which run every two minutes(default), follow the below steps

1. For those who want to build .net project, build using release and update `CowinScheduler.xml` `Command` property to the new path of CowinNotification.exe in release folder also update the `Author` property to your Domain\UserName.
2. For those who want to use binary files update `CowinScheduler.xml` `Command` property to the new path `{ReplaceDirectory}/BinaryForScheduler/CowinNotification.exe` also update the `Author` property to your Domain\UserName.
3. If you want to update the scheduler interval update `CowinScheduler.xml` `Interval` property to `PT{MinutesToRun}M`
4. Create task scheduler by importing the `CowinScheduler.xml` file in Task scheduler
5. For logging check event viewer with source type CowinLog