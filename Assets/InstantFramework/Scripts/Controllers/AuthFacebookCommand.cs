/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using UnityEngine;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework 
{
    public class AuthFacebookCommand : Command
    {
        // FB Auth -> GS_FB Auth -> FB_GetSocialName -> GS_SetSocialName -> FB_GetSocialPic -> Conclude Auth

        // Services
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IFacebookService facebookService { get; set; }

        // Dispatch Signals
        [Inject] public AuthFacebookSuccessSignal authFacebookSuccessSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }

        public override void Execute()
        {
            CommandBegin();
            facebookService.Auth().Then(OnAuthFacebook_BackendAuth);
        }

        private void OnAuthFacebook_BackendAuth(FacebookResult result, string accessToken)
        {
            if (result == FacebookResult.SUCCESS)
            {
                backendService.AuthFacebook(accessToken).Then(OnBackendAuth_GetSocialName);
            }
            else
            {
                // In case of Auth failure, don't dispatch a success signal
                Release();
            }
        }

        private void OnBackendAuth_GetSocialName(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                facebookService.GetSocialName().Then(OnSocialName_BackendSetSocialName);
            }        
            else
            {
                CommandEnd();
            }        
        }

        private void OnSocialName_BackendSetSocialName(FacebookResult result, string socialName)
        {
            if (result == FacebookResult.SUCCESS)
            {
                backendService.SetPlayerSocialName(socialName).Then(OnBackendSetSocialName_GetSocialPic);
            }
            else
            {
                CommandEnd();
            }
        }

        private void OnBackendSetSocialName_GetSocialPic(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                facebookService.GetSocialPic(facebookService.GetPlayerUserIdAlias()).Then(OnGetSocialPic_AuthConcluded);
            }
            else
            {
                CommandEnd();
            }
        }

        private void OnGetSocialPic_AuthConcluded(FacebookResult result, Sprite sprite)
        {
            if (result == FacebookResult.SUCCESS)
            {
                playerModel.socialPic = sprite;
            }

            CommandEnd();
        }

        private void CommandBegin()
        {
            Retain();
        }

        private void CommandEnd()
        {
            LogUtil.Log("PLAYER PIC:" + playerModel.socialPic.name, "cyan");
            LogUtil.Log("PLAYER NAME:" + playerModel.name, "cyan");
            authFacebookSuccessSignal.Dispatch(playerModel.socialPic, playerModel.name);
            Release();
        }
    }
}
