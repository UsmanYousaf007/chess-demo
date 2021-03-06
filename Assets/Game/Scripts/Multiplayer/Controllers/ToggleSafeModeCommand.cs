/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;

namespace TurboLabz.Multiplayer
{
    public class ToggleSafeModeCommand : Command
    {

        // Dispatch Signals
        [Inject] public UpdateSafeMoveStateSignal updateSafeMoveStateSignal { get; set; }

        // Models
        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }

        public override void Execute()
        {
            Chessboard activeChessboard = chessboardModel.chessboards[matchInfoModel.activeChallengeId];

            activeChessboard.inSafeMode = !activeChessboard.inSafeMode;
            updateSafeMoveStateSignal.Dispatch(activeChessboard.inSafeMode);
            preferencesModel.isSafeMoveOn = activeChessboard.inSafeMode;
        }
    }
}
