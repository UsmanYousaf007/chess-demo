/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;

namespace TurboLabz.InstantFramework
{
    public class RegisterChallengeCommand : Command
    {
        // Parameters
        [Inject] public string challengeId { get; set; }

        // Dispatch signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        public override void Execute()
        {
            Retain();
            backendService.RegisterChallenge(challengeId).Then(OnRegisterChallenge);
        }

        private void OnRegisterChallenge(BackendResult result)
        {
            if (result != BackendResult.SUCCESS && result != BackendResult.CANCELED)
            {
                backendErrorSignal.Dispatch(result);
            }

            Release();
        }
    }
}
