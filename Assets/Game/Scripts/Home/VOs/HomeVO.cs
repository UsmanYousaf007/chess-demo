/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using TurboLabz.InstantGame;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using TurboLabz.CPU;
using UnityEngine;

namespace TurboLabz.InstantGame
{
    public class HomeVO
    {
        public int minStrength;
        public int maxStrength;
        public int selectedStrength;
        public bool inProgress;
        public long playerBucks;
        public IOrderedDictionary<string, int> playerVGoods;
		public string activeSkinId;
		public string activeSkinDisplayName;
        public int onlineCount;

        public StoreVO storeVO;

        public HomeVO(ICPUGameModel cpuGameModel, IPlayerModel playerModel, IMetaDataModel metaDataModel)
        {
            minStrength = CPUSettings.MIN_STRENGTH;
            maxStrength = CPUSettings.MAX_STRENGTH;
            selectedStrength = cpuGameModel.cpuStrength;
            inProgress = cpuGameModel.inProgress;
        	activeSkinId = playerModel.activeSkinId;
			activeSkinDisplayName = metaDataModel.store.items[activeSkinId].displayName;
			playerBucks = playerModel.bucks;
            playerVGoods = playerModel.inventory;
            onlineCount = metaDataModel.appInfo.onlineCount;

            storeVO = new StoreVO();
            storeVO.playerModel = playerModel;
            storeVO.storeSettingsModel = metaDataModel;
        }
    }
}
