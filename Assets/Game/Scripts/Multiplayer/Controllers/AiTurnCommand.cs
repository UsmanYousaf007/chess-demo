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

namespace TurboLabz.MPChess
{
    public class AiTurnCommand : Command
    {
        // Dispatch Signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }

        // Services
        [Inject] public IChessService chessService { get; set; }
        [Inject] public IChessAiService chessAiService { get; set; }
        [Inject] public IBackendService backendService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IChessboardModel chessboardModel { get; set; }

        public override void Execute()
        {
            Retain();

            ++chessboardModel.aiMoveNumber;
            chessAiService.SetPosition(chessService.GetFen());

            AiMoveInputVO vo = new AiMoveInputVO();
            vo.aiColor = chessboardModel.opponentColor;
            vo.playerColor = chessboardModel.playerColor;
            vo.lastPlayerMove = chessboardModel.lastPlayerMove;
            vo.squares = chessboardModel.squares;
            vo.opponentTimer = chessboardModel.backendOpponentTimer;
            vo.timeControl = chessboardModel.aiTimeControl;
            vo.opponentLevel = chessboardModel.opponentLevel;
            vo.aiMoveNumber = chessboardModel.aiMoveNumber;
            vo.overrideSpeed = chessboardModel.overrideAiSpeed;

            IPromise<FileRank, FileRank, string> promise = chessAiService.GetAiMove(vo, chessboardModel.overrideAiStrength);
            promise.Then(OnAiMove);
        }

        private void OnAiMove(FileRank from, FileRank to, string promo)
        {
            if (chessboardModel.aiTimeControl != AiTimeControl.ONE_MINUTE &&
                chessboardModel.aiWillResign &&
                chessService.GetScore(chessboardModel.playerColor) > BotSettings.AI_RESIGN_SCORE_THRESHOLD)
            {
                backendService.AiResign().Then(OnResign);
            }
            else
            {
                backendService.TakeAiTurn(from, to, promo).Then(OnTurnTaken); ;
            }
        }

        private void OnTurnTaken(BackendResult result)
        {
            if (result != BackendResult.SUCCESS)
            {
                backendErrorSignal.Dispatch(result);
            }

            Release();
        }

        private void OnResign(BackendResult result)
        {
            if (result != BackendResult.SUCCESS)
            {
                backendErrorSignal.Dispatch(result);
            }

            Release();
        }

            
    }


}
