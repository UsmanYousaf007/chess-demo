/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-11-07 19:10:09 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.command.impl;

namespace TurboLabz.Gamebet
{
    public class LoadLobbyCommand : Command
    {
        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateLobbyViewSignal updateLobbyViewSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ILevelSettingsModel levelSettingsModel { get; set; }

        public override void Execute()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_LOBBY);

            LevelInfo currentLevelInfo = levelSettingsModel.currentLevelInfo;

            LobbyVO vo;

            vo.playerModel = playerModel;

            vo.currency1 = playerModel.currency1;
            vo.currency2 = playerModel.currency2;

            // TODO: Consider refactoring:
            // hasReachedMaxLevel = (playerModel.level == levelSettingsModel.maxLevel)
            // It's being used elsewhere as well.
            vo.hasReachedMaxLevel = (playerModel.level == levelSettingsModel.maxLevel);
            vo.level = playerModel.level;
            vo.levelStartXp = currentLevelInfo.startXp;
            vo.levelEndXp = currentLevelInfo.endXp;
            vo.xp = playerModel.xp;
            vo.displayName = playerModel.name;
            //vo.profilePicture = playerModel.profilePicture;
            vo.leagueId = playerModel.leagueId;

            updateLobbyViewSignal.Dispatch(vo);
        }
    }
}
