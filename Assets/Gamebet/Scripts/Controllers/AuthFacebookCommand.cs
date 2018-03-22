/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-31 12:30:42 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections;

using UnityEngine;

using strange.extensions.command.impl;

using TurboLabz.Common;

namespace TurboLabz.Gamebet 
{
    public class AuthFacebookCommand : Command
    {
        // Dispatch signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public LoadViewSignal loadViewSignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Models
        [Inject] public IAppEventModel appEventModel { get; set; }

        // Utilities
        [Inject] public IRoutineRunner routineRunner { get; set; }

        public override void Execute()
        {
            Retain();
            appEventModel.reconnectOnPause = false; // Fire a signal here and let the AppEventCommand handle the model state
            loadViewSignal.Dispatch(ViewId.LOADING);
            routineRunner.StartCoroutine(AuthFacebookCR());
        }

        private IEnumerator AuthFacebookCR()
        {
            // Eliminate screen flicker by waiting before sending Facebook auth
            yield return new WaitForSeconds(Settings.AUTH_FACEBOOK_ENTER_DURATION);
            backendService.AuthFacebook().Then(OnAuthFacebook);
        }

        private void OnAuthFacebook(BackendResult result)
        {
            routineRunner.StartCoroutine(OnAuthFacebookCR(result));
        }

        private IEnumerator OnAuthFacebookCR(BackendResult result)
        {
            if (result == BackendResult.AUTH_FACEBOOK_REQUEST_CANCELLED)
            {
                // Eliminate screen flicker by waiting before loading screen
                yield return new WaitForSeconds(Settings.AUTH_FACEBOOK_EXIT_DURATION);
                loadViewSignal.Dispatch(ViewId.AUTHENTICATION);
            }
            else if (result != BackendResult.SUCCESS)
            {
                backendErrorSignal.Dispatch(result);
            }

            appEventModel.reconnectOnPause = true; // Fire a signal here and let the AppEventCommand handle the model state
            Release();
        }
    }
}
