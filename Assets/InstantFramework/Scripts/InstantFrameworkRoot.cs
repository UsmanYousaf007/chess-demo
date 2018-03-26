/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-06 23:15:44 UTC+05:00
/// 
/// @description
/// This is the root of the strange app.

using UnityEngine;

using strange.extensions.context.impl;

namespace TurboLabz.InstantFramework
{
    public class InstantFrameworkRoot : ContextView
    {
        void Awake()
        {
            #if UNITY_EDITOR

            // For debugging on the editor we don't want the app to keep going
            // in the background when we switch focus to MonoDevelop, etc.
            //UnityEngine.Application.runInBackground = true;

            #endif

            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Input.multiTouchEnabled = Settings.MULTI_TOUCH_ENABLED;
            Application.targetFrameRate = Settings.TARGET_FRAME_RATE;

            context = new InstantFrameworkContext(this);
        }
    }
}
