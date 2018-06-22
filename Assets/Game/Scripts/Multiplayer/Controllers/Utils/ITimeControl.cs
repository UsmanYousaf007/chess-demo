/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-19 19:27:55 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System;

using strange.extensions.signal.impl;

namespace TurboLabz.Multiplayer
{
    public interface ITimeControl
    {
        Signal playerTickSignal { get; }
        Signal opponentTickSignal { get; }
        Signal playerTimerExpiredSignal { get; }
        Signal opponentTimerExpiredSignal { get; }

        TimeSpan playerRealTimer { get; set; }
        TimeSpan playerDisplayTimer { get; set; }
        TimeSpan opponentRealTimer { get; set; }
        TimeSpan opponentDisplayTimer { get; set; }

        void SetTimers(TimeSpan playerTimer, TimeSpan opponentTimer);
        void Reset();
        void StartTimers(bool isPlayerTurn);
        void StopTimers();
        void SwapTimers();
        void PauseTimers();
        void ResumeTimers();
    }
}
