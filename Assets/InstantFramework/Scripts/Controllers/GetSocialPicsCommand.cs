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
    public class GetSocialPicsCommand : Command
    {
        // parameters
        [Inject] public Dictionary<string, Friend> friends { get; set; }
     
        // dispatch signals
        [Inject] public UpdateFriendPicSignal updateFriendPicSignal { get; set; }

        // models
        [Inject] public IPlayerModel playerModel { get; set; }

        // services
        [Inject] public IFacebookService facebookService { get; set; }

        int picRequestCount = 0;
        int picResponseCount = 0;

        public override void Execute()
        {
            Retain();
        
            picRequestCount = friends.Count;

            foreach (KeyValuePair<string, Friend> obj in friends)
            {
                Friend friend = obj.Value;
                facebookService.GetSocialPic(friend.publicProfile.facebookUserId, friend.playerId).Then(OnGetSocialPic); 
            }    
        }

        private void OnGetSocialPic(FacebookResult result, Sprite sprite, string friendId)
        {
            if (result == FacebookResult.SUCCESS)
            {
                updateFriendPicSignal.Dispatch(friendId, sprite);

                if (playerModel.community.ContainsKey(friendId))
                {
                    playerModel.community[friendId].publicProfile.profilePicture = sprite;
                }
                else if (playerModel.friends.ContainsKey(friendId))
                {
                    playerModel.friends[friendId].publicProfile.profilePicture = sprite;
                }
            }

            picResponseCount++;

            if (picRequestCount == picResponseCount)
            {
                Release();
            }
        }
    }
}
