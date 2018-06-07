/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;
using strange.extensions.command.impl;
using System.Collections.Generic;
using strange.extensions.promise.api;
using UnityEngine;

namespace TurboLabz.InstantChess
{
	public class LoadMetaDataCommand : Command
	{ 
        // TODO: Move Meta data defaults to GS server
        private const int DEFAULT_STARTING_BUCKS = 100;

        private const int ADS_REWARD_INCREMENT = 10;
        private const int ADS_MAX_IMPRESSIONS_PER_SLOT = 6;
        private const int ADS_SLOT_DEBUG_MINUTES = 2;
        private const int ADS_SLOT_MINUTES = 1440; // 24 hours

        #region LoadMetaData

        // Models
        [Inject] public IMetaDataModel model { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }
        [Inject] public ILeagueSettingsModel leagueSettingsModel { get; set; }
        [Inject] public ILevelSettingsModel levelSettingsModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public INavigatorModel navigatorModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPromotionsModel promotionsModel { get; set; }
        [Inject] public IRoomSettingsModel roomSettingsModel { get; set; }
        [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }

        // Services
		[Inject] public IStoreService storeService { get; set; }
        [Inject] public IBackendService backendService { get; set; }

        // Dispatch Signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public LoadMetaDataCompleteSignal loadMetaDataCompleteSignal { get; set; }

		public override void Execute()
        {
            Retain();
            ResetModels();

            // TODO: version needs to correspond to backend version rather than application version 
            backendService.GetInitData((int)(float.Parse(Application.version) * 100)).Then(OnGetInitData);
        }

        private void InitMetaData()
        {
            model.appInfo = appInfoModel;
            model.store = storeSettingsModel;

            AdSettings adSettings = new AdSettings();
            adSettings.maxImpressionsPerSlot = ADS_MAX_IMPRESSIONS_PER_SLOT;
            adSettings.slotMinutes = Debug.isDebugBuild ? ADS_SLOT_DEBUG_MINUTES : ADS_SLOT_MINUTES;
            adSettings.adsRewardIncrement = ADS_REWARD_INCREMENT;

            model.AddAdSettings(adSettings);
        }

        private void OnGetInitData(BackendResult result)
        {
            if (result != BackendResult.SUCCESS)
            {
                backendErrorSignal.Dispatch(result);
                Release();
                return;
            }

            InitMetaData();
            loadMetaDataCompleteSignal.Dispatch();
            Release();
        }

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
