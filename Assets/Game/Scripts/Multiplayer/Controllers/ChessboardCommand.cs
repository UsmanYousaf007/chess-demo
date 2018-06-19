/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-02-15 20:38:14 UTC+05:00
///
/// @description
/// [add_description_here]

using strange.extensions.command.impl;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;

namespace TurboLabz.Multiplayer
{
    public class ChessboardCommand : Command
    {
        // Signal parameters
        [Inject] public ChessboardEvent chessboardEvent { get; set; }

        // Dispatch signals
        [Inject] public ChessboardEventSignal chessboardEventSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public SetupChessboardSignal setupChessboardSignal { get; set; }
        [Inject] public UpdateMatchInfoSignal updateMatchInfoSignal { get; set; }
        [Inject] public UpdateChessboardSignal updateChessboardSignal { get; set; }
        [Inject] public AiTurnSignal aiTurnSignal { get; set; }
        [Inject] public ShowPlayerFromIndicatorSignal showPlayerFromIndicatorSignal { get; set; }
        [Inject] public ShowPlayerToIndicatorSignal showPlayerToIndicatorSignal { get; set; }
        [Inject] public HidePlayerFromIndicatorSignal hidePlayerFromIndicatorSignal { get; set; }
        [Inject] public HidePlayerToIndicatorSignal hidePlayerToIndicatorSignal { get; set; }
        [Inject] public ShowPossibleMovesSignal showPossibleMovesSignal { get; set; }
        [Inject] public HidePossibleMovesSignal hidePossibleMovesSignal { get; set; }
        [Inject] public UpdateOpponentMoveSignal updateOpponentMoveSignal { get; set; }
        [Inject] public UpdatePlayerMoveSignal updatePlayerMoveSignal { get; set; }
        [Inject] public UpdatePlayerPrePromoMoveSignal updatePlayerPrePromoMoveSignal { get; set; }
        [Inject] public BackendPlayerTurnSignal backendPlayerTurnSignal { get; set; }
        [Inject] public ShowFiftyMoveDrawDialogSignal showFiftyMoveDrawDialogSignal { get; set; }
        [Inject] public ShowThreefoldRepeatDrawDialogSignal showThreefoldRepeatDrawDialogSignal { get; set; }
        [Inject] public HideDrawDialogSignal hideDrawDialogSignal { get; set; }
        [Inject] public ShowResultsDialogSignal showResultsDialogSignal { get; set; }
        [Inject] public ShowPromoDialogSignal showPromoDialogSignal { get; set; }
        [Inject] public HidePromoDialogSignal hidePromoDialogSignal { get; set; }
        [Inject] public UpdatePromoSignal updatePromoSignal { get; set; }
        [Inject] public ClaimFiftyMoveDrawSignal claimFiftyMoveDrawSignal { get; set; }
        [Inject] public ClaimThreefoldRepeatDrawSignal claimThreefoldRepeatDrawSignal { get; set; }
        [Inject] public RunTimeControlSignal runTimeControlSignal { get; set; }
        [Inject] public TakeTurnSwapTimeControlSignal takeTurnSwapTimeControlSignal { get; set; }
        [Inject] public ReceiveTurnSwapTimeControlSignal receiveTurnSwapTimeControlSignal { get; set; }
        [Inject] public StopTimersSignal stopTimersSignal { get; set; }
        [Inject] public EnablePlayerTurnInteractionSignal enablePlayerTurnInteraction { get; set; }
        [Inject] public EnableOpponentTurnInteractionSignal enableOpponentTurnInteraction { get; set; }
        [Inject] public UpdateMoveForResumeSignal updateMoveForResumeSignal { get; set; }

        // Models
        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }

        // Services
        [Inject] public IChessService chessService { get; set; }
        [Inject] public IChessAiService chessAiService { get; set; }

        public override void Execute()
        {
            //LogUtil.Log("ChessboardEvent: " + chessboardEvent);

            if (chessboardModel.currentState == null)
            {
                chessboardModel.currentState = new CCSDefault();
            }

            CCS currentState = chessboardModel.currentState;
            CCS newState = chessboardModel.currentState.HandleEvent(this);

            if (newState != null)
            {
                chessboardModel.previousState = currentState;
                chessboardModel.currentState = newState;
                newState.RenderDisplayOnEnter(this);

                //LogUtil.Log(chessboardEvent + ": " + newState.GetType().Name, "white");
            }
        }

    }
}
