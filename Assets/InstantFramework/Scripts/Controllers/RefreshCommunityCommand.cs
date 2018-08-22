/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using strange.extensions.command.impl;
using TurboLabz.InstantFramework;

namespace TurboLabz.InstantGame
{
    public class RefreshCommunityCommand : Command
    {
        // services
        [Inject] public IBackendService backendService { get; set; }

        // dispatch signals
        [Inject] public ClearCommunitySignal clearCommunitySignal { get; set; }

        public override void Execute()
        {
            clearCommunitySignal.Dispatch();
            backendService.FriendsOpCommunity();
        }
    }
}
