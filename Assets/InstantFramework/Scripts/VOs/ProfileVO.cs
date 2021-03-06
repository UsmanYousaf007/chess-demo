/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using UnityEngine.Android;


namespace TurboLabz.InstantFramework
{
    public class ProfileVO
    {
        public Sprite playerPic;
        public string playerName;
        public bool isFacebookLoggedIn;
        public bool isAppleSignedIn;
        public bool isAppleSignInSupported;
        public int eloScore;
        public string countryId;
        public string playerId;
        public bool isOnline;
        public string avatarId;
        public string avatarColorId;
        public bool isActive;
        public string activity;
        public bool isPremium;
        public Sprite leagueBorder;
        public int trophies2;
    }
}
