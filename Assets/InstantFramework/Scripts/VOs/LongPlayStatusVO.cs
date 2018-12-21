/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using System;

namespace TurboLabz.InstantFramework
{
    public struct LongPlayStatusVO
    {
        public string playerId;
        public LongPlayStatus longPlayStatus;
        public DateTime lastActionTime;
        public TimeSpan playerTimer;
        public TimeSpan opponentTimer;
        public bool isGameCanceled;
    }
}
