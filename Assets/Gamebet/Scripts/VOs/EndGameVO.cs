/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-05-10 15:52:19 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections.Generic;

using UnityEngine;

namespace TurboLabz.Gamebet
{
    public struct EndGameVO
    {
        public IPlayerModel playerModel;

        public EndGameResult endGameResult;
        public long currency1;
        public long currency2;
        public RoomSetting roomInfo;
        public PublicProfile playerPublicProfile;
        public PublicProfile opponentPublicProfile;
        public Promotions promotions;
    }

    public struct Promotions
    {
        public IList<LevelPromotion> levelPromotions;
        public LeaguePromotion leaguePromotion;
        public TrophyPromotion trophyPromotion;
        public RoomTitlePromotion roomTitlePromotion;
    }
}
