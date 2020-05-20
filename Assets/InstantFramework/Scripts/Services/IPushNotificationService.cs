/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using strange.extensions.promise.api;

namespace TurboLabz.InstantFramework
{
    public interface IPushNotificationService
    {
        void Init();
        string GetToken();
        bool IsNotificationOpened();
        void ClearNotifications();
    }
}
