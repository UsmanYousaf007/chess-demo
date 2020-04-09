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

using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using TurboLabz.Chess;
using TurboLabz.InstantGame;

namespace TurboLabz.CPU
{
    public class ChessboardCommand : Command
    {
        // Signal parameters
        [Inject] public ChessboardEvent chessboardEvent { get; set; }

        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public ChessboardEventSignal chessboardEventSignal { get; set; }
        [Inject] public SetupChessboardSignal setupChessboardSignal { get; set; }
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
        [Inject] public ShowFiftyMoveDrawDialogSignal showFiftyMoveDrawDialogSignal { get; set; }
        [Inject] public ShowThreefoldRepeatDrawDialogSignal showThreefoldRepeatDrawDialogSignal { get; set; }
        [Inject] public HideDrawDialogSignal hideDrawDialogSignal { get; set; }
        [Inject] public UpdateResultDialogSignal updateResultsDialogSignal { get; set; }
        [Inject] public UpdatePromoDialogSignal updatePromoDialogSignal { get; set; }
        [Inject] public UpdatePromoSignal updatePromoSignal { get; set; }
        [Inject] public AutoQueenPromoSignal autoQueenPromoSignal { get; set; }
        [Inject] public RunTimeControlSignal runTimeControlSignal { get; set; }
        [Inject] public TakeTurnSwapTimeControlSignal takeTurnSwapTimeControlSignal { get; set; }
        [Inject] public ReceiveTurnSwapTimeControlSignal receiveTurnSwapTimeControlSignal { get; set; }
        [Inject] public StopTimersSignal stopTimersSignal { get; set; }
        [Inject] public EnablePlayerTurnInteractionSignal enablePlayerTurnInteraction { get; set; }
        [Inject] public EnableOpponentTurnInteractionSignal enableOpponentTurnInteraction { get; set; }
        [Inject] public UpdateMoveForResumeSignal updateMoveForResumeSignal { get; set; }
        [Inject] public DisableMenuButtonSignal disableMenuButtonSignal { get; set; }
        [Inject] public InitInfiniteTimersSignal initInfiniteTimersSignal { get; set; }
        [Inject] public SaveGameSignal saveGameSignal { get; set; }
        [Inject] public HindsightAvailableSignal hindsightAvailableSignal { get; set; }
        [Inject] public HintAvailableSignal hintAvailableSignal { get; set; }
        [Inject] public UpdateHintCountSignal updateHintCountSignal { get; set; }
        [Inject] public UpdateHindsightCountSignal updateHindsightCountSignal { get; set; }
        [Inject] public UpdateSafeMoveCountSignal updateSafeMoveCountSignal { get; set; }
        [Inject] public UpdateSafeMoveStateSignal updateSafeMoveStateSignal { get; set; }
        [Inject] public TurnSwapSignal turnSwapSignal { get; set; }
        [Inject] public UpdateGameInfoSignal updateGameInfoSignal { get; set; }
        [Inject] public SaveStatsSignal saveStatsSignal { get; set; }
        [Inject] public DisableUndoBtnSignal disableUndoBtnSignal { get; set; }
        [Inject] public ToggleStepForwardSignal toggleStepForwardSignal { get; set; }
        [Inject] public ToggleStepBackwardSignal toggleStepBackwardSignal { get; set; }
        [Inject] public CancelHintSingal cancelHintSignal { get; set; }
        [Inject] public OnboardingTooltipSignal onboardingTooltipSignal { get; set; }


        // Models
        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public ICPUGameModel cpuGameModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ICPUStatsModel statsModel { get; set; }
        [Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }

        // Services
        [Inject] public IChessService chessService { get; set; }
        [Inject] public IChessAiService chessAiService { get; set; }
        [Inject] public IAdsService adsService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }
        [Inject] public IAppsFlyerService appsFlyerService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }

        public override void Execute()
        {
            if (chessboardModel.currentState == null)
            {
                chessboardModel.currentState = new CCSDefault();
            }

//            LogUtil.Log("Current State: " + chessboardModel.currentState.GetType().Name, "white");
//            LogUtil.Log("ChessboardEvent: " + chessboardEvent, "white");
            appInfoModel.gameMode = GameMode.CPU;

            //if (chessboardEvent == ChessboardEvent.PLAYER_MOVE_COMPLETE)
            //{
            //    cancelHintSignal.Dispatch();
            //}

            CCS currentState = chessboardModel.currentState;
            CCS newState = chessboardModel.currentState.HandleEvent(this);

            if (newState != null)
            {
                chessboardModel.previousState = currentState;
                chessboardModel.currentState = newState;
                newState.RenderDisplayOnEnter(this);

//                LogUtil.Log("New State: " + newState.GetType().Name, "white");
            }
            else
            {
//                LogUtil.Log("Event ignored.", "white");
            }
        }
    }
}
