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
        private static Dictionary<AdPlacements, string> adPlacementToRewardTypeMap;
        private static Dictionary<AdPlacements, AnalyticsContext> adPlacementToAdContextMap;
        public static Dictionary<string, int> fakeEloScores = new Dictionary<string, int>();


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

        public static string GetRewardTypeFromAdPlacement(AdPlacements id)
        {
            if (adPlacementToRewardTypeMap == null)
            {
                CreateAdPlacementToRewardTypeMap();
            }

            if (!adPlacementToRewardTypeMap.ContainsKey(id))
            {
                return string.Empty;
            }

            return adPlacementToRewardTypeMap[id];
        }

        public static AnalyticsContext GetAdContextFromAdPlacement(AdPlacements id)
        {
            if (adPlacementToAdContextMap == null)
            {
                CreateAdPlacementToAdContextMap();
            }

            if (!adPlacementToAdContextMap.ContainsKey(id))
            {
                return AnalyticsContext.unknown;
            }

            return adPlacementToAdContextMap[id];
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
            stateToContextMap.Add("RewardTournamentEnd", "championship_reward");
            stateToContextMap.Add("TournamentLeaderboard", "tournament_main");
            stateToContextMap.Add("RewardLeaguePromotion", "league_progression_rewards");
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
            stringToContextMap.Add("coins", AnalyticsContext.coins);
            stringToContextMap.Add(GSBackendKeys.ShopItem.SPECIAL_BUNDLE_ELITE, AnalyticsContext.elite_bundle);
            stringToContextMap.Add(GSBackendKeys.ShopItem.SPECIAL_BUNDLE_GOLDEN, AnalyticsContext.golden_bundle);
            stringToContextMap.Add(GSBackendKeys.ShopItem.SPECIAL_BUNDLE_EMERALD, AnalyticsContext.emerald_bundle);
            stringToContextMap.Add(GSBackendKeys.ShopItem.SPECIAL_BUNDLE_RUBY, AnalyticsContext.ruby_bundle);
            stringToContextMap.Add(GSBackendKeys.ShopItem.SPECIAL_BUNDLE_DIAMOND, AnalyticsContext.diamond_bundle);
            stringToContextMap.Add(GSBackendKeys.ShopItem.SPECIAL_BUNDLE_GRAND_MASTER, AnalyticsContext.grand_master_bundle);

        }

        private static void CreateStringToAdPlacementMap()
        {
            stringToAdPlacementMap = new Dictionary<string, AdPlacements>();
            stringToAdPlacementMap.Add("Rewarded_daily_reward", AdPlacements.Rewarded_daily_reward);
            stringToAdPlacementMap.Add("Rewarded_lobby_chest", AdPlacements.Rewarded_lobby_chest);
            stringToAdPlacementMap.Add("Rewarded_coins_purchase", AdPlacements.Rewarded_coins_purchase);
            stringToAdPlacementMap.Add("Rewarded_powerplay", AdPlacements.Rewarded_powerplay);
            stringToAdPlacementMap.Add("Rewarded_coins_banner", AdPlacements.Rewarded_coins_banner);
            stringToAdPlacementMap.Add("Rewarded_coins_popup", AdPlacements.Rewarded_coins_popup);
            stringToAdPlacementMap.Add("Rewarded_analysis", AdPlacements.Rewarded_analysis);
            stringToAdPlacementMap.Add("RV_rating_booster", AdPlacements.RV_rating_booster);
            stringToAdPlacementMap.Add("Interstitial_pregame", AdPlacements.Interstitial_pregame);
            stringToAdPlacementMap.Add("Interstitial_endgame", AdPlacements.Interstitial_endgame);
            stringToAdPlacementMap.Add("interstitial_in_game_cpu", AdPlacements.interstitial_in_game_cpu);
            stringToAdPlacementMap.Add("interstitial_in_game_30_min", AdPlacements.interstitial_in_game_30_min);
            stringToAdPlacementMap.Add("Banner", AdPlacements.Banner);
        }

        private static void CreateAdPlacementToRewardTypeMap()
        {
            adPlacementToRewardTypeMap = new Dictionary<AdPlacements, string>();
            adPlacementToRewardTypeMap.Add(AdPlacements.Rewarded_coins_purchase, GSBackendKeys.ClaimReward.TYPE_COINS_PURCHASE);
            adPlacementToRewardTypeMap.Add(AdPlacements.Rewarded_daily_reward, GSBackendKeys.ClaimReward.TYPE_DAILY);
            adPlacementToRewardTypeMap.Add(AdPlacements.Rewarded_lobby_chest, GSBackendKeys.ClaimReward.TYPE_LOBBY_CHEST_V2);
            adPlacementToRewardTypeMap.Add(AdPlacements.Rewarded_coins_banner, GSBackendKeys.ClaimReward.TYPE_COINS_PURCHASE);
            adPlacementToRewardTypeMap.Add(AdPlacements.Rewarded_coins_popup, GSBackendKeys.ClaimReward.TYPE_COINS_PURCHASE);
            adPlacementToRewardTypeMap.Add(AdPlacements.Rewarded_powerplay, GSBackendKeys.ClaimReward.TYPE_POWERPLAY);
            adPlacementToRewardTypeMap.Add(AdPlacements.RV_rating_booster, GSBackendKeys.ClaimReward.TYPE_RV_RATING_BOOSTER);
            adPlacementToRewardTypeMap.Add(AdPlacements.Rewarded_analysis, GSBackendKeys.ClaimReward.TYPE_RV_ANALYSIS);
        }

        private static void CreateAdPlacementToAdContextMap()
        {
            adPlacementToAdContextMap = new Dictionary<AdPlacements, AnalyticsContext>();
            adPlacementToAdContextMap.Add(AdPlacements.Rewarded_coins_purchase, AnalyticsContext.rewarded_coins_spot_state_1);
            adPlacementToAdContextMap.Add(AdPlacements.Rewarded_daily_reward, AnalyticsContext.rewarded_daily_doubler);
            adPlacementToAdContextMap.Add(AdPlacements.Rewarded_lobby_chest, AnalyticsContext.rewarded_lobby_chest);
            adPlacementToAdContextMap.Add(AdPlacements.Rewarded_coins_banner, AnalyticsContext.rewarded_out_of_coins_lobby_banner_popup);
            adPlacementToAdContextMap.Add(AdPlacements.Rewarded_coins_popup, AnalyticsContext.rewarded_out_of_coins_lobby_popup);
            adPlacementToAdContextMap.Add(AdPlacements.Rewarded_powerplay, AnalyticsContext.rewarded_power_mode);
            adPlacementToAdContextMap.Add(AdPlacements.RV_rating_booster, AnalyticsContext.rewarded_rating_booster);
            adPlacementToAdContextMap.Add(AdPlacements.Rewarded_cpu_in_game_power_mode, AnalyticsContext.rewarded_cpu_in_game_power_mode);
            adPlacementToAdContextMap.Add(AdPlacements.Rewarded_cpu_pregame_power_mode, AnalyticsContext.rewarded_cpu_pregame_power_mode);
            adPlacementToAdContextMap.Add(AdPlacements.Rewarded_cpu_resume_power_mode, AnalyticsContext.rewarded_cpu_resume_power_mode);
            adPlacementToAdContextMap.Add(AdPlacements.Rewarded_analysis, AnalyticsContext.rewarded_analysis);
        }
    }
}
