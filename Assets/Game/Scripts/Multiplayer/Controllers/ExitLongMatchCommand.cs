/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;

namespace TurboLabz.Multiplayer 
{
    public class ExitLongMatchCommand : Command
    {
        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public ResetActiveMatchSignal resetActiveMatchSignal{ get; set; }

        // Models
        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public ICPUStatsModel cpuStatsModel { get; set; }

        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }

        public override void Execute()
        {
            //loadLobbySignal.Dispatch();

            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_FRIENDS);
            //resetActiveMatchSignal.Dispatch();

            //if (!preferencesModel.hasRated && ((playerModel.totalGamesWon + cpuStatsModel.GetStarsCount()) >= metaDataModel.appInfo.rateAppThreshold))
            //{
            //    navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_RATE_APP_DLG);
            //}
        }
    }
}
