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
    public class LobbyVO
    {
        public int minStrength;
        public int maxStrength;
        public int selectedStrength;
        public bool inProgress;
        public long playerBucks;
		public string activeSkinId;
        public int onlineCount;
        public int gamesTodayCount;

        public LobbyVO(ICPUGameModel cpuGameModel, IPlayerModel playerModel, IMetaDataModel metaDataModel)
        {
            minStrength = CPUSettings.MIN_STRENGTH;
            maxStrength = CPUSettings.MAX_STRENGTH;
            selectedStrength = cpuGameModel.cpuStrength;
            inProgress = cpuGameModel.inProgress;
        	activeSkinId = playerModel.activeSkinId;
			playerBucks = playerModel.bucks;
            onlineCount = metaDataModel.appInfo.onlineCount;
            gamesTodayCount = metaDataModel.appInfo.gamesPlayedCount;
        }
    }
}
