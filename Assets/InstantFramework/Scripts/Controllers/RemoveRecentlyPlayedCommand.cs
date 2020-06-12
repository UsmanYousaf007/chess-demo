using strange.extensions.command.impl;

namespace TurboLabz.InstantFramework
{
    public class RemoveRecentlyPlayedCommand : Command
    {
        // parameter
        [Inject] public string friendId { get; set; }

        // dispatch signals
        [Inject] public ClearFriendSignal clearFriendSignal { get; set; }

        // models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }

        // services
        [Inject] public IBackendService backendService { get; set; }

        public override void Execute()
        {
            picsModel.DeleteFriendPic(friendId);
            backendService.FriendsOpRemove(friendId);
            clearFriendSignal.Dispatch(friendId);
            playerModel.friends.Remove(friendId);
        }
    }
}
