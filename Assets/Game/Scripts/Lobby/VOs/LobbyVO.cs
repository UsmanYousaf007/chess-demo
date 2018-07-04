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
        public IOrderedDictionary<string, int> playerVGoods;
		public string activeSkinId;
		public string activeSkinDisplayName;
        public Sprite playerPic;
        public string playerName;
        public bool isFacebookLoggedIn;
        public int eloScore;
        public string countryId;

        public LobbyVO(ICPUGameModel cpuGameModel, IPlayerModel playerModel, IMetaDataModel metaDataModel)
        {
            minStrength = CPUSettings.MIN_STRENGTH;
            maxStrength = CPUSettings.MAX_STRENGTH;
            selectedStrength = cpuGameModel.cpuStrength;
            inProgress = cpuGameModel.inProgress;
        	activeSkinId = playerModel.activeSkinId;
			activeSkinDisplayName = metaDataModel.store.items[activeSkinId].displayName;
			playerBucks = playerModel.bucks;
            playerVGoods = playerModel.inventory;
            playerPic = playerModel.socialPic;
            playerName = playerModel.name;
            eloScore = playerModel.eloScore;
            countryId = playerModel.countryId;
        }
    }
}
