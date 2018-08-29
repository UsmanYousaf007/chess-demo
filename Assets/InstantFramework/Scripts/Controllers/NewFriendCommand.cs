/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using strange.extensions.command.impl;
using TurboLabz.InstantFramework;
using UnityEngine;
using System.Collections.Generic;

namespace TurboLabz.InstantGame
{
    public class NewFriendCommand : Command
    {
        // parameters
        [Inject] public string friendId { get; set; }

        // dispatch signals
        [Inject] public RefreshFriendsSignal refreshFriendsSignal { get; set; }
        [Inject] public RefreshCommunitySignal refreshCommunitySignal { get; set; }

        // services
        [Inject] public IBackendService backendService { get; set; }

        // models
        [Inject] public IPlayerModel playerModel { get; set; }


        public override void Execute()
        {
            Retain();
            backendService.FriendsOpAdd(friendId).Then(OnFriendsOpAdd);
        }

        private void OnFriendsOpAdd(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                refreshFriendsSignal.Dispatch();

                // remove if existed in community
                if (playerModel.community.ContainsKey(friendId))
                {
                   refreshCommunitySignal.Dispatch();
                }
            }

            Release();
        }
    }
}
