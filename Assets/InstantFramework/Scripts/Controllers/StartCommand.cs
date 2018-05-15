/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class StartCommand : Command
    {
        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public LoadMetaDataSignal loadMetaDataSiganl { get; set; }

        // Services
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IShareService shareService { get; set; }
		[Inject] public IStoreService storeService { get; set; }

        public override void Execute()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Input.multiTouchEnabled = Settings.MULTI_TOUCH_ENABLED;
            Application.targetFrameRate = Settings.TARGET_FRAME_RATE;

            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPLASH);
        
            audioService.Init();
            shareService.Init();

            loadMetaDataSiganl.Dispatch();

        }
    }
}
