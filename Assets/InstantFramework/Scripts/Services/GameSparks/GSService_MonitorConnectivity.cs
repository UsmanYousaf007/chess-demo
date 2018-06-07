/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-22 12:55:25 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections;

using UnityEngine;

using GameSparks.Api.Responses;

using TurboLabz.TLUtils;
using GameSparks.Core;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public void MonitorConnectivity()
        {
            GS.GameSparksAvailable += GameSparksAvailable;
        }

        void GameSparksAvailable(bool isAvailable)
        {
            if (isAvailable)
            {
                receptionSignal.Dispatch();
            }
            else
            {
                // TODO: Show some kind of overlay when GS disconnects?
            }
        }
    }
}
