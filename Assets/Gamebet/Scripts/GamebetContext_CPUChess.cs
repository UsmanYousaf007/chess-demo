/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-07 16:38:23 UTC+05:00
/// 
/// @description
/// 

using UnityEngine;

using strange.extensions.context.api;
using strange.extensions.context.impl;

using TurboLabz.Chess;
using TurboLabz.CPUChess;

namespace TurboLabz.Gamebet
{
    public partial class GamebetContext : MVCSContext
    {
        private void MapCPUGameBindings()
        {
            // Bind signals to commands
            commandBinder.Bind<StartCPUGameSignal>().To<StartNewCPUGameCommand>();
            commandBinder.Bind<RunTimeControlSignal>().To<RunTimeControlCommand>();
            commandBinder.Bind<ResignSignal>().To<ResignCommand>();
            commandBinder.Bind<SaveGameSignal>().To<SaveGameCommand>();
            commandBinder.Bind<LoadGameSignal>().To<LoadGameCommand>();
            commandBinder.Bind<AiTurnSignal>().To<AiTurnCommand>();
            commandBinder.Bind<ChessboardEventSignal>().To<ChessboardCommand>();
            commandBinder.Bind<SquareClickedSignal>().To<ChessboardSquareClickedCommand>();
            commandBinder.Bind<PromoSelectedSignal>().To<ChessboardPromoCommand>();

            commandBinder.Bind<AdjustStrengthSignal>().To<AdjustStrengthCommand>();
            commandBinder.Bind<AdjustTimerSignal>().To<AdjustDurationCommand>();
            commandBinder.Bind<AdjustPlayerColorSignal>().To<AdjustPlayerColorCommand>();
            commandBinder.Bind<DevFenValueChangedSignal>().To<DevFenChangedCommand>();
            commandBinder.Bind<UndoMoveSignal>().To<UndoMoveCommand>();
            commandBinder.Bind<GetHintSignal>().To<GetHintCommand>();

            // Bind views to mediators
            mediationBinder.Bind<CPUMenuView>().To<CPUMenuMediator>();
            mediationBinder.Bind<GameView>().To<GameMediator>();

            // Bind signals for dispatching to/from mediators
            injectionBinder.Bind<SetupChessboardSignal>().ToSingleton();
            injectionBinder.Bind<UpdateCPUGameInfoSignal>().ToSingleton();
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
            injectionBinder.Bind<UpdateCPUMenuViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdateStrengthSignal>().ToSingleton();
            injectionBinder.Bind<UpdateDurationSignal>().ToSingleton();
            injectionBinder.Bind<UpdatePlayerColorSignal>().ToSingleton();
            injectionBinder.Bind<RenderHintSignal>().ToSingleton();
            injectionBinder.Bind<UpdateHintCountSignal>().ToSingleton();
            injectionBinder.Bind<TurnSwapSignal>().ToSingleton();
            injectionBinder.Bind<NavigatorShowViewSignal>().ToSingleton();
            injectionBinder.Bind<NavigatorHideViewSignal>().ToSingleton();

            // Bind signals for dipatching from command to command
            injectionBinder.Bind<TakeTurnSwapTimeControlSignal>().ToSingleton();
            injectionBinder.Bind<ReceiveTurnSwapTimeControlSignal>().ToSingleton();
            injectionBinder.Bind<StopTimersSignal>().ToSingleton();

            // Bind models
            injectionBinder.Bind<IChessboardModel>().To<ChessboardModel>().ToSingleton();
            injectionBinder.Bind<ICPUGameModel>().To<CPUGameModel>().ToSingleton();
            injectionBinder.Bind<INavigatorModel>().To<NavigatorModel>().ToSingleton();

        }
    }
}
