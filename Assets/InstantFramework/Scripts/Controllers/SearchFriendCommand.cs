/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using strange.extensions.command.impl;
using TurboLabz.InstantFramework;
using System.Collections.Generic;
using UnityEngine;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantGame
{
    public class SearchFriendCommand : Command
    {
        // Parameters
        [Inject] public string matchString { get; set; }
        [Inject] public int skip { get; set; }

        // dispatch signals
        [Inject] public AddFriendsSignal addFriendsSignal { get; set; }
        [Inject] public GetSocialPicsSignal getSocialPicsSignal { get; set; }
        [Inject] public SortSearchedSignal sortSearchedSignal { get; set; }


        // models
        [Inject] public IPlayerModel playerModel { get; set; }

        // services
        [Inject] public IBackendService backendService { get; set; }

        public override void Execute()
        {
            // Clear list on special parameters
            if (matchString == "" && skip == -1)
            {
                if (playerModel.search != null)
                {
                    playerModel.search.Clear();
                }
                return;
            }

            Retain();

            backendService.FriendsOpSearch(matchString, skip).Then(OnSearchFriend);
        }

        private void OnSearchFriend(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                addFriendsSignal.Dispatch(playerModel.search, FriendCategory.SEARCHED);
                getSocialPicsSignal.Dispatch(playerModel.search);
                sortSearchedSignal.Dispatch();
            }

            Release();
        }
    }
}
