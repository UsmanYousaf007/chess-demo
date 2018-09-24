/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class ProfileVO
    {
        public Sprite playerPic;
        public string playerName;
        public bool isFacebookLoggedIn;
        public int eloScore;
        public string countryId;
        public string playerId;
        public bool isOnline;

        public ProfileVO()
        {
            
        }
    }
}
