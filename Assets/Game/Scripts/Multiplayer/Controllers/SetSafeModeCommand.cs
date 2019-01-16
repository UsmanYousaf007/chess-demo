/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;

namespace TurboLabz.Multiplayer
{
    public class SetSafeModeCommand : Command
    {
        // Dispatch Signals
        [Inject] public UpdateSafeMoveCountSignal updateSafeMoveCountSignal { get; set; }
        [Inject] public ConsumeVirtualGoodSignal consumeVirtualGoodSignal { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        public override void Execute()
        {
            chessboardModel.chessboards[matchInfoModel.activeChallengeId].inSafeMode = true;

            updateSafeMoveCountSignal.Dispatch(playerModel.PowerUpSafeMoveCount - 1);
            consumeVirtualGoodSignal.Dispatch(GSBackendKeys.PowerUp.SAFE_MOVE, 1);
        }
    }
}
