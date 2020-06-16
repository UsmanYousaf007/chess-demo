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
        [Inject] public RefreshFriendsSignal refreshFriendsSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateConfirmDlgSignal updateConfirmDlgSignal { get; set; }
        [Inject] public ResetUnblockButtonSignal resetUnblockButtonSignal { get; set; }

        //Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }

        public override void Execute()
        {
            //cant unblock if friends limit is reached
            if (playerModel.playerFriendsCount >= settingsModel.maxFriendsCount)
            {
                //show dailogue
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CONFIRM_DLG);

                var vo = new ConfirmDlgVO
                {
                    title = localizationService.Get(LocalizationKey.FRIENDS_UNBLOCK_FAILED_TITLE),
                    desc = localizationService.Get(LocalizationKey.FRIENDS_UNBLOCK_FAILED_DESC),
                    yesButtonText = localizationService.Get(LocalizationKey.LONG_PLAY_OK),
                    onClickYesButton = delegate
                    {
                        navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
                    }
                };

                updateConfirmDlgSignal.Dispatch(vo);
                resetUnblockButtonSignal.Dispatch(friendId);
            }
            else
            {
                Retain();
                backendService.FriendsOpUnblock(friendId).Then(OnFriendUnblocked);
            }
        }

        private void OnFriendUnblocked(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                manageBlockedFriendsSignal.Dispatch(string.Empty, false);
                refreshFriendsSignal.Dispatch();
            }

            Release();
        }
    }
}
