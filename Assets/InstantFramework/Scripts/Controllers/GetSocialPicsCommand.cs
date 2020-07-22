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
        [Inject] public IPicsModel picsModel { get; set; }

        // services
        [Inject] public IFacebookService facebookService { get; set; }
        [Inject] public IProfilePicService profilePicService { get; set; }
        int picRequestCount = 0;
        int picResponseCount = 0;

        public override void Execute()
        {
            Retain();

            foreach (KeyValuePair<string, Friend> obj in friends)
            {
                Friend friend = obj.Value;
                var cachedPic = picsModel.GetPlayerPic(friend.playerId);

                if (cachedPic == null)
                {
                    if (!string.IsNullOrEmpty(friend.uploadedPicId))
                    {
                        picRequestCount++;
                        profilePicService.GetProfilePic(friend.playerId, friend.uploadedPicId).Then(OnGetProfilePic);         
                    }

                    else if (friend.publicProfile.facebookUserId != null)
                    {
                        picRequestCount++;
                        facebookService.GetSocialPic(friend.publicProfile.facebookUserId, friend.playerId).Then(OnGetSocialPic);
                    }
                }
                else
                {
                    updateFriendPicSignal.Dispatch(friend.playerId, cachedPic);
                }
            }
        }

        private void OnGetSocialPic(FacebookResult result, Sprite sprite, string friendId)
        {
            if (result == FacebookResult.SUCCESS)
            {
                SetPics(sprite, friendId);
            }

            picResponseCount++;

            if (picRequestCount == picResponseCount)
            {
                SavePics();
            }
        }

        private void OnGetProfilePic(BackendResult result, Sprite sprite, string friendId)
        {
            if (result == BackendResult.SUCCESS)
            {
                SetPics(sprite, friendId);
            }

            picResponseCount++;

            if (picRequestCount == picResponseCount)
            {
                SavePics();
            }
        }

        private void SetPics(Sprite sprite, string friendId)
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
                    playerModel.friends[friendId].publicProfile.profilePicture = sprite;
                }
                else if (playerModel.blocked.ContainsKey(friendId))
                {
                    playerModel.blocked[friendId].publicProfile.profilePicture = sprite;
                }
            }
        }

        private void SavePics()
        {
            //Only saving frinds pics to disk
            picsModel.SetFriendPics(playerModel.friends, true);
            picsModel.SetFriendPics(playerModel.search, false);
            picsModel.SetFriendPics(playerModel.community, false);
            picsModel.SetFriendPics(playerModel.blocked, false);

            Resources.UnloadUnusedAssets();
            Release();
        }


    }
}
