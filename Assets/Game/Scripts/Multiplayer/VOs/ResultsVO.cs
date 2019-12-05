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
using TurboLabz.Chess;

namespace TurboLabz.Multiplayer
{
    public struct ResultsVO
    {
        public GameEndReason reason;
        public bool playerWins;
        public int currentEloScore;
        public int eloScoreDelta;
        public bool isRanked;
        public int powerupUsedCount;
        public bool removeAds;
        public string playerName;
        public string opponentName;
        public bool isChatEnabled;
        public string challengeId;
    }
}
