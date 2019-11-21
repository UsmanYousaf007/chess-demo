﻿/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

namespace TurboLabz.InstantFramework
{
    public interface ISettingsModel
    {
        int maxLongMatchCount { get; set; }
        int maxFriendsCount { get; set; }
        int maxCommunityMatches { get; set; }
        bool maintenanceFlag { get; set; }
        string updateMessage { get; set; }
        string maintenanceMessage { get; set; }
        string minimumClientVersion { get; set; }

    }
}
