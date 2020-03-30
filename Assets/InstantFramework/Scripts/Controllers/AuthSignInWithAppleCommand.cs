using strange.extensions.command.impl;
using TurboLabz.InstantGame;
using UnityEngine.SignInWithApple;

namespace TurboLabz.InstantFramework
{
    public class AuthSignInWithAppleCommand : Command
    {
        //Services
        [Inject] public ISignInWithAppleService signInWithAppleService { get; set; }
        [Inject] public IBackendService backendService { get; set; }

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

        private void OnSignInWithAppleComplete(SignInWithApple.CallbackArgs args)
        {
            if (string.IsNullOrEmpty(args.error))
            {
                backendService.AuthSignInWithApple(args.userInfo.idToken, false).Then(OnAuthSignInWithAppleComplete);
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
                if (string.IsNullOrEmpty(playerModel.editedName))
                {
                    backendService.SetPlayerSocialName(signInWithAppleService.GetDisplayName()).Then(OnAuthConcluded);
                }
                else
                {
                    backendService.SetPlayerSocialName(playerModel.editedName).Then(OnAuthConcluded);
                }
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
                refreshCommunitySignal.Dispatch();

                //in case if fb logged in user has subscription, dispatch this signal in order to unlock all subscription features
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