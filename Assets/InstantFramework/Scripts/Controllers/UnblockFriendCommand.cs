using strange.extensions.command.impl;

namespace TurboLabz.InstantFramework
{
    public class UnblockFriendCommand : Command
    {
        // Parameter
        [Inject] public string friendId { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public ILocalizationService localizationService { get; set; }

        // Dispatch Signals
        [Inject] public ManageBlockedFriendsSignal manageBlockedFriendsSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateConfirmDlgSignal updateConfirmDlgSignal { get; set; }
        [Inject] public ResetUnblockButtonSignal resetUnblockButtonSignal { get; set; }
        [Inject] public AddFriendsSignal addFriendsSignal { get; set; }
        [Inject] public GetSocialPicsSignal getSocialPicsSignal { get; set; }
        [Inject] public SortFriendsSignal sortFriendsSignal { get; set; }
        [Inject] public UpdateFriendBarSignal updateFriendBarSignal { get; set; }

        //Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }

        public override void Execute()
        {
            Retain();
            backendService.FriendsOpUnblock(friendId).Then(OnFriendUnblocked);
        }

        private void OnFriendUnblocked(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                manageBlockedFriendsSignal.Dispatch(string.Empty, false);
                addFriendsSignal.Dispatch(playerModel.friends, FriendCategory.FRIEND);
                updateFriendBarSignal.Dispatch(playerModel.friends[friendId], friendId);
                getSocialPicsSignal.Dispatch(playerModel.friends);
                sortFriendsSignal.Dispatch();
            }

            Release();
        }
    }
}
