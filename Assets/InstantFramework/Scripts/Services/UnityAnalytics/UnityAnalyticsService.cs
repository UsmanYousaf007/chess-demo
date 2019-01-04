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
using TurboLabz.TLUtils;
using System.Text;

namespace TurboLabz.InstantFramework
{
    public class UnityAnalyticsService : IAnalyticsService
    {

        [Inject] public IFacebookService facebookService { get; set; }

        // These events are sent once per session
        public void FacebookLoggedIn()
        {
            Analytics.CustomEvent("facebook_loggedin");

            Print("facebook_loggedin");
        }

        private void CountEvent(string name, int count)
        {
            Dictionary<string, object> p = new Dictionary<string, object>
            {
                { "total", count }
            };

            Analytics.CustomEvent(name, p);

            Print(name, p);
        }

        public void TapShare()
        {
            Analytics.CustomEvent("tap_share");

            Print("tap_share");
        }

        public void TapHelp()
        {
            Analytics.CustomEvent("tap_help");

            Print("tap_help");
        }

        public void TapInvite()
        {
            Analytics.CustomEvent("tap_invite");

            Print("tap_invite");
        }

        public void TapShopSkin(string name)
        {
            Dictionary<string, object> p = new Dictionary<string, object>
            {
                { "name", name }
            };

            Analytics.CustomEvent("tap_shop_skin", p);

            Print("tap_shop_skin", p);
        }

        public void TapCommunityRefresh()
        {
            Analytics.CustomEvent("tap_community_refresh");

            Print("tap_community_refresh");
        }

        public void TapComputerUndo()
        {
            Analytics.CustomEvent("tap_computer_undo");

            Print("tap_computer_undo");
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

            Dictionary<string, object> p = new Dictionary<string, object>
            {
                { "bucket", bucketStr }
            };

            Analytics.CustomEvent("player_rating", p);

            Print("player_rating", p);
        }

        public void ActiveLongMatchCount(int count)
        {
            CountEvent("active_long_match_count", count);
        }

        public void AdStart(bool rewarded)
        {
            AnalyticsEvent.AdStart(rewarded);

            Print("AdStart:" + rewarded);
        }

        public void AdComplete(bool rewarded)
        {
            AnalyticsEvent.AdComplete(rewarded);

            Print("AdComplete:" + rewarded);
        }

        public void AdSkip(bool rewarded)
        {
            AnalyticsEvent.AdSkip(rewarded);

            Print("AdSkip:" + rewarded);
        }

        public void VisitProfile()
        {
            AnalyticsEvent.ScreenVisit("profile");

            Print("ScreenVisit:" + "profile");
        }

        public void VisitShop()
        {
            AnalyticsEvent.ScreenVisit("shop");

            Print("ScreenVisit:" + "shop");
        }

        public void VisitFriends()
        {
            AnalyticsEvent.ScreenVisit("friends");

            Print("ScreenVisit:" + "friends");
        }

        public void VisitFriendsProfile()
        {
            AnalyticsEvent.ScreenVisit("friends_profile");

            Print("ScreenVisit:" + "friends_profile");
        }

        public void ComputerMatchStarted(string level)
        {
            Dictionary<string, object> p = new Dictionary<string, object>
            {
                { "level", level },
                { "fb_logged_in", facebookService.isLoggedIn() }
            };

            Analytics.CustomEvent("computer_match_started", p);

            Print("computer_match_started", p);
        }

        public void ComputerMatchContinued(string level)
        {
            Dictionary<string, object> p = new Dictionary<string, object>
            {
                { "level", level },
                { "fb_logged_in", facebookService.isLoggedIn() }
            };

            Analytics.CustomEvent("computer_match_continued", p);

            Print("computer_match_continued", p);
        }

        public void ComputerMatchCompleted(string level, string result)
        {
            Dictionary<string, object> p = new Dictionary<string, object>
            {
                { "level", level },
                { "fb_logged_in", facebookService.isLoggedIn() },
                { "result", result }
            };

            Analytics.CustomEvent("computer_match_completed", p);

            Print("computer_match_completed", p);
        }

        public void QuickBotMatchStarted(float botDifficulty)
        {
            Dictionary<string, object> p = new Dictionary<string, object>
            {
                { "bot_difficulty", botDifficulty },
                { "fb_logged_in", facebookService.isLoggedIn() }
            };

            Analytics.CustomEvent("quick_bot_match_started", p);

            Print("quick_bot_match_started", p);
        }

        public void QuickBotMatchCompleted(float botDifficulty, string result)
        {
            Dictionary<string, object> p = new Dictionary<string, object>
            {
                { "bot_difficulty", botDifficulty },
                { "fb_logged_in", facebookService.isLoggedIn() },
                { "result", result }
            };

            Analytics.CustomEvent("quick_bot_match_completed", p);

            Print("quick_bot_match_completed", p);
        }

        public void QuickMatchStarted()
        {
            Dictionary<string, object> p = new Dictionary<string, object>
            {
                { "fb_logged_in", facebookService.isLoggedIn() }
            };

            Analytics.CustomEvent("quick_match_started", p);

            Print("quick_match_started", p);
        }

        public void QuickMatchCompleted(string result)
        {
            Dictionary<string, object> p = new Dictionary<string, object>
            {
                { "fb_logged_in", facebookService.isLoggedIn() },
                { "result", result }
            };

            Analytics.CustomEvent("quick_match_completed", p);

            Print("quick_match_completed", p);
        }

        public void LongMatchEngaged()
        {
            Analytics.CustomEvent("long_match_engaged");

            Print("long_match_engaged");
        }

        public void LongMatchCanceled()
        {
            Analytics.CustomEvent("long_match_canceled");

            Print("long_match_canceled");
        }

        public void LongMatchCompleted(string result, double duration)
        {
            Dictionary<string, object> p = new Dictionary<string, object>
            {
                { "result", result },
                { "duration", duration }
            };

            Analytics.CustomEvent("long_match_completed", p);

            Print("long_match_completed", p);
        }

        public void ChatEngaged()
        {
            Analytics.CustomEvent("chat_engaged");

            Print("chat_engaged");
        }


        void Print(string name, Dictionary<string, object> parameters = null)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
      
            StringBuilder builder = new StringBuilder();
            builder.Append("[TLANALYTICS] ");
            builder.Append(name);

            if (parameters != null)
            {
                builder.Append(" [PARAMS] ");

                foreach (KeyValuePair<string, object> kvp in parameters)
                {
                    builder.Append(kvp.Key + ":" + kvp.Value);
                    builder.Append(" , ");
                }
            }

            LogUtil.Log(builder, "yellow");
#endif
        }

    }
}