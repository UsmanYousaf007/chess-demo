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
        private const string EVT_BACKEND_ERROR = "backendError";

        private const string GAME_END_REASON = "gameEndReason";
        private const string SHOP_SKIN_CONTEXT = "shopSkinContext";
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
            // TODO: This is disabled because it throws a "too many events" error.
            // Please remove this.
            // AnalyticsEvent.ScreenVisit(viewId.ToString());
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

        public void AdOffer(bool rewarded)
        {
            AnalyticsEvent.AdOffer(rewarded);
        }

        public void AdStart(bool rewarded)
        {
            AnalyticsEvent.AdStart(rewarded);
        }

        public void AdComplete(bool rewarded)
        {
            AnalyticsEvent.AdComplete(rewarded);
        }

        public void AdSkip(bool rewarded)
        {
            AnalyticsEvent.AdSkip(rewarded);
        }

        public void PurchaseSkin(string skinId)
        {
            AnalyticsEvent.ItemAcquired(AcquisitionType.Soft, SHOP_SKIN_CONTEXT, 1, skinId);
        }

        public void BackendError(string error)
        {
            Analytics.CustomEvent(EVT_BACKEND_ERROR, new Dictionary<string, object>
                {
                    { "error", error }
                });
        }
    }
}