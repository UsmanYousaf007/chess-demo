/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;


namespace TurboLabz.Multiplayer 
{
    public class ExitLongMatchCommand : Command
    {
        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        // Models
        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }

        public override void Execute()
        {
            if (matchInfoModel.activeChallengeId != null)
            {
                chessboardModel.chessboards[matchInfoModel.activeChallengeId].currentState = null;
            }

            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_FRIENDS);
        }
    }
}
