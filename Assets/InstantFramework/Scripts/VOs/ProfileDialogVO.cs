/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class ProfileDialogVO
    {
        public string playerId;

        public Sprite playerProfilePic;
        public string playerProfileName;
        public int playerElo;
        public string playerCountryCode;

        public Sprite oppProfilePic;
        public string oppProfileName;
        public int oppElo;
        public string oppCountryCode;
        public string oppPlayingSinceDate;

        public int playerWinsCount;
        public int playerDrawsCount;
        public int opponentWinsCount;
        public int opponentDrawsCount;
        public int totalGamesCount;

        public bool isCommunity;
    }
}
