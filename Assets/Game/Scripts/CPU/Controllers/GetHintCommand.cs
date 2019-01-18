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

using TurboLabz.InstantFramework;
using strange.extensions.promise.api;
using TurboLabz.TLUtils;
using TurboLabz.Chess;

namespace TurboLabz.CPU
{
    public class GetHintCommand : Command
    {
        // Parameters
        [Inject] public bool isHindsight { get; set; }

        // Dispatch Signals
        [Inject] public RenderHintSignal renderHintSignal { get; set; }
        [Inject] public ConsumeVirtualGoodSignal consumeVirtualGoodSignal { get; set; }
        [Inject] public UpdateHintCountSignal updateHintCountSignal { get; set; }
        [Inject] public UpdateHindsightCountSignal updateHindsightCountSignal { get; set; }


        // Services
        [Inject] public IChessService chessService { get; set; }
        [Inject] public IChessAiService chessAiService { get; set; }

        // Models
        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public ICPUGameModel cpuGameModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        public override void Execute()
        {
            Retain();

            chessboardModel.usedHelp = true;

            string fen = isHindsight ? chessboardModel.previousPlayerTurnFen : chessService.GetFen();
            chessAiService.NewGame();
            chessAiService.SetPosition(fen);

            AiMoveInputVO vo = new AiMoveInputVO();
            vo.aiColor = chessboardModel.playerColor;
            vo.playerColor = chessboardModel.opponentColor;
            vo.squares = chessboardModel.squares;
            vo.aiMoveDelay = AiMoveDelay.NONE;
            vo.isHint = true;

            IPromise<FileRank, FileRank, string> promise = chessAiService.GetAiMove(vo);
            promise.Then(OnAiMove);
        }

        private void OnAiMove(FileRank from, FileRank to, string promo)
        {
            HintVO vo;
            vo.fromSquare = chessboardModel.squares[from.file, from.rank];
            vo.toSquare = chessboardModel.squares[to.file, to.rank];
            vo.isHindsight = isHindsight;
            renderHintSignal.Dispatch(vo);

            if (isHindsight)
            {
                updateHindsightCountSignal.Dispatch(playerModel.PowerUpHindsightCount - 1);
                consumeVirtualGoodSignal.Dispatch(GSBackendKeys.PowerUp.HINDSIGHT, 1);
            }
            else
            {
                updateHintCountSignal.Dispatch(playerModel.PowerUpHintCount - 1);
                consumeVirtualGoodSignal.Dispatch(GSBackendKeys.PowerUp.HINT, 1);
            }

            Release();
        }
    }
}
