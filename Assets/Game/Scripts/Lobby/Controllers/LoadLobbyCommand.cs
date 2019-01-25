/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @description
/// [add_description_here]

using strange.extensions.command.impl;
using TurboLabz.Chess;
using System;
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;
using TurboLabz.CPU;
using UnityEngine;
using TurboLabz.Multiplayer;

namespace TurboLabz.InstantGame
{
    public class LoadLobbyCommand : Command
    {
        // Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateMenuViewSignal updateMenuViewSignal { get; set; }
        [Inject] public UpdateAdsSignal updateAdsSignal { get; set; }
        [Inject] public SetSkinSignal setSkinSignal { get; set; }
        [Inject] public LoadGameSignal loadCPUGameDataSignal { get; set; }
        [Inject] public UpdatePlayerBucksSignal updatePlayerBucksDisplaySignal { get; set; }
        [Inject] public UpdateProfileSignal updateProfileSignal { get; set; }
        [Inject] public UpdateRemoveAdsSignal updateRemoveAdsDisplaySignal { get; set; }
        [Inject] public ResetActiveMatchSignal resetActiveMatchSignal{ get; set; }

        // Models
        [Inject] public ICPUGameModel cpuGameModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
		[Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public ICPUStatsModel cpuStatsModel { get; set; }

        // Services
        [Inject] public IFacebookService facebookService { get; set; }
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IRateAppService rateAppService { get; set; }

        public override void Execute()
        {
            setSkinSignal.Dispatch(playerModel.activeSkinId);

            resetActiveMatchSignal.Dispatch();
            loadCPUGameDataSignal.Dispatch();
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_LOBBY);

			LobbyVO vo = new LobbyVO(cpuGameModel, playerModel, metaDataModel);

            updateMenuViewSignal.Dispatch(vo);
            updateAdsSignal.Dispatch();
            updatePlayerBucksDisplaySignal.Dispatch(playerModel.bucks);

            string localizedMins = localizationService.Get(LocalizationKey.FREE_NO_ADS_MINUTES);
            string localizedHours = localizationService.Get(LocalizationKey.FREE_NO_ADS_HOURS);
            string localizedDays = localizationService.Get(LocalizationKey.FREE_NO_ADS_DAYS);
            string timeRemain =  TimeUtil.TimeToExpireString(playerModel.creationDate, metaDataModel.adsSettings.freeNoAdsPeriod, 
                localizedMins, localizedHours, localizedDays);

            updateRemoveAdsDisplaySignal.Dispatch(timeRemain, playerModel.HasAdsFreePeriod(metaDataModel.adsSettings));

            ProfileVO pvo = new ProfileVO();
            pvo.playerPic = picsModel.GetPlayerPic(playerModel.id);
            pvo.playerName = playerModel.name;
            pvo.eloScore = playerModel.eloScore;
            pvo.countryId = playerModel.countryId;
            pvo.isFacebookLoggedIn = facebookService.isLoggedIn();
            pvo.playerId = playerModel.id;

            if (pvo.isFacebookLoggedIn && pvo.playerPic == null)
            {
                pvo.playerPic = picsModel.GetPlayerPic(playerModel.id);
            }
                
            updateProfileSignal.Dispatch(pvo);

            if (!preferencesModel.hasRated && ((playerModel.totalGamesWon + cpuStatsModel.GetStarsCount()) >= metaDataModel.appInfo.rateAppThreshold))
            {
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_RATE_APP_DLG);
            }
        }
    }
}
