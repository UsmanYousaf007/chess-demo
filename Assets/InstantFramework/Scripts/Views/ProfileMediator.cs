/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-03 16:10:58 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;

using strange.extensions.mediation.impl;

using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using TurboLabz.CPU;
using System;

namespace TurboLabz.InstantGame
{
    [CLSCompliant(false)]
    public class ProfileMediator : Mediator
    {
        // Dispatch signals
        [Inject] public AuthFaceBookSignal authFacebookSignal { get; set; }
        [Inject] public AuthSignInWithAppleSignal authSignInWithAppleSignal { get; set; }
        [Inject] public PlayerProfilePicTappedSignal playerProfilePicTappedSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public LoadArenaSignal loadArenaSignal { get; set; }
        [Inject] public UpdateBottomNavSignal updateBottomNavSignal { get; set; }
        [Inject] public LoadInboxSignal loadInboxSignal { get; set; }
        [Inject] public ShowThemesInventoryTabSignal showThemesInventoryTabSignal { get; set; }
        [Inject] public ShareAppSignal shareAppSignal { get; set; }

        // View injection
        [Inject] public ProfileView view { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ITournamentsModel tournamentsModel { get; set; }

        public override void OnRegister()
        {
            view.Init();

            view.facebookButtonClickedSignal.AddListener(OnFacebookButtonClicked);
            view.profilePicButtonClickedSignal.AddListener(OnProfilePicButtonClicked);
            view.signInWithAppleClicked.AddListener(OnSignInWithAppleButtonClicked);
            view.inboxButtonClickedSignal.AddListener(OnInboxButtonClicked);
            view.changeThemesButtonClickedSignal.AddListener(OnChangeThemesButtonClicked);
            view.socialConnectionButtonClickedSignal.AddListener(OnSocialConnectionButtonClicked);
            view.inviteFriendSignal.AddListener(OnInviteFriend);
        }

        [ListensTo(typeof(UpdateProfileSignal))]
        public void OnUpdateProfile(ProfileVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(UpdateEloScoresSignal))]
        public void OnUpdateEloScoresSignal(EloVO vo)
        {
            view.UpdateEloScores(vo);
        }

        [ListensTo(typeof(AuthFacebookResultSignal))]
        public void OnAuthFacebookResult(AuthFacebookResultVO vo)
        {
            view.FacebookAuthResult(vo);
        }

        [ListensTo(typeof(AuthSignInWithAppleResultSignal))]
        public void OnAuthSignInWithAppleResult(AuthSignInWIthAppleResultVO vo)
        {
            view.SignInWithAppleResult(vo);
        }

        [ListensTo(typeof(SignOutSocialAccountSignal))]
        public void OnSignOutSocialAccount()
        {
            view.SignOutSocialAccount();
        }

        [ListensTo(typeof(ToggleFacebookButton))]
        public void OnToggleFacebookButton(bool toggle)
        {
            view.ToggleFacebookButton(toggle);
        }

        [ListensTo(typeof(PhotoPickerCompleteSignal))]
        public void OnProfilePicUpdate(PhotoVO vo)
        {
            view.UpdateProfilePic(vo.sprite);
        }

        private void OnFacebookButtonClicked()
        {
            authFacebookSignal.Dispatch();
        }

        private void OnSignInWithAppleButtonClicked()
        {
            authSignInWithAppleSignal.Dispatch();
        }

        private void OnProfilePicButtonClicked()
        {
            playerProfilePicTappedSignal.Dispatch();
        }

        //[ListensTo(typeof(PlayerModelUpdatedSignal))]
        public void OnPlayerModelUpdated(IPlayerModel playerModel)
        {
            var leagueAssets = tournamentsModel.GetLeagueSprites(playerModel.league.ToString());
            view.SetLeagueBorder(leagueAssets != null ? leagueAssets.ringSprite : null);
        }

        //[ListensTo(typeof(UpdatePurchasedStoreItemSignal))]
        //public void OnSubscrionPurchased(StoreItem item)
        //{
        //    view.ShowPremiumBorder(playerModel.HasSubscription());
        //}

        private void OnInboxButtonClicked()
        {
            loadInboxSignal.Dispatch();
        }

        [ListensTo(typeof(UpdateInboxMessageCountViewSignal))]
        public void OnMessagesUpdated(long messagesCount)
        {
            view.UpdateMessagesCount(messagesCount);
        }

        void OnChangeThemesButtonClicked()
        {
            view.preferencesModel.inventoryTabVisited = true;
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_INVENTORY);
            updateBottomNavSignal.Dispatch(BottomNavView.ButtonId.Inventory);
            showThemesInventoryTabSignal.Dispatch();
        }

        void OnSocialConnectionButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SOCIAL_CONNECTION_DLG);
        }

        private void OnInviteFriend()
        {
            shareAppSignal.Dispatch();
        }

        [ListensTo(typeof(SkinUpdatedSignal))]
        public void UpdateThemeIcon()
        {
            view.UpdateThemeIcon();
        }
    }
}
