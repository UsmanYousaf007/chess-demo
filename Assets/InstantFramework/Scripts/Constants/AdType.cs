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
}
