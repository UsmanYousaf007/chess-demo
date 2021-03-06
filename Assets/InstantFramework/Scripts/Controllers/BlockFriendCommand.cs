/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using strange.extensions.command.impl;
using TurboLabz.InstantFramework;
using System.Collections.Generic;
using UnityEngine;

namespace TurboLabz.InstantGame
{
    public class BlockFriendCommand : Command
    {
        // parameter
        [Inject] public string friendId { get; set; }

        // dispatch signals
        [Inject] public ClearFriendSignal clearFriendSignal { get; set; }
        [Inject] public SortFriendsSignal sortFriendsSignal { get; set; }
        [Inject] public ShowGenericProcessingSignal showProcessingSignal { get; set; }

        // models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }

        // services
        [Inject] public IBackendService backendService { get; set; }

        public override void Execute()
        {
            Retain();

            //-- Show UI blocker here
            showProcessingSignal.Dispatch(true);

            backendService.FriendsOpBlock(friendId).Then(OnFriendBlock);
        }

        private void OnFriendBlock(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                clearFriendSignal.Dispatch(friendId);
                sortFriendsSignal.Dispatch();
            }

            //-- Hide UI blocker here
            showProcessingSignal.Dispatch(false);

            Release();
        }
    }
}
