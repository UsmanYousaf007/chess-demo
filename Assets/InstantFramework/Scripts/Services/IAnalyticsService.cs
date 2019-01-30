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
        void ScreenVisit(AnalyticsScreen screen);
        void ScreenVisit(AnalyticsScreen screen, bool fb_logged_in);
        void ScreenVisit(AnalyticsScreen screen, bool fb_logged_in, bool is_bot);
        void ScreenVisit(AnalyticsScreen screen, AnalyticsContext context);

        void Event(AnalyticsEvent evt);
        void Event(AnalyticsEvent evt, AnalyticsContext context);
        void Event(AnalyticsEvent evt, AnalyticsParameter param, bool val);
        void Event(AnalyticsEvent evt, AnalyticsParameter param, string val);
        void Event(AnalyticsEvent evt, AnalyticsParameter param, int val);
        void Event(AnalyticsEvent evt, AnalyticsParameter param, float val);

        void LevelComplete(int difficulty);
        void LevelFail(int difficulty);















        /*

        // These events are sent once per session after init data
        void FacebookLoggedIn();
        void FacebookFriendCount(int count);
        void CommunityFriendCount(int count);
        void PlayerRating(int rating);
        void ActiveLongMatchCount(int count);

        // These events are sent each time they occur
        void AdStart(bool rewarded);
        void AdComplete(bool rewarded);
        void AdSkip(bool rewarded);

        void VisitProfile();
        void VisitShop();
        void VisitFriends();
        void VisitFriendsProfile();

        void TapShopItem(string name);
        void TapCommunityRefresh();
        void TapShare();
        void TapHelp();
        void TapInvite();
        void TapComputerUndo();

        void ComputerMatchStarted(string level);
        void ComputerMatchContinued(string level);
        void ComputerMatchCompleted(string level, string result);

        void QuickBotMatchStarted(float botDifficulty);
        void QuickBotMatchCompleted(float botDifficulty, string result);
        void QuickMatchStarted();
        void QuickMatchCompleted(string result);

        void LongMatchEngaged();
        void LongMatchCanceled();
        void LongMatchCompleted(string result, double duration);

        void ChatEngaged();

        void VirtualGoodConsumed(string itemKey, int quantity);
        */
    }
}
