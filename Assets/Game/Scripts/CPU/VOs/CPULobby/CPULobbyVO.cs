/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using TurboLabz.InstantChess;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantChess
{
    public class CPULobbyVO
    {
        public int minStrength;
        public int maxStrength;
        public int selectedStrength;
        public int[] durationMinutes;
        public int selectedDurationIndex;
        public ChessColor[] playerColors;
        public int selectedPlayerColorIndex;
        public bool inProgress;
        public int totalGames;
		public long playerBucks;
        public IOrderedDictionary<string, int> playerVGoods;
		public string activeSkinId;
		public string activeSkinDisplayName;

        public CPULobbyVO(ICPUGameModel cpuGameModel, IPlayerModel playerModel, IMetaDataModel metaDataModel)
        {
            minStrength = CPUSettings.MIN_STRENGTH;
            maxStrength = CPUSettings.MAX_STRENGTH;
            selectedStrength = cpuGameModel.cpuStrength;
            durationMinutes = CPUSettings.DURATION_MINUTES;
            selectedDurationIndex = cpuGameModel.durationIndex;
            playerColors = CPUSettings.PLAYER_COLORS;
            selectedPlayerColorIndex = cpuGameModel.playerColorIndex;
            inProgress = cpuGameModel.inProgress;
            totalGames = cpuGameModel.totalGames;
			activeSkinId = playerModel.activeSkinId;
			activeSkinDisplayName = metaDataModel.store.items[activeSkinId].displayName;
			playerBucks = playerModel.bucks;
            playerVGoods = playerModel.inventory;
        }
    }
}
