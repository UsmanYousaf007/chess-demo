/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using System.Text;
using TurboLabz.InstantGame;
using System;
using System.Collections.Generic;

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

        // Dispatch Signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public LoadMetaDataCompleteSignal loadMetaDataCompleteSignal { get; set; }

		public override void Execute()
        {
            Retain();
            ResetModels();

            string appData = BuildAppData();

            TLUtils.LogUtil.Log("BUILD APP DATA: " + appData, "cyan");

            backendService.GetInitData(appInfoModel.appBackendVersion, appData).Then(OnComplete);
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

        string BuildPatch1()
        {
            SaveToFile(); // TEST

            string id = "";
            string activeSkinId = "";
            int bucks = 0;
            List<string> vGoods = new List<string>();
            int adLifetimeImpressions = 0;
            int adSlotImpressions = 0;
            long adSlotId = 0;

            bool isAvailable = LoadFromFile(ref id, ref activeSkinId, ref bucks, ref vGoods, 
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

        string BuildAppData()
        {
            StringBuilder json = new StringBuilder();

            json.Append("{");
            json.Append(BuildPatch1());
            json.Append("}");

            return json.ToString();
        }

        public bool LoadFromFile(ref string id, ref string activeSkinId, ref int bucks, ref List<string> vGoods, 
                                    ref int adLifetimeImpressions, ref int adSlotImpressions, ref long adSlotId)
        {
            if (!localDataService.FileExists(SaveKeys.PLAYER_SAVE_FILENAME))
            {
                return false;
            }

            try
            {
                ILocalDataReader reader = localDataService.OpenReader(SaveKeys.PLAYER_SAVE_FILENAME);

                id = reader.Read<string>(SaveKeys.PLAYER_ID);
                activeSkinId = reader.Read<string>(SaveKeys.PLAYER_ACTIVE_SKIN_ID);
                bucks = reader.Read<int>(SaveKeys.PLAYER_BUCKS);
                vGoods = reader.ReadList<string>(SaveKeys.PLAYER_VGOODS);
                adLifetimeImpressions = reader.Read<int>(SaveKeys.PLAYER_AD_LIFE_TIME_IMPRESSIONS);
                adSlotImpressions = reader.Read<int>(SaveKeys.PLAYER_AD_SLOT_IMPRESSIONS);
                adSlotId = reader.Read<long>(SaveKeys.PLAYER_AD_SLOT_ID);

                reader.Close();            
                localDataService.DeleteFile(SaveKeys.PLAYER_SAVE_FILENAME);
            }
            catch (Exception e)
            {
                // Assume the file is corrupted.
                localDataService.DeleteFile(SaveKeys.PLAYER_SAVE_FILENAME);
                return false;
            }

            return true;
        }

        public void SaveToFile()
        {
            try
            {
                ILocalDataWriter writer = localDataService.OpenWriter(SaveKeys.PLAYER_SAVE_FILENAME);

                writer.Write<string>(SaveKeys.PLAYER_ID, "TEST_id");
                writer.Write<string>(SaveKeys.PLAYER_ACTIVE_SKIN_ID, "SkinDeepSea");
                writer.Write<int>(SaveKeys.PLAYER_BUCKS, 9999);

                List<string> vGoods = new List<string>();
                vGoods.Add("SkinAmazon");
                vGoods.Add("TEST_VGOOD2");

                writer.WriteList<string>(SaveKeys.PLAYER_VGOODS, vGoods);
                writer.Write<int>(SaveKeys.PLAYER_AD_LIFE_TIME_IMPRESSIONS, 8888);
                writer.Write<int>(SaveKeys.PLAYER_AD_SLOT_IMPRESSIONS, 7777);
                writer.Write<long>(SaveKeys.PLAYER_AD_SLOT_ID, 6666);

                writer.Close();
            }
            catch (Exception e)
            {
                if (localDataService.FileExists(SaveKeys.PLAYER_SAVE_FILENAME))
                {
                    localDataService.DeleteFile(SaveKeys.PLAYER_SAVE_FILENAME);
                }

                TLUtils.LogUtil.Log("Critical error when saving player data. File deleted. " + e, "red");
            }       
        }

	}
}
