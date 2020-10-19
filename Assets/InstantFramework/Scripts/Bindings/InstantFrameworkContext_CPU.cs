/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.context.impl;
using TurboLabz.CPU;

namespace TurboLabz.InstantFramework
{
    public partial class InstantFrameworkContext : MVCSContext
    {
        protected void MapCPUGameBindings()
        {
            // Bind signals to commands
            commandBinder.Bind<LoadGameSignal>().To<LoadGameCommand>();
            commandBinder.Bind<StartCPUGameSignal>().To<StartCPUGameCommand>();
            commandBinder.Bind<RunTimeControlSignal>().To<RunTimeControlCommand>();
            commandBinder.Bind<ResignSignal>().To<ResignCommand>();
            commandBinder.Bind<SaveGameSignal>().To<SaveGameCommand>();
            commandBinder.Bind<AiTurnSignal>().To<AiTurnCommand>();
            commandBinder.Bind<ChessboardEventSignal>().To<ChessboardCommand>();
            commandBinder.Bind<SquareClickedSignal>().To<ChessboardSquareClickedCommand>();
            commandBinder.Bind<PromoSelectedSignal>().To<ChessboardPromoCommand>();
            commandBinder.Bind<DevFenValueChangedSignal>().To<DevFenChangedCommand>();
            commandBinder.Bind<GetHintSignal>().To<GetHintCommand>();
            commandBinder.Bind<SafeMoveSignal>().To<SafeMoveCommand>();
            commandBinder.Bind<ToggleSafeModeSignal>().To<ToggleSafeModeCommand>();
            commandBinder.Bind<StepSignal>().To<StepCommand>();
            commandBinder.Bind<OnboardingTooltipSignal>().To<OnboardingTooltipCommand>();

            // Bind views to mediators
            mediationBinder.Bind<GameView>().To<GameMediator>();

            // Bind signals for dispatching to/from mediators
            injectionBinder.Bind<UpdateGameInfoSignal>().ToSingleton();
            injectionBinder.Bind<SetupChessboardSignal>().ToSingleton();
            injectionBinder.Bind<InitTimersSignal>().ToSingleton();
            injectionBinder.Bind<InitInfiniteTimersSignal>().ToSingleton();
            injectionBinder.Bind<UpdatePlayerTimerSignal>().ToSingleton();
            injectionBinder.Bind<UpdateOpponentTimerSignal>().ToSingleton();
            injectionBinder.Bind<PlayerTimerExpiredSignal>().ToSingleton();
            injectionBinder.Bind<OpponentTimerExpiredSignal>().ToSingleton();
            injectionBinder.Bind<UpdateResultDialogSignal>().ToSingleton();
            injectionBinder.Bind<ShowPossibleMovesSignal>().ToSingleton();
            injectionBinder.Bind<HidePossibleMovesSignal>().ToSingleton();
            injectionBinder.Bind<UpdateChessboardSignal>().ToSingleton();
            injectionBinder.Bind<UpdatePlayerMoveSignal>().ToSingleton();
            injectionBinder.Bind<UpdatePlayerPrePromoMoveSignal>().ToSingleton();
            injectionBinder.Bind<UpdateOpponentMoveSignal>().ToSingleton();
            injectionBinder.Bind<ShowPlayerFromIndicatorSignal>().ToSingleton();
            injectionBinder.Bind<ShowPlayerToIndicatorSignal>().ToSingleton();
            injectionBinder.Bind<HidePlayerFromIndicatorSignal>().ToSingleton();
            injectionBinder.Bind<HidePlayerToIndicatorSignal>().ToSingleton();
            injectionBinder.Bind<ShowOpponentFromIndicatorSignal>().ToSingleton();
            injectionBinder.Bind<ShowOpponentToIndicatorSignal>().ToSingleton();
            injectionBinder.Bind<HideOpponentFromIndicatorSignal>().ToSingleton();
            injectionBinder.Bind<HideOpponentToIndicatorSignal>().ToSingleton();
            injectionBinder.Bind<UpdatePromoDialogSignal>().ToSingleton();
            injectionBinder.Bind<UpdatePromoSignal>().ToSingleton();
            injectionBinder.Bind<AutoQueenPromoSignal>().ToSingleton();
            injectionBinder.Bind<ShowFiftyMoveDrawDialogSignal>().ToSingleton();
            injectionBinder.Bind<ShowThreefoldRepeatDrawDialogSignal>().ToSingleton();
            injectionBinder.Bind<HideDrawDialogSignal>().ToSingleton();
            injectionBinder.Bind<EnablePlayerTurnInteractionSignal>().ToSingleton();
            injectionBinder.Bind<EnableOpponentTurnInteractionSignal>().ToSingleton();
            injectionBinder.Bind<UpdateMoveForResumeSignal>().ToSingleton();
            injectionBinder.Bind<DisableUndoButtonSignal>().ToSingleton();
            injectionBinder.Bind<DisableMenuButtonSignal>().ToSingleton();
            injectionBinder.Bind<DisableHintButtonSignal>().ToSingleton();
            injectionBinder.Bind<RenderHintSignal>().ToSingleton();
            injectionBinder.Bind<CancelHintSingal>().ToSingleton();
            injectionBinder.Bind<UpdateHintCountSignal>().ToSingleton();
            injectionBinder.Bind<UpdateHindsightCountSignal>().ToSingleton();
            injectionBinder.Bind<UpdateSafeMoveCountSignal>().ToSingleton();
            injectionBinder.Bind<UpdateSafeMoveStateSignal>().ToSingleton();
            injectionBinder.Bind<HindsightAvailableSignal>().ToSingleton();
            injectionBinder.Bind<HintAvailableSignal>().ToSingleton();
            injectionBinder.Bind<TurnSwapSignal>().ToSingleton();
            injectionBinder.Bind<EnableResultsDialogButtonSignal>().ToSingleton();
            injectionBinder.Bind<DisableUndoBtnSignal>().ToSingleton();
            injectionBinder.Bind<ToggleStepBackwardSignal>().ToSingleton();
            injectionBinder.Bind<ToggleStepForwardSignal>().ToSingleton();
            injectionBinder.Bind<ShowStrengthOnboardingTooltipSignal>().ToSingleton();
            injectionBinder.Bind<ShowCoachOnboardingTooltipSignal>().ToSingleton();
            injectionBinder.Bind<SetupSpecialHintSignal>().ToSingleton();
            injectionBinder.Bind<SpecialHintAvailableSignal>().ToSingleton();

            // Bind signals for dipatching from command to command
            injectionBinder.Bind<TakeTurnSwapTimeControlSignal>().ToSingleton();
            injectionBinder.Bind<ReceiveTurnSwapTimeControlSignal>().ToSingleton();
            injectionBinder.Bind<StopTimersSignal>().ToSingleton();
            injectionBinder.Bind<PauseTimersSignal>().ToSingleton();
            injectionBinder.Bind<ResumeTimersSignal>().ToSingleton();

            // Bind models
            injectionBinder.Bind<IChessboardModel>().To<ChessboardModel>().ToSingleton();
            injectionBinder.Bind<ICPUGameModel>().To<CPUGameModel>().ToSingleton();

            // Bind utils
            injectionBinder.Bind<ITimeControl>().To<TimeControl>(); 
        }
    }
}
