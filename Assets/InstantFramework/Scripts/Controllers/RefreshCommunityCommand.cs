﻿/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using strange.extensions.command.impl;
using TurboLabz.InstantFramework;
using System.Collections.Generic;
using UnityEngine;

namespace TurboLabz.InstantGame
{
    public class RefreshCommunityCommand : Command
    {
        // dispatch signals
        [Inject] public ClearCommunitySignal clearCommunitySignal { get; set; }
        [Inject] public UpdateFriendPicSignal updateFriendPicSignal { get; set; }
        [Inject] public AddFriendSignal addFriendSignal { get; set; }

        // models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }

        // services
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IFacebookService facebookService { get; set; }

        public override void Execute()
        {
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
                    addFriendSignal.Dispatch(friend);
                    updateFriendPicSignal.Dispatch(friend.playerId, picsModel.GetPic(friend.playerId));
                    facebookService.GetSocialPic(friend.publicProfile.facebookUserId, friend.playerId).Then(OnGetSocialPic);    
                }    
            }
        }

        private void OnGetSocialPic(FacebookResult result, Sprite sprite, string friendId)
        {
            if (result == FacebookResult.SUCCESS)
            {
                updateFriendPicSignal.Dispatch(friendId, sprite);
                playerModel.community[friendId].publicProfile.profilePicture = sprite;
            }
        }
    }
}
