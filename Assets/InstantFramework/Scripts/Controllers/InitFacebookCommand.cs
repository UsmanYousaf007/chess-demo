/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using TurboLabz.TLUtils;
using TurboLabz.Multiplayer;

namespace TurboLabz.InstantFramework
{
    public class InitFacebookCommand : Command
    {
        // Services
        [Inject] public IFacebookService facebookService { get; set; }
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public ISignInWithAppleService signInWithAppleService { get; set; }
        // Signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public AuthFacebookResultSignal authFacebookResultSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }

        public override void Execute()
        {
            Retain();

            facebookService.Init().Then(OnFacebookInit);

        }

        void OnFacebookInit(FacebookResult result)
        {
            // Existing facebook account
            if ((result == FacebookResult.SUCCESS) && facebookService.isLoggedIn())
            {
                backendService.AuthFacebook(facebookService.GetAccessToken(), true).Then(OnFacebookAuthComplete);
            }
            else
            {
                LogSessionAnalytics();
                Release();
            }
        }

        private void OnFacebookAuthComplete(BackendResult result)
        {
            analyticsService.Event(AnalyticsEventId.session_facebook, AnalyticsParameter.num_facebook_friends, playerModel.GetSocialFriendsCount());

            if (result == BackendResult.SUCCESS)
            {
                CommandEnd(FacebookResult.SUCCESS, null, null);
            }
            else if (result != BackendResult.CANCELED)
            {
                backendErrorSignal.Dispatch(result);
                Release();
            }
            else
            {
                Release();
            }
        }

        void CommandEnd(FacebookResult result, Sprite sprite, string facebookUserId)
        {
            //picsModel.SetPlayerPic(playerModel.id, sprite);

            AuthFacebookResultVO vo = new AuthFacebookResultVO();
            vo.isSuccessful = true;
            vo.pic = sprite;
            vo.name = playerModel.name;
            vo.playerId = playerModel.id;
            vo.rating = playerModel.eloScore;
            vo.trophies2 = playerModel.trophies2;
            vo.countryId = playerModel.countryId;

            authFacebookResultSignal.Dispatch(vo);

            Release();
        }

        void LogSessionAnalytics()
        {
            if (signInWithAppleService.IsSignedIn())
            {
                analyticsService.Event(AnalyticsEventId.session_apple_id);
            }
            else
            {
                analyticsService.Event(AnalyticsEventId.session_guest);
            }
        }
    }
}
