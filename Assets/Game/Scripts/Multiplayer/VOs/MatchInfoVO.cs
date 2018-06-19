/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-12-16 16:41:09 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;

namespace TurboLabz.Multiplayer
{
    public struct MatchInfoVO
    {
        public string playerName;
        public string playerRoomTitleId;
        public int playerLevel;
        public string playerCountryId;
        public string opponentName;
        public string opponentRoomTitleId;
        public int opponentLevel;
        public string opponentCountryId;
        public string roomId;
        public bool isPlayerTurn;
        public long prize;
        public int durationMinutes;
        public Sprite opponentProfilePictureSprite;
    }
}
