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

        // Listen to signals
        [Inject] public ModelsResetSignal modelsResetSignal { get; set; }
        [Inject] public ModelsLoadFromDiskSignal modelsLoadFromDiskSignal { get; set; }
        [Inject] public ModelsSaveToDiskSignal modelsSaveToDiskSignal { get; set; }

        //Dispatch Signals
        [Inject] public NotificationRecievedSignal notificationRecievedSignal { get; set; }

        private List<Notification> registeredNotifications;

        [PostConstruct]
        public void PostConstruct()
        {
            modelsResetSignal.AddListener(Reset);
            modelsLoadFromDiskSignal.AddListener(LoadFromDisk);
            modelsSaveToDiskSignal.AddListener(SaveToDisk);
        }

        // Constants
        const string NOTIFICATIONS_FILE = "notificationsFile";
        const string NOTITICATIONS_DATA = "notificationsData";

        private void Reset()
        {
            registeredNotifications = new List<Notification>();
        }

        private void LoadFromDisk()
        {
            if (!localDataService.FileExists(NOTIFICATIONS_FILE))
            {
                LogUtil.Log($"NOTIFICATION READING file not found", "red");
                return;
            }

            try
            {
                LogUtil.Log($"NOTIFICATION READING STARTED", "red");

                ILocalDataReader reader = localDataService.OpenReader(NOTIFICATIONS_FILE);

                if (reader.HasKey(NOTITICATIONS_DATA))
                {
                    registeredNotifications = reader.ReadList<Notification>(NOTITICATIONS_DATA);
                }

                reader.Close();
                LogUtil.Log($"NOTIFICATION READING ENDED", "red");

            }
            catch
            {
                localDataService.DeleteFile(NOTIFICATIONS_FILE);
                LogUtil.Log($"NOTIFICATION READING FAILED", "red");

            }

            //Clear all registered notifications
            //Local notifications will be registered when app is going to background
            HNotifications.Local.ClearAllNotifications();

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
        }

        private void SaveToDisk()
        {
            foreach (var notification in registeredNotifications)
            {
                //Register local notification in case notication time has not passed
                if (notification.timestamp > backendService.serverClock.currentTimestamp)
                {
                    var notificationData = new NotificationData();
                    notificationData.title = notification.title;
                    notificationData.text = notification.body;
                    notificationData.delayInSeconds = (int)(notification.timestamp - backendService.serverClock.currentTimestamp)/1000;
                    HNotifications.Local.ScheduleNotification(notificationData);
                }
            }

            try
            {
                LogUtil.Log($"NOTIFICATION WRITING STARTED", "red");
                ILocalDataWriter writer = localDataService.OpenWriter(NOTIFICATIONS_FILE);
                writer.WriteList(NOTITICATIONS_DATA, registeredNotifications);
                writer.Close();
                LogUtil.Log($"NOTIFICATION WRITING ENDED", "red");
            }
            catch(Exception e)
            {
                LogUtil.Log($"NOTIFICATION WRITING FAILED {e.Message}", "red");

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
            var delayInSeconds = (int)(notification.timestamp - backendService.serverClock.currentTimestamp) / 1000;
            routineRunner.StartCoroutine(ScheduleInGameNotification(notification, delayInSeconds));
        }

        private IEnumerator ScheduleInGameNotification(Notification notification, int delayInSeconds)
        {
            LogUtil.Log($"NOTIFICATION title {notification.title} STARTED", "red");
            LogUtil.Log($"NOTIFICATION timestamp {notification.timestamp}", "red");
            yield return new WaitForSeconds(delayInSeconds);
            LogUtil.Log($"NOTIFICATION title {notification.title} ENDED", "red");
            DisplayInGameNotification(notification);
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

        public bool IsNotificationRegistered(string title)
        {
            var notification = (from n in registeredNotifications
                                where n.title.Equals(title)
                                select n).FirstOrDefault();

            return notification != null;
        }
    }

    [Serializable]
    public class Notification
    {
        public string title;
        public string body;
        public long timestamp;
        public string sender;
    }
}
