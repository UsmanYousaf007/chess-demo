/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-14 13:49:04 UTC+05:00
/// 
/// @description
/// This provides the information about Unity game engine.

using UnityEngine;

namespace TurboLabz.Common
{
    public class UnityInfo : IGameEngineInfo
    {
        public bool isInternetReachable
        {
            get
            {
                return Application.internetReachability != NetworkReachability.NotReachable;
            }
        }
    }
}
