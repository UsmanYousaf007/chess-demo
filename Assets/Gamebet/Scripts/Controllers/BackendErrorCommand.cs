/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-19 18:43:50 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.command.impl;

using TurboLabz.Common;
using UnityEngine;

namespace TurboLabz.Gamebet
{
    public class BackendErrorCommand : Command
    {
        // Signal parameter
        [Inject] public BackendResult result { get; set; }

        // Dispatch signals
        [Inject] public LoadViewSignal loadViewSignal { get; set; }
        [Inject] public UpdateRetryConnectionMessageSignal updateRetryConnectionMessageSignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }
        
        public override void Execute()
        {
            // 1. EXPIRED_RESPONSE: Expired responses are from an old request
            // session. Sessions are ended when there is a Disconnect() call.
            // 2. CHALLENGE_COMPLETE: GameSparks sends a CHALLENGE_COMLETE
            // response if the challenge is complete and a challenge request is
            // sent for the same challenge that has been completed.
            if ((result == BackendResult.EXPIRED_RESPONSE) ||
                (result == BackendResult.CHALLENGE_COMPLETE))
            {
                LogUtil.Log(this.GetType().Name + ": Handled expected backend error result: " + result, "red");
                return;
            }

            LogUtil.Log(this.GetType().Name + ": Error result: " + result, "red");

            backendService.Disconnect();
            loadViewSignal.Dispatch(ViewId.RETRY_CONNECTION);
            updateRetryConnectionMessageSignal.Dispatch(result);
        }
    }
}
