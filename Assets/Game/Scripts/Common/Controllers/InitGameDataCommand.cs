/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

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
        [Inject] public ILocalDataService localDataService { get; set; }
        [Inject] public IFacebookService facebookService { get; set; }

        // Dispatch Signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public LoadMetaDataCompleteSignal loadMetaDataCompleteSignal { get; set; }
        [Inject] public AuthFacebookResultSignal authFacebookSuccessSignal { get; set; }

        public override void Execute()
        {
            Retain();
            ResetModels();

            string appData = BuildAppData();
            backendService.GetInitData(appInfoModel.appBackendVersion, appData).Then(OnComplete);

            // Fetch facebook pic in parallel with backend init data fetch
            if (facebookService.isLoggedIn())
            {
                facebookService.GetSocialPic(facebookService.GetPlayerUserIdAlias(), true).Then(OnGetSocialPic);
            }
        }

        void OnComplete(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                model.appInfo = appInfoModel;
                model.store = storeSettingsModel;
                model.adsSettings = adsSettingsModel;

                loadMetaDataCompleteSignal.Dispatch();
                Release();
            }
            else if (result != BackendResult.CANCELED)
            {
                backendErrorSignal.Dispatch(result);    
                Release();
            }
        }

        void OnGetSocialPic(FacebookResult result, Sprite sprite)
        {
            playerModel.socialPic = sprite;
            authFacebookSuccessSignal.Dispatch(true, playerModel.socialPic, playerModel.name);
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

        string BuildAppData()
        {
            StringBuilder json = new StringBuilder();
            json.Append("{");

			json.Append(FBAccessToken_Build());
            json.Append(Patch1_Build());

            json.Append("}");
            return json.ToString();
        }

		string FBAccessToken_Build()
		{
			string fbToken = facebookService.GetAccessToken();
			if (fbToken == null) 
			{
				return "";
			}

			StringBuilder json = new StringBuilder();
			json.Append("\"fbToken\": ");

			json.Append("\"");
			json.Append(fbToken);
			json.Append("\"");

			return json.ToString();
		}

        string Patch1_Build()
        {
            string id = "";
            string activeSkinId = "";
            int bucks = 0;
            List<string> vGoods = new List<string>();
            int adLifetimeImpressions = 0;
            int adSlotImpressions = 0;
            long adSlotId = 0;

            bool isAvailable = Patch1_LoadPlayerDataFromFile(ref id, ref activeSkinId, ref bucks, ref vGoods, 
                                                ref adLifetimeImpressions, ref adSlotImpressions, ref adSlotId);
            if (isAvailable == false)
            {
                return "";
            }

            StringBuilder json = new StringBuilder();
            json.Append("\"patch1\":{");

            json.Append("\"activeSkinId\":");
            json.Append("\"");
            json.Append(activeSkinId);
            json.Append("\"");
            json.Append(", ");
            json.Append("\"bucks\":");
            json.Append(bucks.ToString());
            json.Append(", ");
            json.Append("\"vGoods\":");
            json.Append("[");
            for(int i = 0; i < vGoods.Count; i++)
            {
                json.Append("\"");
                json.Append(vGoods[i]);
                json.Append("\"");
                if (i < (vGoods.Count - 1))
                {
                    json.Append(", ");
                }
            }
            json.Append("]");
            json.Append(", ");
            json.Append("\"adLifetimeImpressions\":");
            json.Append(adLifetimeImpressions.ToString());

            json.Append("}");

            return json.ToString();
        }
            
        public bool Patch1_LoadPlayerDataFromFile(ref string id, ref string activeSkinId, ref int bucks, ref List<string> vGoods, 
                                    ref int adLifetimeImpressions, ref int adSlotImpressions, ref long adSlotId)
        {
            // PLAYER MODEL
            const string PLAYER_SAVE_FILENAME = "playersSave";
            const string PLAYER_BUCKS = "playerBucks";
            const string PLAYER_VGOODS = "playerVGoods";
            const string PLAYER_ID = "playerId";
            const string PLAYER_ACTIVE_SKIN_ID = "playerActiveSkinId";
            const string PLAYER_AD_LIFE_TIME_IMPRESSIONS = "playerAdLifetimeImpressions";
            const string PLAYER_AD_SLOT_IMPRESSIONS = "playerAdSlotImpressions";
            const string PLAYER_AD_SLOT_ID = "playerAdSlotId";

            if (!localDataService.FileExists(PLAYER_SAVE_FILENAME))
            {
                return false;
            }

            try
            {
                ILocalDataReader reader = localDataService.OpenReader(PLAYER_SAVE_FILENAME);

                id = reader.Read<string>(PLAYER_ID);
                activeSkinId = reader.Read<string>(PLAYER_ACTIVE_SKIN_ID);
                bucks = reader.Read<int>(PLAYER_BUCKS);
                vGoods = reader.ReadList<string>(PLAYER_VGOODS);
                adLifetimeImpressions = reader.Read<int>(PLAYER_AD_LIFE_TIME_IMPRESSIONS);
                adSlotImpressions = reader.Read<int>(PLAYER_AD_SLOT_IMPRESSIONS);
                adSlotId = reader.Read<long>(PLAYER_AD_SLOT_ID);

                reader.Close();            
                localDataService.DeleteFile(PLAYER_SAVE_FILENAME);
            }
            catch (Exception e)
            {
                // Assume the file is corrupted.
                localDataService.DeleteFile(PLAYER_SAVE_FILENAME);
                TLUtils.LogUtil.Log("LoadPlayerDataFromFile() EXEPTION! " + e, "red");
                return false;
            }

            return true;
        }
	}
}
