/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-19 21:59:37 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.command.impl;
using UnityEngine;

namespace TurboLabz.Gamebet
{
    public class ReceptionCommand : Command
    {
        // Dispatch signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public LoadViewSignal loadViewSignal { get; set; }
        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }
        [Inject] public UpdateSetPlayerSocialNameViewSignal updateSetPlayerSocialNameViewSignal { get; set; }
        [Inject] public GetPlayerProfilePictureSignal getPlayerProfilePictureSignal { get; set; }
        [Inject] public GetOpponentProfilePictureSignal getOpponentProfilePictureSignal { get; set; }
        [Inject] public StartGameSignal startGameSignal { get; set; }
        [Inject] public ApplyPlayerInventorySignal applyPlayerInventorySignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }

        public override void Execute()
        {
            Retain();
            backendService.CheckGameVersion().Then(OnGetVersionData);
        }

        public void OnGetVersionData(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                if (AppVersionConstants.CURRENT_APP_VERSION < appInfoModel.appVersion)
                {
                    loadViewSignal.Dispatch(ViewId.UPDATE_APP);
                    Release(); 
                }
                else
                {
                    backendService.GetInitData().Then(OnGetInitData);
                } 
            }
            else
            {
                backendErrorSignal.Dispatch(result);
            }
        }

        // TODO Mubeen to code review
        private void OnGetInitData(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {

                if (!matchInfoModel.isResuming)
                {
                    AvatarThumbsContainer.Load();
                    AvatarBorderThumbsContainer.Load();
                }

                // TODO(mubeeniqbal): Evaluate if the
                // GetPlayerProfilePictureCommand itself should be responsible
                // for checking if the profile picture can even be fetched i.e.
                // if an external authentication is even present or should the
                // caller be responsible for it.
                getPlayerProfilePictureSignal.Dispatch();

                applyPlayerInventorySignal.Dispatch();

                if (matchInfoModel.isResuming)
                {
                    getOpponentProfilePictureSignal.Dispatch();
                    startGameSignal.Dispatch();

                    Release();
     
                    return;
                }

                // If the player is connecting using a social account for the
                // first time then we need to set the player's name from his/her
                // social account.
                if (playerModel.hasExternalAuth && !playerModel.isSocialNameSet)
                {
                    loadViewSignal.Dispatch(ViewId.SET_PLAYER_SOCIAL_NAME);

                    SetPlayerSocialNameVO vo;
                    vo.nameOptions = playerModel.name.Split(' ');

                    updateSetPlayerSocialNameViewSignal.Dispatch(vo);
                }
                else
                {
                    loadLobbySignal.Dispatch();
                }
            }
            else
            {
                backendErrorSignal.Dispatch(result);
            }

            Release();
        }
    }
}