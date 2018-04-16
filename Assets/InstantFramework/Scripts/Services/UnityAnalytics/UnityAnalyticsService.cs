/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-01-10 10:52:49 UTC+05:00

using UnityEngine;
using UnityEngine.Analytics;

namespace TurboLabz.InstantFramework
{
    public class UnityAnalyticsService : IAnalyticsService
    {
        public void ScreenVisit(NavigatorViewId viewId)
        {
            AnalyticsEvent.ScreenVisit(viewId.ToString());
        }

        public void LevelStart(int index)
        {
            AnalyticsEvent.LevelStart(index);
        }

        public void LevelComplete(int index)
        {
            AnalyticsEvent.LevelStart(index);
        }

        public void LevelFail(int index)
        {
            AnalyticsEvent.LevelFail(index);
        }

        public void LevelQuit(int index)
        {
            AnalyticsEvent.LevelQuit(index);
        }

        public void SocialShare(ShareContext context)
        {
            AnalyticsEvent.SocialShare(context.ToString(), SocialNetwork.None);
        }

        public void AdOffer(bool rewarded, string placementId)
        {
            AnalyticsEvent.AdOffer(rewarded, null, placementId);
        }

        public void AdStart(bool rewarded, string placementId)
        {
            AnalyticsEvent.AdStart(rewarded, null, placementId);
        }

        public void AdComplete(bool rewarded, string placementId)
        {
            AnalyticsEvent.AdComplete(rewarded, null, placementId);
        }

        public void AdSkip(bool rewarded, string placementId)
        {
            AnalyticsEvent.AdSkip(rewarded, null, placementId);
        }
    }
}