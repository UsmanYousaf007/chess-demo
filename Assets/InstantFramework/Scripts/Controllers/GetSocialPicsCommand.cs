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
    public class GetSocialPicsCommand : Command
    {
        // parameters
        [Inject] public Dictionary<string, Friend> friends { get; set; }
     
        // dispatch signals
        [Inject] public UpdateFriendPicSignal updateFriendPicSignal { get; set; }

        // models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }

        // services
        [Inject] public IFacebookService facebookService { get; set; }

        int picRequestCount = 0;
        int picResponseCount = 0;
        bool cacheFriendPics = false;

        public override void Execute()
        {
            Retain();

            //picRequestCount = friends.Count;

            foreach (KeyValuePair<string, Friend> obj in friends)
            {
                Friend friend = obj.Value;

                if (friend.publicProfile.facebookUserId != null)
                {
                    picRequestCount++;
                }
            }

            foreach (KeyValuePair<string, Friend> obj in friends)
            {
                Friend friend = obj.Value;

                if(friend.publicProfile.facebookUserId != null && picsModel.GetPlayerPic(friend.playerId) == null)
                {
                    facebookService.GetSocialPic(friend.publicProfile.facebookUserId, friend.playerId).Then(OnGetSocialPic);
                }
                else
                {
                    GetCachedPic(friend.playerId);
                }
            }    
        }

        private void GetCachedPic(string friendId)
        {
            Sprite sprite = picsModel.GetPlayerPic(friendId);
            updateFriendPicSignal.Dispatch(friendId, sprite);
        }

        private void OnGetSocialPic(FacebookResult result, Sprite sprite, string friendId)
        {
            if (result == FacebookResult.SUCCESS)
            {
            	TLUtils.LogUtil.LogNullValidation(friendId, "friendId");
            
                if (friendId != null)
                {
                    updateFriendPicSignal.Dispatch(friendId, sprite);

                    if (playerModel.community.ContainsKey(friendId))
                    {
                        playerModel.community[friendId].publicProfile.profilePicture = sprite;
                    }
                    else if (playerModel.search.ContainsKey(friendId))
                    {
                        playerModel.search[friendId].publicProfile.profilePicture = sprite;
                    }
                    else if (playerModel.friends.ContainsKey(friendId))
                    {
                        cacheFriendPics = true;
                        playerModel.friends[friendId].publicProfile.profilePicture = sprite;
                    }
                }
            }

            picResponseCount++;

            if (picRequestCount == picResponseCount)
            {
                if (cacheFriendPics)
                {
                    picsModel.SetFriendPics(playerModel.friends);
                }

                Resources.UnloadUnusedAssets();
                Release();
            }
        }
    }
}
