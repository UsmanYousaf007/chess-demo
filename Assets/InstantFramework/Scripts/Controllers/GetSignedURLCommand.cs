/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public class GetSignedURLCommand : Command
    {
        // Params
        [Inject] public string unsignedURL { get; set; }

        // Dispatch Signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public ReceivedSignedURLSignal signedURLReceivedSignal { get; set; }

        // Services
        [Inject] public IAWSService aWSService { get; set; }

        public override void Execute()
        {
            Retain();
            aWSService.GetSignedUrl(unsignedURL).Then(OnURLReceived);
        }

        public void OnURLReceived(BackendResult result, string signedUrl)
        {
            if (result == BackendResult.SUCCESS)
            {
                signedURLReceivedSignal.Dispatch(signedUrl);
            }
            else
            {
                backendErrorSignal.Dispatch(result);
            }
        }
    }
}