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
        Rewarded_rating_booster,
        Rewarded_hints,
        Rewarded_keys,
        Rewarded_tickets,
        Rewarded_rating_booster_popup,
        Rewarded_keys_popup,
        Rewarded_tickets_popup,
        Rewarded_hints_popup,
        Interstitial_pregame,
        Interstitial_endgame,
        interstitial_in_game_cpu,
        interstitial_in_game_30_min,
        Interstitial_tournament_pre,
        Interstitial_tournament_end_co,
        Interstitial_in_game_classic,
        Banner,
        Unknown
    }
}
