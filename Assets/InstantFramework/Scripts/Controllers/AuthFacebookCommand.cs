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
        [Inject] public IFacebookService facebookService { get; set; }

        // Dispatch Signals
        [Inject] public AuthFacebookSuccessSignal authFacebookSuccessSignal { get; set; }

        public override void Execute()
        {
            Retain();

            // TODO: Need to switch on a "standard" background view blocker
            facebookService.Auth().Then(OnAuthFacebook);
        }

        private void OnAuthFacebook(FacebookResult result, string accessToken)
        {
            if (result == FacebookResult.SUCCESS)
            {
                backendService.AuthFacebook(accessToken).Then(OnBackendAuthFacebook);
            }
            else
            {
                Release();
            }
        }

        private void OnBackendAuthFacebook(BackendResult result)
        {
            // TODO: Need to switch off a "standard" background view blocker
            if (result == BackendResult.SUCCESS)
            {
                authFacebookSuccessSignal.Dispatch();
            }

            Release();
        }
    }
}
