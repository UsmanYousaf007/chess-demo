using strange.extensions.command.impl;
using UnityEngine;

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
            string opJson = JsonUtility.ToJson(friendsSubOp);
            backendService.FriendsOpRemove(string.IsNullOrEmpty(friendId) ? null : friendId, opJson);
        }
    }
}
