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
        [Inject] public UpdateFriendPicSignal updateFriendPicSignal { get; set; }
        [Inject] public AddFriendsSignal addFriendsSignal { get; set; }
        [Inject] public GetSocialPicsSignal getSocialPicsSignal { get; set; }

        // models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }

        // services
        [Inject] public IBackendService backendService { get; set; }

        public override void Execute()
        {
            if (playerModel.busyRefreshingCommunity)
            {
                LogUtil.Log("IM BUSY EXITING", "red");
                return;
            }
            else
            {
                LogUtil.Log("IM GOOD PROCEEDING...", "green");
            }
            playerModel.busyRefreshingCommunity = true;

            Retain();

            backendService.FriendsOpCommunity().Then(OnCommunityRefresh);
            clearCommunitySignal.Dispatch();
        }

        private void OnCommunityRefresh(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                foreach (KeyValuePair<string, Friend> obj in playerModel.community)
                {
                    Friend friend = obj.Value;
                    friend.publicProfile.profilePicture = picsModel.GetPic(friend.playerId);
                }    
            }

            addFriendsSignal.Dispatch(playerModel.community);
            getSocialPicsSignal.Dispatch(playerModel.community);

            playerModel.busyRefreshingCommunity = false;
            Release();
        }
    }
}
