/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-04-21 16:16:23 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public interface IPromotionsModel
    {
        IList<LevelPromotion> levelPromotions { get; set; }
        LeaguePromotion leaguePromotion { get; set; }
        TrophyPromotion trophyPromotion { get; set; }
        RoomTitlePromotion roomTitlePromotion { get; set; }
        bool awardMedal { get; set; }
        int prizeBucks { get; set; }

        void Reset();
    }
}
