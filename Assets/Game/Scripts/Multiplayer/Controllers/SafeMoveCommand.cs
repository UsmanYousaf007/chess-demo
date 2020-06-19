/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;

namespace TurboLabz.Multiplayer
{
    public class SafeMoveCommand : Command
    {
        // Parameters
        [Inject] public bool confirm { get; set; }

        // Dispatch Signals
        [Inject] public ChessboardEventSignal chessboardEventSignal { get; set; }
        [Inject] public UpdateSafeMoveCountSignal updateSafeMoveCountSignal { get; set; }
        [Inject] public ConsumeVirtualGoodSignal consumeVirtualGoodSignal { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }

        public override void Execute()
        {
            if (confirm)
            {
                chessboardEventSignal.Dispatch(ChessboardEvent.MOVE_CONFIRMED);
            }
            else
            {
                chessboardEventSignal.Dispatch(ChessboardEvent.MOVE_UNDO);

                UpdateSafeMoveCounts();
            }
        }

        void UpdateSafeMoveCounts()
        {
            updateSafeMoveCountSignal.Dispatch(playerModel.PowerUpSafeMoveCount - 1);

            if (playerModel.PowerUpSafeMoveCount - 1 == 0)
            {
                chessboardModel.chessboards[matchInfoModel.activeChallengeId].inSafeMode = false;
            }

            consumeVirtualGoodSignal.Dispatch(GSBackendKeys.PowerUp.SAFE_MOVE, 1);
        }
    }
}
