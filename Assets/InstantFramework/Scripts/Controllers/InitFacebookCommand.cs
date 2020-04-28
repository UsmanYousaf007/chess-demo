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
                Release();
            }
        }

        private void OnFacebookAuthComplete(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                facebookService.GetSocialPic(facebookService.GetFacebookId(), playerModel.id).Then(OnGetSocialPic);
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

        void OnGetSocialPic(FacebookResult result, Sprite sprite, string facebookUserId)
        {
            picsModel.SetPlayerPic(playerModel.id, sprite);

            AuthFacebookResultVO vo = new AuthFacebookResultVO();
            vo.isSuccessful = true;
            vo.pic = sprite;
            vo.name = playerModel.name;
            vo.playerId = playerModel.id;
            vo.rating = playerModel.eloScore;

            authFacebookResultSignal.Dispatch(vo);

            Release();
        }

    }
}
