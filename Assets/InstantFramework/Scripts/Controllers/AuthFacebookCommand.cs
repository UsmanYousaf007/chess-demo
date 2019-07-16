/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using UnityEngine;
using TurboLabz.TLUtils;
using System;

namespace TurboLabz.InstantFramework 
{
    public class AuthFacebookCommand : Command
    {
        // FB Auth -> GS_FB Auth -> FB_GetSocialName -> GS_SetSocialName -> GetFriends -> FB_GetSocialPic -> Conclude Auth

        // Services
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IFacebookService facebookService { get; set; }
        [Inject] public IPushNotificationService pushNotificationService { get; set; }

        // Dispatch Signals
        [Inject] public AuthFacebookResultSignal authFacebookResultSignal { get; set; }
        [Inject] public RefreshFriendsSignal refreshFriendsSignal { get; set; }
        [Inject] public RefreshCommunitySignal refreshCommunitySignal { get; set; }
        [Inject] public ToggleFacebookButton toggleFacebookButton { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }

        public override void Execute()
        {
            CommandBegin();
            facebookService.Auth().Then(OnAuthFacebook_BackendAuth);
        }

        private void OnAuthFacebook_BackendAuth(FacebookResult result, string accessToken)
        {
            if (result == FacebookResult.SUCCESS)
            {
                backendService.AuthFacebook(accessToken, false).Then(OnBackendAuth_GetSocialName);
            }
            else
            {
                // In case of Auth failure, don't dispatch a success signal
                CommandEnd(false);
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
                CommandEnd(false);
            }        
        }

        private void OnSocialName_BackendSetSocialName(FacebookResult result, string socialName)
        {
            if (result == FacebookResult.SUCCESS)
            {
                //Use edited name if player has edit his name else use facebook name
                if(String.IsNullOrEmpty(playerModel.editedName))
                {
                    backendService.SetPlayerSocialName(socialName).Then(OnBackendSetSocialName_GetFriends);
                }
                else
                {
                    backendService.SetPlayerSocialName(playerModel.editedName).Then(OnBackendSetSocialName_GetFriends);
                } 
            }
            else
            {
                CommandEnd(false);
            }
        }

        private void OnBackendSetSocialName_GetFriends(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                string pushToken = pushNotificationService.GetToken();
                if (pushToken != null)
                {
                    backendService.PushNotificationRegistration(pushToken);
                }
                backendService.FriendsOpInitialize().Then(OnGetFriends_GetSocialPic);
            }
            else
            {
                CommandEnd(false);
            }
        }

		private void OnGetFriends_GetSocialPic(BackendResult result)
		{
			if (result == BackendResult.SUCCESS)
			{
                facebookService.GetSocialPic(facebookService.GetFacebookId(), playerModel.id).Then(OnGetSocialPic_AuthConcluded);
			}
			else
			{
				CommandEnd(true);
			}
		}

        private void OnGetSocialPic_AuthConcluded(FacebookResult result, Sprite sprite, string facebookUserId)
        {
            if (result == FacebookResult.SUCCESS)
            {
                picsModel.SetPlayerPic(playerModel.id, sprite);
            }

            CommandEnd(true);
        }

        private void CommandBegin()
        {
            Retain();
            toggleFacebookButton.Dispatch(false);
        }

        private void CommandEnd(bool isSuccessful)
        {
            if (!isSuccessful)
            {
                facebookService.LogOut();
            }
            else
            {
                refreshFriendsSignal.Dispatch();
                refreshCommunitySignal.Dispatch();
            }

            Sprite playerPic = picsModel.GetPlayerPic(playerModel.id);

            AuthFacebookResultVO vo = new AuthFacebookResultVO();
            vo.isSuccessful = isSuccessful;
            vo.pic = playerPic;
            vo.name = playerModel.name;
            vo.playerId = playerModel.id;

            authFacebookResultSignal.Dispatch(vo);
            toggleFacebookButton.Dispatch(true);


            Release();
        }
    }
}
