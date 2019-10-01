/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-15 17:46:40 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.command.impl;
using TurboLabz.Chess;
using System;
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;

namespace TurboLabz.CPU
{
    public class StepCommand : Command
    {
        // Parameters
        [Inject] public bool stepForward { get; set; }

        // Dispatch signals
        //[Inject] public ChessboardEventSignal chessboardEventSignal { get; set; }

        // Models
        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public ICPUGameModel cpuGameModel { get; set; }

        
        public override void Execute()
        {
            LogUtil.Log("STEP:" + stepForward, "blue");
        }
    }
}
