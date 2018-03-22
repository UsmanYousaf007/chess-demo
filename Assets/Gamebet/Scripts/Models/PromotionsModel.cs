/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-04-21 16:17:22 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections.Generic;

namespace TurboLabz.Gamebet
{
    // TODO(mubeeniqbal): Think about adding rewards to all promotions
    // whatsoever. Every promotion might deserve its own reward. The user
    // probably earned it.
    public class PromotionsModel : IPromotionsModel
    {
        public IList<LevelPromotion> levelPromotions { get; set; }
        public LeaguePromotion leaguePromotion { get; set; }
        public TrophyPromotion trophyPromotion { get; set; }
        public RoomTitlePromotion roomTitlePromotion { get; set; }

        public void Reset()
        {
            levelPromotions = null;
            leaguePromotion = null;
            trophyPromotion = null;
            roomTitlePromotion = null;
        }
    }

    // TODO(mubeeniqbal): Standardize objects to be structs and/or classes.
    public struct LevelPromotion
    {
        public int from;
        public int to;
        public LevelPromotionReward reward;
        public NextLeaguePromotion nextLeaguePromotion;
    }

    public struct LevelPromotionReward
    {
        public long currency2;
    }

    public class LeaguePromotion
    {
        public string from;
        public string to;
        public NextLeaguePromotion nextLeaguePromotion;
    }

    public class NextLeaguePromotion
    {
        public string id;
        public int startLevel;
    }

    public class TrophyPromotion
    {
        public int from;
        public int to;
    }

    public class RoomTitlePromotion
    {
        public string from;
        public string to;
        public RoomTitlePromotionReward reward;
    }

    public struct RoomTitlePromotionReward
    {
        public long currency2;
    }
}
