/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-04-14 13:07:44 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System;
using System.Collections.Generic;
using TurboLabz.InstantFramework;

namespace TurboLabz.TLUtils
{
    public static class CollectionsUtil
    {
        private static Random random = new Random();
        private static Dictionary<string, string> stateToContextMap;
        private static Dictionary<string, AnalyticsContext> stringToContextMap;

        public static void Shuffle<T>(T[] collection)
        {
            int length = collection.Length;

            // Knuth shuffle algorithm.
            for (int i = 0; i < length; ++i)
            {
                int index = random.Next(i, length);
                T tmp = collection[i];
                collection[i] = collection[index];
                collection[index] = tmp;
            }
        }

        public static string GetContextFromState(string state)
        {
            if (stateToContextMap == null)
            {
                CreateStateToContextMap();
            }

            if (!stateToContextMap.ContainsKey(state))
            {
                return $"unknown_{state}";
            }

            return stateToContextMap[state];
        }

        public static AnalyticsContext GetContextFromString(string context)
        {
            if (stringToContextMap == null)
            {
                CreateStringToContextMap();
            }

            if (!stringToContextMap.ContainsKey(context))
            {
                return AnalyticsContext.unknown;
            }

            return stringToContextMap[context];
        }

        private static void CreateStateToContextMap()
        {
            stateToContextMap = new Dictionary<string, string>();
            stateToContextMap.Add("Lobby", "lobby");
            stateToContextMap.Add("LimitReachedDlg", "games_limit");
            stateToContextMap.Add("ThemeSelectionDlg", "themes");
            stateToContextMap.Add("Settings", "settings");
            stateToContextMap.Add("Shop", "shop");
            stateToContextMap.Add("CPU", "in_game");
            stateToContextMap.Add("Inventory", "inventory");
            stateToContextMap.Add("LessonsView", "lessons");
            stateToContextMap.Add("Multiplayer", "in_game");
            stateToContextMap.Add("MultiplayerResultsDlg", "end_card");
            stateToContextMap.Add("RewardDailySubscription", "daily_subscription_reward");
            stateToContextMap.Add("RewardDailyLeague", "daily_league_reward");
            stateToContextMap.Add("RewardTournamentEnd", "tournament_reward");
            stateToContextMap.Add("TournamentLeaderboard", "tournament_main");
        }

        private static void CreateStringToContextMap()
        {
            stringToContextMap = new Dictionary<string, AnalyticsContext>();
            stringToContextMap.Add("SpecialItemRatingBooster", AnalyticsContext.ratingBooster);
            stringToContextMap.Add("SpecialItemHint", AnalyticsContext.hint);
            stringToContextMap.Add("SpecialItemKey", AnalyticsContext.key);
            stringToContextMap.Add("SpecialItemGemsBooster", AnalyticsContext.lucky_gem_booster);
            stringToContextMap.Add("SpecialItemRatingBoosterPoints", AnalyticsContext.rewarded_rating_booster);
            stringToContextMap.Add("SpecialItemHintPoints", AnalyticsContext.rewarded_hints);
            stringToContextMap.Add("SpecialItemKeyPoints", AnalyticsContext.rewarded_keys);
            stringToContextMap.Add("SpecialItemGemBoosterPoints", AnalyticsContext.rewarded_gem_booster);
            stringToContextMap.Add("gems", AnalyticsContext.gems);
            stringToContextMap.Add("SpecialItemTicket", AnalyticsContext.ticket);
            stringToContextMap.Add("SpecialItemTicketPoints", AnalyticsContext.rewarded_tickets);
            stringToContextMap.Add("SpecialItemRatingBoosterPointsPopup", AnalyticsContext.rewarded_rating_booster_popup);
            stringToContextMap.Add("SpecialItemHintPointsPopup", AnalyticsContext.rewarded_hints_popup);
            stringToContextMap.Add("SpecialItemKeyPointsPopup", AnalyticsContext.rewarded_keys_popup);
            stringToContextMap.Add("SpecialItemTicketPointsPopup", AnalyticsContext.rewarded_tickets_popup);
            stringToContextMap.Add("SpecialItemRatingBoosterPopup", AnalyticsContext.popup_rating_booster);
            stringToContextMap.Add("SpecialItemHintPopup", AnalyticsContext.popup_hint);
            stringToContextMap.Add("SpecialItemKeyPopup", AnalyticsContext.popup_key);
            stringToContextMap.Add("SpecialItemTicketPopup", AnalyticsContext.popup_ticket);
        }
    }
}
