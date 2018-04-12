/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-07 17:57:19 UTC+05:00
/// 
/// @description
/// This is the entry point to the game.

using strange.extensions.command.impl;

namespace TurboLabz.InstantFramework
{
    public class StartCommand : Command
    {
        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        // Services
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IShareService shareService { get; set; }

        public override void Execute()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPLASH);
        
            audioService.Init();
            shareService.Init();
        }
    }
}
