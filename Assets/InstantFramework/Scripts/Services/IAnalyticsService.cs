/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-01-10 10:47:41 UTC+05:00
/// 
/// @description
/// [add_description_here]
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public interface IAnalyticsService
    {
        void ScreenVisit(NavigatorViewId viewId);
        void LevelStart(int index);
        void LevelComplete(int index, string gameEndReason);
        void LevelFail(int index, string gameEndReason);
        void LevelQuit(int index);
        void SocialShare(ShareContext context);
        void AdOffer(bool rewarded, string placementId);
        void AdStart(bool rewarded, string placementId);
        void AdComplete(bool rewarded, string placementId);
        void AdSkip(bool rewarded, string placementId);
    }
}
