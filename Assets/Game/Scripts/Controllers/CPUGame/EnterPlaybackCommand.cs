/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-11 11:42:52 UTC+05:00
///
/// @description
/// [add_description_here]

using strange.extensions.command.impl;
using System.Collections.Generic;
using TurboLabz.Chess;
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;


namespace TurboLabz.InstantChess
{
    public class EnterPlaybackCommand : Command
    {
        // Models
        [Inject] public IChessboardModel chessboardModel { get; set; }

        // Dispatch Signal
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public EnableResultsDialogButtonSignal enableResultsDialogButtonSignal { get; set; }

        public override void Execute()
        {
            chessboardModel.inPlaybackMode = true;
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_PLAY);
            enableResultsDialogButtonSignal.Dispatch();
        }
    }
}
