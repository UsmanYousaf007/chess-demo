/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-01-11 17:59:19 UTC+05:00
/// 
/// @description
/// [add_description_here]
using System.Collections.Generic;
using TurboLabz.InstantChess;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;

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
		public int playerBucks;
		public List<string> playerVGoods;
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
			activeSkinDisplayName = metaDataModel.items[activeSkinId].displayName;
			playerBucks = playerModel.bucks;
        }
    }
}
