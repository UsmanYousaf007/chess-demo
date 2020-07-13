using strange.extensions.command.impl;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    public class AuthSignInWithAppleCommand : Command
    {
        //Services
        [Inject] public ISignInWithAppleService signInWithAppleService { get; set; }
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IMetaDataModel metaDataModel { get; set; }

        // Dispatch Signals
        [Inject] public AuthSignInWithAppleResultSignal authSignInWithAppleResultSignal { get; set; }
        [Inject] public RefreshFriendsSignal refreshFriendsSignal { get; set; }
        [Inject] public RefreshCommunitySignal refreshCommunitySignal { get; set; }
        [Inject] public ToggleFacebookButton toggleFacebookButton { get; set; }
        [Inject] public SetSkinSignal setSkinSignal { get; set; }
        [Inject] public ShowProcessingSignal showProcessingSignal { get; set; }
        [Inject] public UpdatePurchasedStoreItemSignal updatePurchasedStoreItemSignal { get; set; }

        public override void Execute()
        {
            CommandBegin();
            signInWithAppleService.Login().Then(OnSignInWithAppleComplete);
        }

        private void OnSignInWithAppleComplete(bool success, string authorizationCode)
        {
            if (success)
            {
                backendService.AuthSignInWithApple(authorizationCode, false).Then(OnAuthSignInWithAppleComplete);
            }
            else
            {
                CommandEnd(false);
            }
        }

        private void OnAuthSignInWithAppleComplete(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                var displayName = signInWithAppleService.GetDisplayName();

                if (!string.IsNullOrEmpty(displayName))
                {
                    if (string.IsNullOrEmpty(playerModel.editedName))
                    {
                        backendService.SetPlayerSocialName(displayName).Then(OnAuthConcluded);
                    }
                    else
                    {
                        backendService.SetPlayerSocialName(playerModel.editedName).Then(OnAuthConcluded);
                    }
                }
                else
                {
                    CommandEnd(true);
                }

                analyticsService.Event(AnalyticsEventId.session_apple_id);
            }
            else
            {
                CommandEnd(false);
            }
        }

        private void OnAuthConcluded(BackendResult result)
        {
            CommandEnd(true);
        }

        private void CommandBegin()
        {
            Retain();
            toggleFacebookButton.Dispatch(false);
            showProcessingSignal.Dispatch(true, false);
        }

        private void CommandEnd(bool isSuccessful)
        {
            if (isSuccessful)
            { 
                setSkinSignal.Dispatch(playerModel.activeSkinId);
                refreshFriendsSignal.Dispatch();
                refreshCommunitySignal.Dispatch(true);

                //in case if siwa user has subscription, dispatch this signal in order to unlock all subscription features
                if (playerModel.HasSubscription())
                {
                    updatePurchasedStoreItemSignal.Dispatch(metaDataModel.store.items[GSBackendKeys.ShopItem.SUBSCRIPTION_SHOP_TAG]);
                }
            }

            var vo = new AuthSignInWIthAppleResultVO();
            vo.isSuccessful = isSuccessful;
            vo.name = playerModel.name;
            vo.playerId = playerModel.id;

            authSignInWithAppleResultSignal.Dispatch(vo);
            toggleFacebookButton.Dispatch(true);
            showProcessingSignal.Dispatch(false, false);

            Release();
        }
    }
}