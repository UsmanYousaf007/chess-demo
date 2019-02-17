/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-21 13:53:20 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections;

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine;
using TurboLabz.TLUtils;
using AppodealAds.Unity.Api;

namespace TurboLabz.InstantFramework
{
    public class AppEventView : View
    {
        public Signal appPausedSignal = new Signal();
        public Signal appResumedSignal = new Signal();
        public Signal appQuitSignal = new Signal();
        public Signal appEscapeSignal = new Signal();

        // TODO: Verify that this class behaves correctly for EACH platform
        // Windows Phone might require OnApplicationFocus - this this out
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                appEscapeSignal.Dispatch();
            }
        }
            
        void OnApplicationPause(bool isPaused)
        {
            if (isPaused)
            {
                appPausedSignal.Dispatch();
            }
            else
            {
                appResumedSignal.Dispatch();
            }
        }

        void OnApplicationQuit()
        {
            appQuitSignal.Dispatch();   
        }

        void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                Appodeal.onResume();
            }
        }
    }
}
