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
    public class RefreshCommunityCommand : Command
    {
        // dispatch signals
        [Inject] public ClearCommunitySignal clearCommunitySignal { get; set; }
        [Inject] public AddFriendsSignal addFriendsSignal { get; set; }
        [Inject] public GetSocialPicsSignal getSocialPicsSignal { get; set; }
        [Inject] public SortCommunitySignal sortCommunitySignal { get; set; }

        // models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }

        // services
        [Inject] public IBackendService backendService { get; set; }

        public override void Execute()
        {
            if (playerModel.busyRefreshingCommunity)
            {
                return;
            }

            playerModel.busyRefreshingCommunity = true;

            Retain();

            backendService.FriendsOpCommunity().Then(OnCommunityRefresh);
        }

        private void OnCommunityRefresh(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                clearCommunitySignal.Dispatch();

                addFriendsSignal.Dispatch(playerModel.community, FriendCategory.COMMUNITY);
                getSocialPicsSignal.Dispatch(playerModel.community);
                sortCommunitySignal.Dispatch();
            }
            else
            {
                clearCommunitySignal.Dispatch();
                sortCommunitySignal.Dispatch();
            }

            playerModel.busyRefreshingCommunity = false;
            Release();
        }
    }
}
