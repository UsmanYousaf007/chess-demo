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
        [Inject] public StartCPUGameSignal startCPUGameSignal { get; set; }
        [Inject] public ToggleStepBackwardSignal toggleStepBackwardSignal { get; set; }
        [Inject] public ToggleStepForwardSignal toggleStepForwardSignal { get; set; }

        // Models
        [Inject] public IChessboardModel chessboardModel { get; set; }

        
        public override void Execute()
        {
            LogUtil.Log("STEP:" + stepForward, "blue");

            if (stepForward)
            {
                int trimmedMoveListCount = chessboardModel.trimmedMoveList.Count;

                if (trimmedMoveListCount > 1)
                {
                    chessboardModel.moveList.Add(chessboardModel.trimmedMoveList[trimmedMoveListCount - 1]);
                    chessboardModel.moveList.Add(chessboardModel.trimmedMoveList[trimmedMoveListCount - 2]);
                    chessboardModel.trimmedMoveList.RemoveRange(trimmedMoveListCount - 2, 2);
                }

                if (chessboardModel.trimmedMoveList.Count == 0)
                {
                    toggleStepForwardSignal.Dispatch(false);
                }
            }
            else
            {
                int moveListCount = chessboardModel.moveList.Count;

                if (moveListCount > 1)
                {
                    chessboardModel.trimmedMoveList.Add(chessboardModel.moveList[moveListCount - 1]);
                    chessboardModel.trimmedMoveList.Add(chessboardModel.moveList[moveListCount - 2]);
                    chessboardModel.moveList.RemoveRange(moveListCount - 2, 2);

                    toggleStepForwardSignal.Dispatch(true);
                }

                if (chessboardModel.moveList.Count < 2)
                {
                    toggleStepBackwardSignal.Dispatch(false);
                }
            }
            chessboardModel.usedHelp = true;
            startCPUGameSignal.Dispatch(false);
        }
    }
}
