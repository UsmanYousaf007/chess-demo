using System;
using HUF.Notifications.Runtime.API;
using HUF.Utils.Runtime.Extensions;
using JetBrains.Annotations;

namespace HUF.Notifications.Runtime.Data.Structs
{
    [Serializable]
    public class NotificationData
    {
        /// <summary>
        /// Id given when notification cannot be scheduled.
        /// </summary>
        [PublicAPI] public const string INVALID_NOTIFICATION_ID = "-1";

        /// <summary>
        /// Unique identifier of notification.
        /// Leave it empty if you wish it to be generated automatically.
        /// </summary>
        [PublicAPI] public string identifier = default;
        
        /// <summary>
        /// Title of notification.
        /// </summary>
        [PublicAPI] public string title = default;

        /// <summary>
        /// Text message of notification.
        /// </summary>
        [PublicAPI] public string text = default;

        /// <summary>
        /// The identifier of the app-defined category object.
        /// </summary>
        [PublicAPI] public string categoryIdentifier = "default_category";

        /// <summary>
        /// Small icon is shown on Android status bar. Monochrome. <para/>
        /// More info about icon configuration can be found in specific notification documentation. <para/>
        /// The value is an id given to specific icon in NotificationSettings.
        /// Custom icons are available only on Android platform.
        /// </summary>
        [PublicAPI] public string smallIcon = default;

        /// <summary>
        /// Large icon is shown on notification view.
        /// More info about icon configuration can be found in specific notification documentation. <para/>
        /// The value is an id given to specific icon in NotificationSettings.
        /// Custom icons are available only on Android platform.
        /// </summary>
        [PublicAPI] public string largeIcon = default;

        /// <summary>
        /// Delay after which the notification will be shown to user. Value in seconds.
        /// </summary>
        [PublicAPI] public int delayInSeconds = 0;

        /// <summary>
        /// Intent data in string that will be available through the use of <see cref="HLocalNotifications"/>.
        /// </summary>
        [PublicAPI] public string intentData = default;

        /// <summary>
        /// Time when notification will be fired for the first time.
        /// </summary>
        [PublicAPI]
        public DateTime FireTime => DateTime.Now.AddSeconds(delayInSeconds);

        /// <summary>
        /// Returns if notification data is valid for scheduling. 
        /// </summary>
        /// <returns>True if notification is valid, false otherwise.</returns>
        [PublicAPI]
        public bool IsNotificationValid()
        {
            var textDataValid = !title.IsNullOrEmpty() && !text.IsNullOrEmpty();
            var fireTimeValid = FireTime > DateTime.Now;
            return textDataValid && fireTimeValid;
        }
        
        public override string ToString()
        {
            var s =
                $"Identifier: {identifier}\n" +
                $"Title: {title}\n" +
                $"Text: {text}\n" +
                $"Category identifier: {categoryIdentifier}\n" +
                $"Small icon: {smallIcon}\n" +
                $"Large icon: {largeIcon}\n" +
                $"Delay in seconds: {delayInSeconds}\n" +
                $"Intent data: {intentData}\n" +
                $"Fire time: {FireTime}\n";
            return s;
        }
    }
}