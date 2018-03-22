/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-04-19 14:43:25 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System;

namespace TurboLabz.Chess
{
    public struct InitTimerVO
    {
        public TimeSpan startingTimer;
        public TimeSpan playerTimer;
        public TimeSpan opponentTimer;
        public bool isPlayerTurn;
    }
}
