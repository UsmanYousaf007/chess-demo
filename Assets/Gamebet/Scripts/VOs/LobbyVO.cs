/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-03 15:58:49 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;

namespace TurboLabz.Gamebet
{
    public struct LobbyVO
    {
        public IPlayerModel playerModel;

        public long currency1;
        public long currency2;
        public bool hasReachedMaxLevel;
        public int level;
        public int levelStartXp;
        public int levelEndXp;
        public int xp;
        public string displayName;
        //public Sprite profilePicture;
        public string leagueId;
    }
}
