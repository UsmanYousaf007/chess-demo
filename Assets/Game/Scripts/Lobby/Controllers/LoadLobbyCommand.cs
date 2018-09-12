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

        // Models
        [Inject] public ICPUGameModel cpuGameModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
		[Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }

        // Services
        [Inject] public IFacebookService facebookService { get; set; }
        [Inject] public ILocalizationService localizationService { get; set; }

        public override void Execute()
        {
            setSkinSignal.Dispatch(playerModel.activeSkinId);

            loadCPUGameDataSignal.Dispatch();
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_LOBBY);

			LobbyVO vo = new LobbyVO(cpuGameModel, playerModel, metaDataModel);

            updateMenuViewSignal.Dispatch(vo);
            updateAdsSignal.Dispatch();
            updatePlayerBucksDisplaySignal.Dispatch(playerModel.bucks);

            string timeRemain = GetFreeNoAdsPeriodString(playerModel.creationDate, metaDataModel.adsSettings.freeNoAdsPeriod);
            updateRemoveAdsDisplaySignal.Dispatch(timeRemain, playerModel.OwnsVGood(GSBackendKeys.SHOP_ITEM_FEATURE_REMOVE_ADS));

            ProfileVO pvo = new ProfileVO();
            pvo.playerPic = playerModel.profilePic;
            pvo.playerName = playerModel.name;
            pvo.eloScore = playerModel.eloScore;
            pvo.countryId = playerModel.countryId;
            pvo.isFacebookLoggedIn = facebookService.isLoggedIn();

            if (pvo.isFacebookLoggedIn && pvo.playerPic == null)
            {
                pvo.playerPic = picsModel.GetPlayerPic(playerModel.id);
            }
                
            updateProfileSignal.Dispatch(pvo);
        }

        private string GetFreeNoAdsPeriodString(long msUTC, int expireDays)
        {
            DateTime expireDate = TurboLabz.TLUtils.TimeUtil.ToDateTime(msUTC);
            expireDate = expireDate.AddDays(expireDays);

            TimeSpan elapsedTime;
            if (DateTime.Compare(expireDate, DateTime.UtcNow) > 0)
            {
                elapsedTime = expireDate.Subtract(DateTime.UtcNow);
            }
            else
            {
                return null;
            }

            if (elapsedTime.TotalHours < 1)
            {
                return localizationService.Get(LocalizationKey.LONG_PLAY_MINUTES, 
                    Mathf.Max(1, Mathf.FloorToInt((float)elapsedTime.TotalMinutes)));
            }
            else if (elapsedTime.TotalDays < 1)
            {
                return localizationService.Get(LocalizationKey.LONG_PLAY_HOURS, 
                    Mathf.Max(1, Mathf.FloorToInt((float)elapsedTime.TotalHours)));
            }
            else
            {
                return localizationService.Get(LocalizationKey.LONG_PLAY_DAYS, 
                    Mathf.Max(1, Mathf.FloorToInt((float)elapsedTime.TotalDays)));
            }
        }
    }
}
