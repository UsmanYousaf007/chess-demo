/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public interface INotificationsModel
    {
        void RegisterNotification(Notification notification);
        bool IsNotificationRegistered(string sender);
        void UnregisterNotifications(string sender);
        void Init();
        void RegisterDailyRewardNotification();
    }
}
