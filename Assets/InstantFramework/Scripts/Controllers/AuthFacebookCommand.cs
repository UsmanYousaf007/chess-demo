/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;

namespace TurboLabz.InstantFramework 
{
    public class AuthFacebookCommand : Command
    {
        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Dispatch Signals
        [Inject] public AuthFacebookCompleteSignal authFacebookCompleteSignal { get; set; }

        public override void Execute()
        {
            Retain();
            // TODO: Need to switch on a "standard" background view blocker
            backendService.AuthFacebook().Then(OnAuthFacebook);
        }

        private void OnAuthFacebook(BackendResult result)
        {
            // TODO: Need to switch off a "standard" background view blocker
            authFacebookCompleteSignal.Dispatch(result);
            Release();
        }
    }
}
