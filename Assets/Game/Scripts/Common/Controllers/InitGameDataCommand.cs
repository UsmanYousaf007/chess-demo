/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;

namespace TurboLabz.InstantFramework
{
	public class InitGameDataCommand : Command
	{ 
        // Models
        [Inject] public IMetaDataModel model { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public INavigatorModel navigatorModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
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

            backendService.GetInitData(appInfoModel.appBackendVersion).Then(OnComplete);
        }

        void OnComplete(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                model.appInfo = appInfoModel;
                model.store = storeSettingsModel;
                model.adsSettings = adsSettingsModel;

                loadMetaDataCompleteSignal.Dispatch();
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
            matchInfoModel.Reset();
            playerModel.Reset();
            storeSettingsModel.Reset();
            adsSettingsModel.Reset();
        }
	}
}
