/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.context.impl;
using TurboLabz.Multiplayer;

namespace TurboLabz.InstantFramework
{
    public partial class InstantFrameworkContext : MVCSContext
    {
        protected void MapMultiplayerGameBindings()
        {
            commandBinder.Bind<RunTimeControlSignal>().To<RunTimeControlCommand>();
            commandBinder.Bind<ClaimFiftyMoveDrawSignal>().To<ClaimFiftyMoveDrawCommand>();
            commandBinder.Bind<ClaimThreefoldRepeatDrawSignal>().To<ClaimThreefoldRepeatDrawCommand>();
            commandBinder.Bind<ResignSignal>().To<ResignCommand>();
            commandBinder.Bind<AiTurnSignal>().To<AiTurnCommand>();
            commandBinder.Bind<ChessboardEventSignal>().To<ChessboardCommand>();
            commandBinder.Bind<SquareClickedSignal>().To<ChessboardSquareClickedCommand>();
            commandBinder.Bind<PromoSelectedSignal>().To<ChessboardPromoCommand>();
            commandBinder.Bind<BackendPlayerTurnSignal>().To<PlayerTurnCommand>();
            commandBinder.Bind<EnterPlaybackSignal>().To<EnterPlaybackCommand>();


            // Bind signals for dispatching to/from mediators
            injectionBinder.Bind<SetupChessboardSignal>().ToSingleton();
            injectionBinder.Bind<SetupSearchModeSignal>().ToSingleton();
            injectionBinder.Bind<UpdateMatchInfoSignal>().ToSingleton();
            injectionBinder.Bind<InitTimersSignal>().ToSingleton();
            injectionBinder.Bind<UpdatePlayerTimerSignal>().ToSingleton();
            injectionBinder.Bind<UpdateOpponentTimerSignal>().ToSingleton();
            injectionBinder.Bind<PlayerTimerExpiredSignal>().ToSingleton();
            injectionBinder.Bind<OpponentTimerExpiredSignal>().ToSingleton();
            injectionBinder.Bind<ShowResultsDialogSignal>().ToSingleton();
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
            injectionBinder.Bind<ShowPromoDialogSignal>().ToSingleton();
            injectionBinder.Bind<HidePromoDialogSignal>().ToSingleton();
            injectionBinder.Bind<UpdatePromoSignal>().ToSingleton();
            injectionBinder.Bind<ShowFiftyMoveDrawDialogSignal>().ToSingleton();
            injectionBinder.Bind<ShowThreefoldRepeatDrawDialogSignal>().ToSingleton();
            injectionBinder.Bind<HideDrawDialogSignal>().ToSingleton();
            injectionBinder.Bind<EnablePlayerTurnInteractionSignal>().ToSingleton();
            injectionBinder.Bind<EnableOpponentTurnInteractionSignal>().ToSingleton();
            injectionBinder.Bind<UpdateMoveForResumeSignal>().ToSingleton();

            // Bind signals for dipatching from command to command
            injectionBinder.Bind<TakeTurnSwapTimeControlSignal>().ToSingleton();
            injectionBinder.Bind<ReceiveTurnSwapTimeControlSignal>().ToSingleton();
            injectionBinder.Bind<StopTimersSignal>().ToSingleton();

            // Bind models
            injectionBinder.Bind<IChessboardModel>().To<ChessboardModel>().ToSingleton();

            // Bind mediator to view
            mediationBinder.Bind<GameView>().To<GameMediator>();
        }
    }
}

/*
 * // Bind signals to commands
commandBinder.Bind<RunTimeControlSignal>().To<RunTimeControlCommand>();
commandBinder.Bind<ClaimFiftyMoveDrawSignal>().To<ClaimFiftyMoveDrawCommand>();
commandBinder.Bind<ClaimThreefoldRepeatDrawSignal>().To<ClaimThreefoldRepeatDrawCommand>();
commandBinder.Bind<ResignSignal>().To<ResignCommand>();
commandBinder.Bind<AiTurnSignal>().To<AiTurnCommand>();
commandBinder.Bind<ChessboardEventSignal>().To<ChessboardCommand>();
commandBinder.Bind<SquareClickedSignal>().To<ChessboardSquareClickedCommand>();
commandBinder.Bind<PromoSelectedSignal>().To<ChessboardPromoCommand>();
commandBinder.Bind<BackendPlayerTurnSignal>().To<PlayerTurnCommand>();


// Bind signals for dispatching to/from mediators
injectionBinder.Bind<SetupChessboardSignal>().ToSingleton();
injectionBinder.Bind<UpdateMatchInfoSignal>().ToSingleton();
injectionBinder.Bind<InitTimersSignal>().ToSingleton();
injectionBinder.Bind<UpdatePlayerTimerSignal>().ToSingleton();
injectionBinder.Bind<UpdateOpponentTimerSignal>().ToSingleton();
injectionBinder.Bind<PlayerTimerExpiredSignal>().ToSingleton();
injectionBinder.Bind<OpponentTimerExpiredSignal>().ToSingleton();
injectionBinder.Bind<ShowResultsDialogSignal>().ToSingleton();
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
injectionBinder.Bind<ShowPromoDialogSignal>().ToSingleton();
injectionBinder.Bind<HidePromoDialogSignal>().ToSingleton();
injectionBinder.Bind<UpdatePromoSignal>().ToSingleton();
injectionBinder.Bind<ShowFiftyMoveDrawDialogSignal>().ToSingleton();
injectionBinder.Bind<ShowThreefoldRepeatDrawDialogSignal>().ToSingleton();
injectionBinder.Bind<HideDrawDialogSignal>().ToSingleton();
injectionBinder.Bind<OpponentDisconnectedSignal>().ToSingleton();
injectionBinder.Bind<OpponentReconnectedSignal>().ToSingleton();
injectionBinder.Bind<EnablePlayerTurnInteractionSignal>().ToSingleton();
injectionBinder.Bind<EnableOpponentTurnInteractionSignal>().ToSingleton();
injectionBinder.Bind<UpdateMoveForResumeSignal>().ToSingleton();

// Bind signals for dipatching from command to command
injectionBinder.Bind<TakeTurnSwapTimeControlSignal>().ToSingleton();
injectionBinder.Bind<ReceiveTurnSwapTimeControlSignal>().ToSingleton();
injectionBinder.Bind<StopTimersSignal>().ToSingleton();

// Bind models
injectionBinder.Bind<IChessboardModel>().To<ChessboardModel>().ToSingleton();

// Bind mediator to view
mediationBinder.Bind<GameView>().To<GameMediator>();*/