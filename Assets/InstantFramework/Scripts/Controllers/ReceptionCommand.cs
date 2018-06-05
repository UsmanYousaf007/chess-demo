/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class ReceptionCommand : Command
    {
        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }
        //[Inject] public UpdateSetPlayerSocialNameViewSignal updateSetPlayerSocialNameViewSignal { get; set; }
        [Inject] public GetPlayerProfilePictureSignal getPlayerProfilePictureSignal { get; set; }
        //[Inject] public GetOpponentProfilePictureSignal getOpponentProfilePictureSignal { get; set; }
        [Inject] public ApplyPlayerInventorySignal applyPlayerInventorySignal { get; set; }
        [Inject] public InitBackendOnceSignal initBackendOnceSignal { get; set; }
        [Inject] public LoadMetaDataSignal loadMetaDataSignal  { get; set; }
        [Inject] public LoadMetaDataCompleteSignal loadMetaDataCompleteSignal { get; set; }

        public override void Execute()
        {
            Retain();

            loadMetaDataCompleteSignal.AddListener(OnLoadDataComplete);

            ResetModels();
            GSRequestSession.Instance.EndSession();
            loadMetaDataSignal.Dispatch();
            initBackendOnceSignal.Dispatch();
        }
            
        private void OnLoadDataComplete()
        {
            loadMetaDataCompleteSignal.RemoveListener(OnLoadDataComplete);

            // Check version information. Prompt the player if an update is needed.
            if (appInfoModel.appVersionValid == false)
            {
                // TODO: handle application update message
                TurboLabz.TLUtils.LogUtil.Log("ERROR: VERSION MISMATCH", "red");
                //navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_UPDATE_APP);
                Release();
                return;
            }

            // TODO: handle loading avatar thumbs
            // Load borders and avatars
            //AvatarThumbsContainer.Load();
            //AvatarBorderThumbsContainer.Load();
                
            // Call for player profile picture while loading in parallel
            getPlayerProfilePictureSignal.Dispatch();
                
            applyPlayerInventorySignal.Dispatch();

            // TODO: move out to own command
            // If the player is connecting using a social account for the
            // first time then we need to set the player's name from his/her
            // social account.
            if (playerModel.hasExternalAuth && !playerModel.isSocialNameSet)
            {
                // TODO: handle update social name
                //navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SET_SOCIAL_NAME);

                //SetPlayerSocialNameVO vo;
                //vo.nameOptions = playerModel.name.Split(' ');

                //updateSetPlayerSocialNameViewSignal.Dispatch(vo);
            }
            else
            {
                
                loadLobbySignal.Dispatch();
                //loadGameSignal.Dispatch();
            }
 
            Release();
        }


        #region ResetModels

        // Models
        [Inject] public IAppInfoModel appInfoModel { get; set; }
        [Inject] public ILeagueSettingsModel leagueSettingsModel { get; set; }
        [Inject] public ILevelSettingsModel levelSettingsModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public INavigatorModel navigatorModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPromotionsModel promotionsModel { get; set; }
        [Inject] public IRoomSettingsModel roomSettingsModel { get; set; }
        [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }

        void ResetModels()
        {
            appInfoModel.Reset();
            leagueSettingsModel.Reset();
            levelSettingsModel.Reset();
            matchInfoModel.Reset();
            playerModel.Reset();
            promotionsModel.Reset();
            roomSettingsModel.Reset();
            storeSettingsModel.Reset();
         }

        #endregion


    }
}