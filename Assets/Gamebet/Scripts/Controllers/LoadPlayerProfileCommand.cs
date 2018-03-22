/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-11-23 16:44:27 UTC+05:00
///
/// @description
/// [add_description_here]

using System.Collections.Generic;

using strange.extensions.command.impl;

namespace TurboLabz.Gamebet
{
    public class LoadPlayerProfileCommand : Command
    {
        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public LoadViewSignal loadViewSignal { get; set; }
        [Inject] public UpdatePlayerProfileViewSignal updatePlayerProfileViewSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ILevelSettingsModel levelSettingsModel { get; set; }
        [Inject] public IRoomSettingsModel roomSettingsModel { get; set; }

        public override void Execute()
        {
            // Display the menu
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_PLAYER_PROFILE);
            //loadViewSignal.Dispatch(ViewId.PLAYER_PROFILE);

            LevelInfo currentLevelInfo = levelSettingsModel.currentLevelInfo;

            PlayerProfileVO vo;
            vo.tag = playerModel.tag;
            vo.name = playerModel.name;
            vo.countryId = playerModel.countryId;
            vo.profilePicture = playerModel.profilePicture;
            vo.currency1 = playerModel.currency1;
            vo.currency2 = playerModel.currency2;
            vo.currency1Winnings = playerModel.currency1Winnings;

            // TODO: Consider refactoring:
            // hasReachedMaxLevel = (playerModel.level == levelSettingsModel.maxLevel)
            // It's being used elsewhere as well.
            vo.hasReachedMaxLevel = (playerModel.level == levelSettingsModel.maxLevel);
            vo.level = playerModel.level;
            vo.levelStartXp = currentLevelInfo.startXp;
            vo.levelEndXp = currentLevelInfo.endXp;
            vo.xp = playerModel.xp;
            vo.leagueId = playerModel.leagueId;

            IDictionary<string, RoomRecordVO> roomRecords = new Dictionary<string, RoomRecordVO>();

            foreach (RoomRecord record in playerModel.roomRecords.Values)
            {
                string roomId = record.id;
                RoomSetting roomInfo = roomSettingsModel.settings[roomId];

                RoomRecordVO recordVO;
                recordVO.id = roomId;
                recordVO.gameDuration = roomInfo.gameDuration;
                recordVO.gamesWon = record.gamesWon;
                recordVO.gamesLost = record.gamesLost;
                recordVO.gamesDrawn = record.gamesDrawn;
                recordVO.trophiesWon = record.trophiesWon;
                recordVO.roomTitleId = record.roomTitleId;

                roomRecords.Add(roomId, recordVO);
            }

            vo.roomRecords = roomRecords;

            vo.totalGamesWon = playerModel.totalGamesWon;
            vo.totalGamesLost = playerModel.totalGamesLost;
            vo.totalGamesDrawn = playerModel.totalGamesDrawn;
            vo.totalGames = playerModel.totalGames;
            vo.playerModel = playerModel;

            updatePlayerProfileViewSignal.Dispatch(vo);
        }
    }
}
