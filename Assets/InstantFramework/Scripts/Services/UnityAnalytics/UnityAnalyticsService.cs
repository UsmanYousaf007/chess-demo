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
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public class UnityAnalyticsService : IAnalyticsService
    {
        private const string GAME_END_REASON = "gameEndReason";
        private enum SocialShareEvent
        {
            TAPPED,
            COMPLETE,
            DISMISS,
            UNKNOWN
        }

        private Dictionary<string, object> gameEndReasonParam = new Dictionary<string, object>
        {
            { GAME_END_REASON, "Unknown" }
        };

        public void ScreenVisit(NavigatorViewId viewId)
        {
            AnalyticsEvent.ScreenVisit(viewId.ToString());
        }

        public void LevelStart(string levelId)
        {
            AnalyticsEvent.LevelStart(levelId);
        }

        public void LevelComplete(string levelId, string gameEndReason)
        {
            gameEndReasonParam[GAME_END_REASON] = gameEndReason;
            AnalyticsEvent.LevelComplete(levelId, gameEndReasonParam);
        }

        public void LevelFail(string levelId, string gameEndReason)
        {
            gameEndReasonParam[GAME_END_REASON] = gameEndReason;
            AnalyticsEvent.LevelFail(levelId, gameEndReasonParam);
        }

        public void LevelQuit(string levelId)
        {
            AnalyticsEvent.LevelQuit(levelId);
        }
            
        public void SocialShareTapped()
        {
            AnalyticsEvent.SocialShare(ShareContext.MAIN_MENU + "_" + SocialShareEvent.TAPPED, SocialNetwork.None);
        }

        public void SocialShareComplete()
        {
            AnalyticsEvent.SocialShare(ShareContext.MAIN_MENU + "_" + SocialShareEvent.COMPLETE, SocialNetwork.None);
        }

        public void SocialShareDismiss()
        {
            AnalyticsEvent.SocialShare(ShareContext.MAIN_MENU + "_" + SocialShareEvent.DISMISS, SocialNetwork.None);
        }

        public void SocialShareUnknown()
        {
            AnalyticsEvent.SocialShare(ShareContext.MAIN_MENU + "_" + SocialShareEvent.UNKNOWN, SocialNetwork.None);
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