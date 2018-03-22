/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-11-23 17:47:12 UTC+05:00
///
/// @description
/// [add_description_here]

using UnityEngine;

using System.Collections.Generic;

namespace TurboLabz.Gamebet
{
    public struct PlayerProfileVO
    {
        public IPlayerModel playerModel;

        public string tag;
        public string name;
        public string countryId;
        public Sprite profilePicture;
        public long currency1;
        public long currency2;
        public long currency1Winnings;
        public bool hasReachedMaxLevel;
        public int level;
        public int levelStartXp;
        public int levelEndXp;
        public int xp;
        public string leagueId;

        // The keys of the dictionary are the IDs of the rooms.
        public IDictionary<string, RoomRecordVO> roomRecords;

        public int totalGamesWon;
        public int totalGamesLost;
        public int totalGamesDrawn;
        public int totalGames;
    }
}
