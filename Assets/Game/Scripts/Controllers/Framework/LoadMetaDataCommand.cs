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
        [Inject] public IAdsSettingsModel adsSettingsModel { get; set; }

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

            appInfoModel.RetrieveAppVersion();
            LogUtil.Log("Client AppVersion: " + appInfoModel.appVersion + " Backend Version: " + appInfoModel.appBackendVersion, "cyan");
            backendService.GetInitData(int.Parse(appInfoModel.appBackendVersion)).Then(OnGetInitData);
        }

        private void InitMetaData()
        {
            model.appInfo = appInfoModel;
            model.store = storeSettingsModel;
            model.adsSettings = adsSettingsModel;
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
            adsSettingsModel.Reset();
        }
	}
}
