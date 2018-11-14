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
	public class GetInitDataCommand : Command
	{ 
        // Models
        [Inject] public IMetaDataModel model { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public INavigatorModel navigatorModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }
        [Inject] public IAdsSettingsModel adsSettingsModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }

        // Todo: Move this to the game folder
        [Inject] public IChessboardModel chessboardModel { get; set; }

        // Services
		[Inject] public IStoreService storeService { get; set; }
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public ILocalDataService localDataService { get; set; }
        // Dispatch Signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public GetInitDataCompleteSignal getInitDataCompleteSignal { get; set; }

        public override void Execute()
        {
            Retain();
            ResetModels();

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

                getInitDataCompleteSignal.Dispatch();
            }
            else if (result != BackendResult.CANCELED)
            {
                backendErrorSignal.Dispatch(result);    
            }

            Release();
        }

        void ResetModels()
        {
            appInfoModel.Reset();
            playerModel.Reset();
            storeSettingsModel.Reset();
            adsSettingsModel.Reset();
        }

        string BuildAppData()
        {
            StringBuilder json = new StringBuilder();
            json.Append("{");

            //json.Append(<call build patch function from here>);

            json.Append("}");
            return json.ToString();
        }
	}
}
