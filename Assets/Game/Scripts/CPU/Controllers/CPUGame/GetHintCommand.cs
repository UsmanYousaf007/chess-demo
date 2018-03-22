/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-16 13:27:47 UTC+05:00
///
/// @description
/// [add_description_here]

using strange.extensions.command.impl;

using TurboLabz.Gamebet;
using strange.extensions.promise.api;
using TurboLabz.Common;
using TurboLabz.Chess;

namespace TurboLabz.CPUChess
{
    public class GetHintCommand : Command
    {
        // Dispatch Signals
        [Inject] public RenderHintSignal renderHintSignal { get; set; }

        // Services
        [Inject] public IChessService chessService { get; set; }
        [Inject] public IChessAiService chessAiService { get; set; }

        // Models
        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public ICPUGameModel cpuGameModel { get; set; }

        public override void Execute()
        {
            if (chessboardModel.availableHints == 0)
            {
                return;
            }

            Retain();

            --chessboardModel.availableHints;

            chessAiService.SetPosition(chessService.GetFen());

            AiMoveInputVO vo = new AiMoveInputVO();
            vo.aiColor = chessboardModel.playerColor;
            vo.playerColor = chessboardModel.opponentColor;
            vo.squares = chessboardModel.squares;
            vo.timeControl = AiTimeControl.HINT;

            IPromise<FileRank, FileRank, string> promise = chessAiService.GetAiMove(vo, AiOverrideStrength.SMART);
            promise.Then(OnAiMove);
        }

        private void OnAiMove(FileRank from, FileRank to, string promo)
        {
            LogUtil.Log("RECEIVED HINT: " + from + " -> " + to + " promo:" + promo);

            HintVO vo;
            vo.fromSquare = chessboardModel.squares[from.file, from.rank];
            vo.toSquare = chessboardModel.squares[to.file, to.rank];
            vo.availableHints = chessboardModel.availableHints;
            renderHintSignal.Dispatch(vo);

            Release();
        }
    }
}
