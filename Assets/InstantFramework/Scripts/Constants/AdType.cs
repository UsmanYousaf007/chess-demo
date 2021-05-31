/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.Chess;

namespace TurboLabz.InstantFramework
{
    public enum AdType
    {
        RewardedVideo,
        Interstitial,
        Promotion
    }

    public enum AdContext
    {
        interstitial_pregame,
        interstitial_endgame,
        interstitial_replacement,
        interstitial_capped
    }

    public enum AdPlacements
    {
        Rewarded_daily_reward,
        Rewarded_lobby_chest,
        Rewarded_coins_purchase,
        Rewarded_coins_popup,
        Rewarded_coins_banner,
        Rewarded_powerplay,
        Rewarded_analysis,
        RV_rating_booster,
        Interstitial_pregame,
        Interstitial_endgame,
        interstitial_in_game_cpu,
        interstitial_in_game_30_min,
        Banner,
        Unknown
    }
}
