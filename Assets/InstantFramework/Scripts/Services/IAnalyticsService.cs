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
using GameAnalyticsSDK;

namespace TurboLabz.InstantFramework
{
    public interface IAnalyticsService
    {
        void ScreenVisit(AnalyticsScreen screen);
        void ScreenVisit(AnalyticsScreen screen, bool fb_logged_in);
        void ScreenVisit(AnalyticsScreen screen, bool fb_logged_in, bool is_bot);
        void ScreenVisit(AnalyticsScreen screen, AnalyticsContext context);

        void Event(AnalyticsEventId evt);
        void Event(AnalyticsEventId evt, AnalyticsContext context);
        void Event(string evt, AnalyticsContext context);
        void Event(AnalyticsEventId evt, AnalyticsParameter param, object val);
        void Event(string evt, AnalyticsParameter param, object val);

        void DesignEvent(AnalyticsEventId evt);
        void DesignEvent(AnalyticsEventId evt, AnalyticsParameter param, object val, AnalyticsEventId subEvt);
        void DesignEvent(AnalyticsEventId evt, params string[] contexts);
        void HEvent(string evt, params string [] param);
        void ResourceEvent(GAResourceFlowType flowType, string currency, int amount, string itemType, string itemId);
        void ValueEvent(AnalyticsEventId evt, string context, float val);

        void LevelComplete(int difficulty);
        void LevelFail(int difficulty);
    }
}
