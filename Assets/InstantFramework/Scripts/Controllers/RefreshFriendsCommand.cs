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
    public class RefreshFriendsCommand : Command
    {
        // dispatch signals
        [Inject] public ClearFriendsSignal clearFriendsSignal { get; set; }
        [Inject] public UpdateFriendPicSignal updateFriendPicSignal { get; set; }
        [Inject] public AddFriendSignal addFriendSignal { get; set; }

        // models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }

        // services
        [Inject] public IFacebookService facebookService { get; set; }


        public override void Execute()
        {
            clearFriendsSignal.Dispatch();

            foreach (KeyValuePair<string, Friend> obj in playerModel.friends)
            {
                Friend friend = obj.Value;
                addFriendSignal.Dispatch(friend);
                updateFriendPicSignal.Dispatch(friend.playerId, picsModel.GetPic(friend.playerId));
                facebookService.GetSocialPic(friend.publicProfile.facebookUserId, friend.playerId).Then(OnGetSocialPic);    
            }
        }

        private void OnGetSocialPic(FacebookResult result, Sprite sprite, string friendId)
        {
            if (result == FacebookResult.SUCCESS)
            {
                updateFriendPicSignal.Dispatch(friendId, sprite);
                playerModel.friends[friendId].publicProfile.profilePicture = sprite;
            }
        }
    }
}
