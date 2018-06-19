/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.context.impl;
using TurboLabz.CPU;
using TurboLabz.InstantGame;

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
            commandBinder.Bind<UndoMoveSignal>().To<UndoMoveCommand>();
            commandBinder.Bind<GetHintSignal>().To<GetHintCommand>();
            commandBinder.Bind<EnterPlaybackSignal>().To<EnterPlaybackCommand>();

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
            injectionBinder.Bind<ShowFiftyMoveDrawDialogSignal>().ToSingleton();
            injectionBinder.Bind<ShowThreefoldRepeatDrawDialogSignal>().ToSingleton();
            injectionBinder.Bind<HideDrawDialogSignal>().ToSingleton();
            injectionBinder.Bind<EnablePlayerTurnInteractionSignal>().ToSingleton();
            injectionBinder.Bind<EnableOpponentTurnInteractionSignal>().ToSingleton();
            injectionBinder.Bind<UpdateMoveForResumeSignal>().ToSingleton();
            injectionBinder.Bind<UpdateUndoButtonSignal>().ToSingleton();
            injectionBinder.Bind<DisableUndoButtonSignal>().ToSingleton();
            injectionBinder.Bind<DisableMenuButtonSignal>().ToSingleton();
            injectionBinder.Bind<DisableHintButtonSignal>().ToSingleton();
            injectionBinder.Bind<RenderHintSignal>().ToSingleton();
            injectionBinder.Bind<UpdateHintCountSignal>().ToSingleton();
            injectionBinder.Bind<TurnSwapSignal>().ToSingleton();
            injectionBinder.Bind<EnableResultsDialogButtonSignal>().ToSingleton();

            // Bind signals for dipatching from command to command
            injectionBinder.Bind<TakeTurnSwapTimeControlSignal>().ToSingleton();
            injectionBinder.Bind<ReceiveTurnSwapTimeControlSignal>().ToSingleton();
            injectionBinder.Bind<StopTimersSignal>().ToSingleton();
            injectionBinder.Bind<PauseTimersSignal>().ToSingleton();
            injectionBinder.Bind<ResumeTimersSignal>().ToSingleton();

            // Bind models
            injectionBinder.Bind<IChessboardModel>().To<ChessboardModel>().ToSingleton();
            injectionBinder.Bind<ICPUGameModel>().To<CPUGameModel>().ToSingleton();
        }
    }
}
