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

namespace TurboLabz.InstantGame
{
    public class ProfilePicMediator : Mediator
    {
        // Dispatch signals
        [Inject] public PlayerProfilePicTappedSignal playerProfilePicTappedSignal { get; set; }

        // View injection
        [Inject] public ProfilePicView view { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ITournamentsModel tournamentsModel { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.profilePicButtonClickedSignal.AddListener(OnProfilePicButtonClicked);
        }

        [ListensTo(typeof(UpdateProfileSignal))]
        public void OnUpdateProfile(ProfileVO vo)
        {
            view.UpdateView(vo);
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

        [ListensTo(typeof(PhotoPickerCompleteSignal))]
        public void OnProfilePicUpdate(PhotoVO vo)
        {
            view.UpdateProfilePic(vo.sprite);
        }

        private void OnProfilePicButtonClicked()
        {
            playerProfilePicTappedSignal.Dispatch();
        }

        [ListensTo(typeof(UpdatePurchasedStoreItemSignal))]
        public void OnSubscrionPurchased(StoreItem item)
        {
            view.ShowPremiumBorder(playerModel.HasSubscription());
        }

        [ListensTo(typeof(ProfilePictureLoadedSignal))]
        public void OnProfilePictureLoaded(string playerId, Sprite picture)
        {
            if (playerModel.id == playerId)
            {
                view.UpdateProfilePic(picture);
            }
        }

        [ListensTo(typeof(PlayerModelUpdatedSignal))]
        public void OnPlayerModelUpdated(IPlayerModel playerModel)
        {
            var leagueAssets = tournamentsModel.GetLeagueSprites(playerModel.league.ToString());
            view.SetLeagueBorder(leagueAssets != null ? leagueAssets.ringSprite : null);
        }
    }
}
