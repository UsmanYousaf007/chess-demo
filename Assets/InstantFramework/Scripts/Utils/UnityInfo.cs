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

using System;
using UnityEngine;
using System.Globalization;

namespace TurboLabz.TLUtils
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

        public static bool Is64Bit()
        {
            bool retVal = false;

            // Find the architecture of the running process.
            // We can use the Environment property Is64BitProcess along with SystemInfo.processorType to figure it out.
            // Do a case insensitive string check.
            if (CultureInfo.InvariantCulture.CompareInfo.IndexOf(SystemInfo.processorType, "ARM", CompareOptions.IgnoreCase) >= 0)
            {
                if (Environment.Is64BitProcess)
                    retVal = true;
            }

            return retVal;
        }
    }
}
