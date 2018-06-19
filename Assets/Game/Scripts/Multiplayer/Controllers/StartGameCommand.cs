/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-15 17:46:40 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.command.impl;
using strange.extensions.mediation.api;

namespace TurboLabz.Multiplayer
{
    public class StartGameCommand : Command
    {
        // Dispatch signals
        [Inject] public ChessboardEventSignal chessboardEventSignal { get; set; }

        // Rebind mediator
        [Inject] public IMediationBinder mediationBinder { get; set; }


        public override void Execute()
        {
            chessboardEventSignal.Dispatch(ChessboardEvent.GAME_STARTED);
        }
    }
}
