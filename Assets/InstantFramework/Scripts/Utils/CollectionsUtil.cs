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
        private static Dictionary<string, AdPlacements> stringToAdPlacementMap;

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

        public static AdPlacements GetAdPlacementsIdFromString(string id)
        {
            if (stringToAdPlacementMap == null)
            {
                CreateStringToAdPlacementMap();
            }

            if (!stringToAdPlacementMap.ContainsKey(id))
            {
                return AdPlacements.Unknown;
            }

            return stringToAdPlacementMap[id];
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
            stringToContextMap.Add("LESSONS_VIEW", AnalyticsContext.lessons);
            stringToContextMap.Add("TOPICS_VIEW", AnalyticsContext.lessons);
            stringToContextMap.Add("FRIENDS", AnalyticsContext.friends);
            stringToContextMap.Add("TOURNAMENT_LEADERBOARD_VIEW", AnalyticsContext.tournament_leaderboard);
            stringToContextMap.Add("ARENA_VIEW", AnalyticsContext.tournament_main);
            stringToContextMap.Add("SHOP", AnalyticsContext.shop);
            stringToContextMap.Add("LOBBY", AnalyticsContext.games);
        }

        private static void CreateStringToAdPlacementMap()
        {
            stringToAdPlacementMap = new Dictionary<string, AdPlacements>();
            stringToAdPlacementMap.Add("Rewarded_rating_booster", AdPlacements.Rewarded_rating_booster);
            stringToAdPlacementMap.Add("Rewarded_hints", AdPlacements.Rewarded_hints);
            stringToAdPlacementMap.Add("Rewarded_keys", AdPlacements.Rewarded_keys);
            stringToAdPlacementMap.Add("Rewarded_tickets", AdPlacements.Rewarded_tickets);
            stringToAdPlacementMap.Add("Rewarded_rating_booster_popup", AdPlacements.Rewarded_rating_booster_popup);
            stringToAdPlacementMap.Add("Rewarded_keys_popup", AdPlacements.Rewarded_keys_popup);
            stringToAdPlacementMap.Add("Rewarded_tickets_popup", AdPlacements.Rewarded_tickets_popup);
            stringToAdPlacementMap.Add("Interstitial_pregame", AdPlacements.Interstitial_pregame);
            stringToAdPlacementMap.Add("Interstitial_endgame", AdPlacements.Interstitial_endgame);
            stringToAdPlacementMap.Add("interstitial_in_game_cpu", AdPlacements.interstitial_in_game_cpu);
            stringToAdPlacementMap.Add("interstitial_in_game_30_min", AdPlacements.interstitial_in_game_30_min);
            stringToAdPlacementMap.Add("Interstitial_tournament_pre", AdPlacements.Interstitial_tournament_pre);
            stringToAdPlacementMap.Add("Interstitial_tournament_end_co", AdPlacements.Interstitial_tournament_end_co);
            stringToAdPlacementMap.Add("Interstitial_in_game_classic", AdPlacements.Interstitial_in_game_classic);
            stringToAdPlacementMap.Add("Banner", AdPlacements.Banner);
            stringToAdPlacementMap.Add("SpecialItemRatingBoosterPoints", AdPlacements.Rewarded_rating_booster);
            stringToAdPlacementMap.Add("SpecialItemHintPoints", AdPlacements.Rewarded_hints);
            stringToAdPlacementMap.Add("SpecialItemKeyPoints", AdPlacements.Rewarded_keys);
            stringToAdPlacementMap.Add("SpecialItemTicketPoints", AdPlacements.Rewarded_tickets);
            stringToAdPlacementMap.Add("SpecialItemRatingBoosterPointsPopup", AdPlacements.Rewarded_rating_booster_popup);
            stringToAdPlacementMap.Add("SpecialItemKeyPointsPopup", AdPlacements.Rewarded_keys_popup);
            stringToAdPlacementMap.Add("SpecialItemTicketPointsPopup", AdPlacements.Rewarded_tickets_popup);

        }
    }
}
