using strange.extensions.command.impl;

namespace TurboLabz.InstantFramework
{
    public class UnblockFriendCommand : Command
    {
        // Parameter
        [Inject] public string friendId { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Dispatch Signals
        [Inject] public ManageBlockedFriendsSignal manageBlockedFriendsSignal { get; set; }
        [Inject] public RefreshFriendsSignal refreshFriendsSignal { get; set; }

        public override void Execute()
        {
            Retain();
            backendService.FriendsOpUnblock(friendId).Then(OnFriendUnblocked);
        }

        private void OnFriendUnblocked(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                manageBlockedFriendsSignal.Dispatch(string.Empty);
                refreshFriendsSignal.Dispatch();
            }

            Release();
        }
    }
}
