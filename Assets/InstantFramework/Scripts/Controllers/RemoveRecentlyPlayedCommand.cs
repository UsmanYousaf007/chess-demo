using UnityEngine;
using System.Collections.Generic;
using strange.extensions.command.impl;

namespace TurboLabz.InstantFramework
{
    public class RemoveRecentlyPlayedCommand : Command
    {
        // parameter
        [Inject] public string friendId { get; set; }
        [Inject] public FriendsSubOp friendsSubOp { get; set; }

        // dispatch signals
        [Inject] public ClearFriendSignal clearFriendSignal { get; set; }

        // models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }

        // services
        [Inject] public IBackendService backendService { get; set; }

        public override void Execute()
        {
            List<string> friendIds = friendsSubOp.friendIds;
            for (int i = 0; i < friendIds.Count; i++)
            {
                // if friend type is community then we remove from local friends
                if (playerModel.friends[friendIds[i]].friendType == GSBackendKeys.Friend.TYPE_COMMUNITY)
                {
                    picsModel.DeleteFriendPic(friendId);
                    clearFriendSignal.Dispatch(friendId);
                    playerModel.friends.Remove(friendId);
                }
            }

            string opJson = JsonUtility.ToJson(friendsSubOp);
            backendService.FriendsOpRemove(string.IsNullOrEmpty(friendId) ? null : friendId, opJson);
        }
    }
}
