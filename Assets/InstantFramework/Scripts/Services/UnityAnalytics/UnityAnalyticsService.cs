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

        [Inject] public IFacebookService facebookService { get; set; }

        // These events are sent once per session
        public void FacebookLoggedIn()
        {
            Analytics.CustomEvent("facebook_loggedin");
        }

        private void CountEvent(string name, int count)
        {
            Analytics.CustomEvent(name, new Dictionary<string, object>
                {
                    { "total", count }
                });
        }

        public void TapShare()
        {
            Analytics.CustomEvent("tap_share");
        }

        public void TapHelp()
        {
            Analytics.CustomEvent("tap_help");
        }

        public void TapInvite()
        {
            Analytics.CustomEvent("tap_invite");
        }

        public void TapShopSkin(string name)
        {
            Analytics.CustomEvent("tap_shop_skin", new Dictionary<string, object>
                {
                    { "name", name }
                });
        }

        public void TapCommunityRefresh()
        {
            Analytics.CustomEvent("tap_community_refresh");
        }

        public void TapComputerUndo()
        {
            Analytics.CustomEvent("tap_computer_undo");
        }

        public void FacebookFriendCount(int count)
        {
            CountEvent("facebook_friend_count", count);
        }

        public void CommunityFriendCount(int count)
        {
            CountEvent("community_friend_count", count);
        }

        public void PlayerRating(int rating)
        {
            int bucket = rating - (rating % 500);
            string bucketStr = "rating_" + bucket;

            Analytics.CustomEvent("player_rating", new Dictionary<string, object>
                {
                    { "bucket", bucketStr }
                });
        }

        public void ActiveLongMatchCount(int count)
        {
            CountEvent("active_long_match_count", count);
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

        public void VisitProfile()
        {
            AnalyticsEvent.ScreenVisit("profile");
        }

        public void VisitShop()
        {
            AnalyticsEvent.ScreenVisit("shop");
        }

        public void VisitFriends()
        {
            AnalyticsEvent.ScreenVisit("friends");
        }

        public void VisitFriendsProfile()
        {
            AnalyticsEvent.ScreenVisit("friends_profile");
        }

        public void ComputerMatchStarted(string level)
        {
            Analytics.CustomEvent("computer_match_started", new Dictionary<string, object>
                {
                    { "level", level },
                { "fb_logged_in", facebookService.isLoggedIn() }
                });
        }

        public void ComputerMatchContinued(string level)
        {
            Analytics.CustomEvent("computer_match_continued", new Dictionary<string, object>
                {
                    { "level", level },
                    { "fb_logged_in", facebookService.isLoggedIn() }
                });
        }

        public void ComputerMatchCompleted(string level, string result)
        {
            Analytics.CustomEvent("computer_match_completed", new Dictionary<string, object>
                {
                    { "level", level },
                    { "fb_logged_in", facebookService.isLoggedIn() },
                    { "result", result }
                });
        }

        public void QuickBotMatchStarted(float botDifficulty)
        {
            Analytics.CustomEvent("quick_bot_match_started", new Dictionary<string, object>
                {
                    { "bot_difficulty", botDifficulty },
                    { "fb_logged_in", facebookService.isLoggedIn() }
                });
        }

        public void QuickBotMatchCompleted(float botDifficulty, string result)
        {
            Analytics.CustomEvent("quick_bot_match_completed", new Dictionary<string, object>
                {
                    { "bot_difficulty", botDifficulty },
                    { "fb_logged_in", facebookService.isLoggedIn() },
                    { "result", result }
                });
        }

        public void QuickMatchStarted()
        {
            Analytics.CustomEvent("quick_match_started", new Dictionary<string, object>
                {
                    { "fb_logged_in", facebookService.isLoggedIn() }
                });
        }

        public void QuickMatchCompleted(string result)
        {
            Analytics.CustomEvent("quick_match_completed", new Dictionary<string, object>
                {
                    { "fb_logged_in", facebookService.isLoggedIn() },
                    { "result", result }
                });
        }

        public void LongMatchEngaged()
        {
            Analytics.CustomEvent("long_match_engaged");
        }

        public void LongMatchCompleted(string result, double duration)
        {
            Analytics.CustomEvent("long_match_completed", new Dictionary<string, object>
                {
                    { "result", result },
                    { "duration", duration }
                });
        }

        public void ChatEngaged()
        {
            Analytics.CustomEvent("chat_engaged");
        }
    }
}