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
using HUF.AnalyticsHBI.API;

namespace TurboLabz.InstantFramework
{
	public class GetInitDataCommand : Command
	{ 
        // Params
        [Inject] public bool isResume { get; set; }
        // Models
        [Inject] public IMetaDataModel model { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public INavigatorModel navigatorModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }
        [Inject] public IAdsSettingsModel adsSettingsModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }
        [Inject] public IChatModel chatModel { get; set; }
        [Inject] public IRewardsSettingsModel rewardsSettingsModel { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public TurboLabz.CPU.ICPUGameModel cPUGameModel { get; set; }


        // Todo: Move this to the game folder
        [Inject] public IChessboardModel chessboardModel { get; set; }

        // Services
		[Inject] public IStoreService storeService { get; set; }
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public ILocalDataService localDataService { get; set; }

        // Dispatch Signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public GetInitDataFailedSignal getInitDataFailedSignal { get; set; }

        public override void Execute()
        {
            Retain();

            string appData = BuildAppData();
            backendService.GetInitData(appInfoModel.appBackendVersion, appData).Then(OnComplete);
        }

        void OnComplete(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                model.appInfo = appInfoModel;
                model.store = storeSettingsModel;
                model.adsSettings = adsSettingsModel;
                model.rewardsSettings = rewardsSettingsModel;
                model.settingsModel = settingsModel;
            }
            else if (result != BackendResult.CANCELED)
            {
                getInitDataFailedSignal.Dispatch(result);
                backendErrorSignal.Dispatch(result);    
            }

            Release();
        }


        string BuildAppData()
        {
            AppData appData;
            appData.lastSavedChatId = chatModel.lastSavedChatIdOnLaunch;
            appData.clientVersion = appInfoModel.clientVersion;
            appData.isResume = isResume;
            appData.playerSkillLevel = playerModel.skillLevel;
            appData.inProgress = cPUGameModel.inProgress;
            appData.hbiUserId = HAnalyticsHBI.UserId;

            return JsonUtility.ToJson(appData);
        }
	}

    [Serializable]
    public struct AppData
    {
        public string lastSavedChatId;
        public string clientVersion;
        public bool isResume;
        public string playerSkillLevel;
        public bool inProgress;
        public string hbiUserId;
    }
}
