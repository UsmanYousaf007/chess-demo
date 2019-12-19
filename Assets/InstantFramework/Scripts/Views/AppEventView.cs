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
using HUF.Analytics.API;

namespace TurboLabz.InstantFramework
{
    public class AppEventView : View
    {
        public Signal appPausedSignal = new Signal();
        public Signal appResumedSignal = new Signal();
        public Signal appQuitSignal = new Signal();
        public Signal appEscapeSignal = new Signal();

        [Inject] public IAdsService adsService { get; set; }

        // TODO: Ads need to be initialized in the start function of our app.
        // However our StartCommand is misleading because it is called through
        // the "Awake" chain of Unity so we fire the init signal here directly.
        // This start up chain needs to be renamed and perhaps we need two commands
        // where the current start command is renamed to awakecommand and then
        // have a startcommand.
        protected override void Start()
        {
            base.Start();
            //adsService.Init();
        }

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

                var analyticsEvent = AnalyticsEvent.Create(AnalyticsEventId.focus_lost.ToString())
                .ST1("focus");
                HAnalytics.LogEvent(analyticsEvent);
            }
            else
            {
                if (SplashLoader.launchCode != 1)
                {
                    SplashLoader.launchCode = 2;
                }

                appResumedSignal.Dispatch();
            }
        }

        void OnApplicationQuit()
        {
            appQuitSignal.Dispatch();   
        }
    }
}
