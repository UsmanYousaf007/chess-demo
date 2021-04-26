using System;
using System.Collections;
using System.Collections.Generic;
using HUF.Notifications.Runtime.API;
using HUF.Notifications.Runtime.Data.Structs;
using TurboLabz.TLUtils;
using UnityEngine;
using System.Linq;

namespace TurboLabz.InstantFramework
{
    public class NotificationsModel : INotificationsModel
    {
        // Services
        [Inject] public ILocalDataService localDataService { get; set; }
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IRoutineRunner routineRunner { get; set; }
        [Inject] public ILocalizationService localizationService { get; set; }

        // Listen to signals
        [Inject] public ModelsResetSignal modelsResetSignal { get; set; }
        [Inject] public GameAppEventSignal gameAppEventSignal { get; set; }

        //Dispatch Signals
        [Inject] public NotificationRecievedSignal notificationRecievedSignal { get; set; }

        //Models
        [Inject] public IInboxModel inboxModel { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }

        private List<Notification> registeredNotifications;
        private List<Coroutine> inGameScheduledNotifications;

        [PostConstruct]
        public void PostConstruct()
        {
            modelsResetSignal.AddListener(Reset);
            gameAppEventSignal.AddListener(OnAppEventSignal);
        }

        // Constants
        const string NOTIFICATIONS_FILE = "notificationsFile";
        const string NOTITICATIONS_DATA = "notificationsData";

        private void Reset()
        {
            registeredNotifications = new List<Notification>();
            inGameScheduledNotifications = new List<Coroutine>();
        }

        private void OnAppEventSignal(AppEvent appEvent)
        {
            switch (appEvent)
            {
                case AppEvent.RESUMED:
                    Init();
                    break;

                case AppEvent.PAUSED:
#if UNITY_EDITOR
                case AppEvent.QUIT:
#endif
                    StopInGameScheduledNotifications();
                    SaveToDisk();
                    break;

            }
        }

        private void LoadFromDisk()
        {
            if (!localDataService.FileExists(NOTIFICATIONS_FILE))
            {
                return;
            }

            try
            {
                ILocalDataReader reader = localDataService.OpenReader(NOTIFICATIONS_FILE);

                if (reader.HasKey(NOTITICATIONS_DATA))
                {
                    registeredNotifications = reader.ReadList<Notification>(NOTITICATIONS_DATA);
                }

                reader.Close();
            }
            catch
            {
                localDataService.DeleteFile(NOTIFICATIONS_FILE);
            }
        }

        private void SaveToDisk()
        {
            RescheduleDailyRewardNotification();
            ScheduleLocalDeviceNotifications();

            try
            {
                ILocalDataWriter writer = localDataService.OpenWriter(NOTIFICATIONS_FILE);
                writer.WriteList(NOTITICATIONS_DATA, registeredNotifications);
                writer.Close();
            }
            catch(Exception e)
            {
                if (localDataService.FileExists(NOTIFICATIONS_FILE))
                {
                    localDataService.DeleteFile(NOTIFICATIONS_FILE);
                }
            }
        }

        public void RegisterNotification(Notification notification)
        {
            registeredNotifications.Add(notification);
            ScheduleInGameNotification(notification);
        }

        private void ScheduleInGameNotification(Notification notification)
        {
            if (!notification.showInGame)
            {
                return;
            }

            var delayInSeconds = (int)(notification.timestamp - backendService.serverClock.currentTimestamp) / 1000;
            var scheduledNotification = routineRunner.StartCoroutine(ScheduleInGameNotification(notification, delayInSeconds));
            inGameScheduledNotifications.Add(scheduledNotification);
        }

        private IEnumerator ScheduleInGameNotification(Notification notification, int delayInSeconds)
        {
            yield return new WaitForSeconds(delayInSeconds);
            DisplayInGameNotification(notification);
        }

        private void StopInGameScheduledNotifications()
        {
            if (inGameScheduledNotifications == null)
            {
                return;
            }

            foreach (var notification in inGameScheduledNotifications)
            {
                routineRunner.StopCoroutine(notification);
            }
        }

        private void DisplayInGameNotification(Notification notification)
        {
            NotificationVO notificationVO;

            notificationVO.isOpened = false;
            notificationVO.title = notification.title;
            notificationVO.body = notification.body;
            notificationVO.senderPlayerId = notification.sender;
            notificationVO.challengeId = "undefined";
            notificationVO.matchGroup = "undefined";
            notificationVO.avatarId = "undefined";
            notificationVO.avaterBgColorId = "undefined";
            notificationVO.profilePicURL = "undefined";
            notificationVO.isPremium = false;
            notificationVO.timeSent = 0;
            notificationVO.actionCode = "undefined";
            notificationVO.league = -1;

            notificationRecievedSignal.Dispatch(notificationVO);
            registeredNotifications.Remove(notification);
        }

        public bool IsNotificationRegistered(string sender)
        {
            var notification = (from n in registeredNotifications
                                where n.sender.Equals(sender)
                                select n).FirstOrDefault();

            return notification != null;
        }

        public void UnregisterNotifications(string sender)
        {
            var notifications = (from n in registeredNotifications
                                where n.sender.Equals(sender)
                                select n).ToList();

            foreach (var notification in notifications)
            {
                registeredNotifications.Remove(notification);
            }
        }

        public void Init()
        {
            //Clear all registered notifications
            //Local notifications will be registered when app is going to background
            HNotifications.Local.ClearAllNotifications();

            //Load Notifications Data from disk
            LoadFromDisk();

            //Remove notifications that has been sent locally on the device
            var notificationsToRemove = (from notificaiton in registeredNotifications
                                         where notificaiton.timestamp <= backendService.serverClock.currentTimestamp
                                         select notificaiton).ToList();

            foreach (var notification in notificationsToRemove)
            {
                registeredNotifications.Remove(notification);
            }

            //Register in-game notifications
            foreach (var notification in registeredNotifications)
            {
                if (notification.timestamp > backendService.serverClock.currentTimestamp)
                {
                    ScheduleInGameNotification(notification);
                }
            }

            //Process Opened Notification
            var intentData = HNotifications.Local.GetLastIntentData();

            if (!string.IsNullOrEmpty(intentData))
            {
                NotificationVO notificationVO;

                notificationVO.isOpened = true;
                notificationVO.title = "undefined";
                notificationVO.body = "undefined";
                notificationVO.senderPlayerId = intentData;
                notificationVO.challengeId = "undefined";
                notificationVO.matchGroup = "undefined";
                notificationVO.avatarId = "undefined";
                notificationVO.avaterBgColorId = "undefined";
                notificationVO.profilePicURL = "undefined";
                notificationVO.isPremium = false;
                notificationVO.timeSent = 0;
                notificationVO.actionCode = "undefined";
                notificationVO.league = -1;

                notificationRecievedSignal.Dispatch(notificationVO);
            }
        }

        //when app is sent to bg it will register the daily reward notification if its not collected
        private void RescheduleDailyRewardNotification()
        {
            //search for daily league reward inbox item
            var dailyNotificationItem =
               inboxModel.items.Where(item => item.Value.type.Equals("RewardDailyLeague"))
                               .Select(item => (KeyValuePair<string, InboxMessage>?)item)
                               .FirstOrDefault();

            if (dailyNotificationItem != null && !IsNotificationRegistered("league"))
            {
                //daily league reward inbox item found
                //registering 8pm remineder notification in case its not already registered

                var reminder = new Notification();
                reminder.title = localizationService.Get(LocalizationKey.NOTIFICATION_DAILY_REWARD_TITLE);
                reminder.body = localizationService.Get(LocalizationKey.NOTIFICATION_DAILY_REWARD_BODY);
                reminder.timestamp = TimeUtil.ToUnixTimestamp(DateTime.Today.AddDays(1).AddHours(settingsModel.dailyNotificationDeadlineHour).ToUniversalTime());
                reminder.sender = "league";
                RegisterNotification(reminder);
            }
        }

        private void ScheduleLocalDeviceNotifications()
        {
            foreach (var notification in registeredNotifications)
            {
                //Register local notification in case notication time has not passed
                if (notification.timestamp > backendService.serverClock.currentTimestamp)
                {
                    var notificationData = new NotificationData();
                    notificationData.title = notification.title;
                    notificationData.text = notification.body;
                    notificationData.delayInSeconds = (int)(notification.timestamp - backendService.serverClock.currentTimestamp) / 1000;
                    notificationData.intentData = notification.sender;
                    HNotifications.Local.ScheduleNotification(notificationData);
                }
            }
        }
    }

    [Serializable]
    public class Notification
    {
        public string title;
        public string body;
        public long timestamp;
        public string sender;
        public bool showInGame = true;
    }
}
