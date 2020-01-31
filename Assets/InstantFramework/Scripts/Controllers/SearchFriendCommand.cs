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

        //Dispatch Singals
        [Inject] public UpdateSearchResultsSignal updateSearchResultsSignal { get; set; }

        // services
        [Inject] public IBackendService backendService { get; set; }

        public override void Execute()
        {
            Retain();

            backendService.FriendsOpSearch(matchString, skip).Then(OnSearchFriend);
        }

        private void OnSearchFriend(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                updateSearchResultsSignal.Dispatch(true);
            }
            else
            {
                updateSearchResultsSignal.Dispatch(false);
            }

            Release();
        }
    }
}
