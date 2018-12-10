/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-11-01 11:39:01 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.command.impl;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class BackendErrorCommand : Command
    {
        // Signal parameters
        [Inject] public BackendResult backendResult { get; set; }

        // Dispatch signals
        [Inject] public SetErrorAndHaltSignal setErrorAndHaltSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        // Services
        [Inject] public IAnalyticsService analytics { get; set; }

        public override void Execute()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_HARD_STOP);
            //analytics.BackendError(backendResult.ToString());
            setErrorAndHaltSignal.Dispatch(backendResult);
        }
    }
}
