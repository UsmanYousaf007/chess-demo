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
            commandBinder.Bind<OfferDrawSignal>().To<OfferDrawCommand>();
            commandBinder.Bind<AiTurnSignal>().To<AiTurnCommand>();
            commandBinder.Bind<ChessboardEventSignal>().To<ChessboardCommand>();
            commandBinder.Bind<SquareClickedSignal>().To<ChessboardSquareClickedCommand>();
            commandBinder.Bind<PromoSelectedSignal>().To<ChessboardPromoCommand>();
            commandBinder.Bind<BackendPlayerTurnSignal>().To<PlayerTurnCommand>();
            commandBinder.Bind<ExitLongMatchSignal>().To<ExitLongMatchCommand>();
            commandBinder.Bind<ResetActiveMatchSignal>().To<ResetActiveMatchCommand>();
            commandBinder.Bind<ToggleSafeModeSignal>().To<ToggleSafeModeCommand>();
            commandBinder.Bind<SafeMoveSignal>().To<SafeMoveCommand>();
            commandBinder.Bind<GetHintSignal>().To<GetHintCommand>();
            commandBinder.Bind<UpdatePlayerNotificationCountSignal>().To<UpdatePlayerNotificationCountCommand>();
            commandBinder.Bind<OnboardingTooltipSignal>().To<OnboardingTooltipCommand>();
            commandBinder.Bind<AnalyseMoveSignal>().To<AnalyseMoveCommand>();
            commandBinder.Bind<RenderMoveAnalysisSignal>().To<RenderMoveAnalysisCommand>();

            // Bind signals for dispatching to/from mediators
            injectionBinder.Bind<SetupChessboardSignal>().ToSingleton();
            injectionBinder.Bind<InitTimersSignal>().ToSingleton();
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
            injectionBinder.Bind<EnableGameChatSignal>().ToSingleton();
            injectionBinder.Bind<UpdateChatView>().ToSingleton();
            injectionBinder.Bind<DisplayChatMessageSignal>().ToSingleton();
            injectionBinder.Bind<RenderHintSignal>().ToSingleton();
            injectionBinder.Bind<CancelHintSingal>().ToSingleton();
            injectionBinder.Bind<TurnSwapSignal>().ToSingleton();
            injectionBinder.Bind<UpdateHintCountSignal>().ToSingleton();
            injectionBinder.Bind<HindsightAvailableSignal>().ToSingleton();
            injectionBinder.Bind<HintAvailableSignal>().ToSingleton();
            injectionBinder.Bind<UpdateHindsightCountSignal>().ToSingleton();
            injectionBinder.Bind<UpdateSafeMoveCountSignal>().ToSingleton();
            injectionBinder.Bind<UpdateSafeMoveStateSignal>().ToSingleton();
            injectionBinder.Bind<WaitingForOpponentAcceptSignal>().ToSingleton();
            injectionBinder.Bind<ChallengeMessageProcessedSignal>().ToSingleton();
            injectionBinder.Bind<DisableUndoBtnSignal>().ToSingleton();
            injectionBinder.Bind<ShowStrengthOnboardingTooltipSignal>().ToSingleton();
            injectionBinder.Bind<ShowCoachOnboardingTooltipSignal>().ToSingleton();
            injectionBinder.Bind<SetupSpecialHintSignal>().ToSingleton();
            injectionBinder.Bind<SpecialHintAvailableSignal>().ToSingleton();
            injectionBinder.Bind<FreeHintAvailableSignal>().ToSingleton();
            injectionBinder.Bind<UpdateKingCheckIndicatorSignal>().ToSingleton();
            injectionBinder.Bind<MoveAnalysiedSignal>().ToSingleton();
            injectionBinder.Bind<UpdateAnalysedMoveAdvantageSignal>().ToSingleton();
            injectionBinder.Bind<AiTurnRequestedSignal>().ToSingleton();

            // Bind signals for dipatching from command to command
            injectionBinder.Bind<TakeTurnSwapTimeControlSignal>().ToSingleton();
            injectionBinder.Bind<ReceiveTurnSwapTimeControlSignal>().ToSingleton();
            injectionBinder.Bind<StopTimersSignal>().ToSingleton();

            // Bind models
            injectionBinder.Bind<IChessboardModel>().To<ChessboardModel>().ToSingleton(); // Lifecycle handled

            // Bind mediator to view
            mediationBinder.Bind<GameView>().To<GameMediator>();

            // Bind utils
            injectionBinder.Bind<ITimeControl>().To<TimeControl>(); 
        }
    }
}
