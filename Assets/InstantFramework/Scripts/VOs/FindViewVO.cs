/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using UnityEngine.Android;


namespace TurboLabz.InstantFramework
{
    public class FindViewVO
    {
        public ProfileVO player;
        public ProfileVO opponent;

        public int timeoutSeconds;
        public bool isTournamentMatch;
        public bool isTicketSpent;
    }
}
