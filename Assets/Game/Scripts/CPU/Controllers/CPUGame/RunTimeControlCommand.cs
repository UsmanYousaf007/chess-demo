/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-21 00:53:12 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System;

using strange.extensions.command.impl;

using TurboLabz.Common;
using TurboLabz.Gamebet;
using TurboLabz.Chess;

namespace TurboLabz.CPUChess
{
    public class RunTimeControlCommand : Command
    {
        // Dispatch signals
        [Inject] public UpdatePlayerTimerSignal updatePlayerTimerSignal { get; set; }
        [Inject] public UpdateOpponentTimerSignal updateOpponentTimerSignal { get; set; }
        [Inject] public PlayerTimerExpiredSignal playerTimerExpiredSignal { get; set; }
        [Inject] public OpponentTimerExpiredSignal opponentTimerExpiredSignal { get; set; }
        [Inject] public InitTimersSignal initTimersSignal { get; set; }
        [Inject] public ChessboardEventSignal chessboardEventSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        // Listen to signals
        [Inject] public TakeTurnSwapTimeControlSignal takeTurnSwapTimeControlSignal { get; set; }
        [Inject] public ReceiveTurnSwapTimeControlSignal receiveTurnSwapTimeControlSignal { get; set; }
        [Inject] public StopTimersSignal stopTimersSignal { get; set; }

        // Models
        [Inject] public ICPUGameModel cpuGameModel { get; set; }
        [Inject] public IChessboardModel chessboardModel { get; set; }

        // Utils
        [Inject] public ITimeControl timeControl { get; set; }

        public override void Execute()
        {
            Retain();
            AddListeners();

            // Initialize player timer
            TimeSpan playerTimer = chessboardModel.playerTimer;
            TimeSpan opponentTimer = chessboardModel.opponentTimer;

            // Update the view for starting values
            InitTimerVO vo;
            vo.startingTimer = chessboardModel.gameDuration;
            vo.playerTimer = playerTimer;
            vo.opponentTimer = opponentTimer;
            vo.isPlayerTurn = chessboardModel.isPlayerTurn;
            initTimersSignal.Dispatch(vo);

            // Kick off
            timeControl.SetTimers(playerTimer, opponentTimer);
            timeControl.StartTimers(chessboardModel.isPlayerTurn);
        }

        private void OnPlayerTimerTick()
        {
            updatePlayerTimerSignal.Dispatch(timeControl.playerDisplayTimer);
            chessboardModel.playerTimer = timeControl.playerRealTimer;
        }

        private void OnOpponentTimerTick()
        {
            updateOpponentTimerSignal.Dispatch(timeControl.opponentDisplayTimer);
            chessboardModel.opponentTimer = timeControl.opponentRealTimer;
        }

        private void OnTakeTurnSwapTimeControl()
        {
            timeControl.SwapTimers();
        }

        private void OnReceiveTurnSwapTimeControl()
        {
            timeControl.SwapTimers();
        }

        private void OnPlayerTimerExpired()
        {
            playerTimerExpiredSignal.Dispatch();
            EndGame(null);
            RemoveListeners();
            Release();
        }

        private void OnOpponentTimerExpired()
        {
            opponentTimerExpiredSignal.Dispatch();
            EndGame(playerModel.id);
            RemoveListeners();
            Release();
        }

        private void EndGame(string winnerId)
        {
            chessboardModel.gameEndReason = GameEndReason.TIMER_EXPIRED;
            chessboardModel.winnerId = winnerId;
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CPU_PLAY);
            chessboardEventSignal.Dispatch(ChessboardEvent.GAME_ENDED);

        }

        private void OnStopTimersSignal()
        {
            timeControl.StopTimers();
            RemoveListeners();
            Release();
        }

        // TODO: Discuss architecture for releasing non-request commands when
        // the backend disconnects.
        private void OnDisconnectBackend()
        {
            timeControl.StopTimers();
            RemoveListeners();
            Release();
        }

        private void AddListeners()
        {
            timeControl.playerTickSignal.AddListener(OnPlayerTimerTick);
            timeControl.opponentTickSignal.AddListener(OnOpponentTimerTick);
            timeControl.playerTimerExpiredSignal.AddListener(OnPlayerTimerExpired);
            timeControl.opponentTimerExpiredSignal.AddListener(OnOpponentTimerExpired);

            takeTurnSwapTimeControlSignal.AddListener(OnTakeTurnSwapTimeControl);
            receiveTurnSwapTimeControlSignal.AddListener(OnReceiveTurnSwapTimeControl);
            stopTimersSignal.AddListener(OnStopTimersSignal);
        }

        private void RemoveListeners()
        {
            timeControl.playerTickSignal.RemoveListener(OnPlayerTimerTick);
            timeControl.opponentTickSignal.RemoveListener(OnOpponentTimerTick);
            timeControl.playerTimerExpiredSignal.RemoveListener(OnPlayerTimerExpired);
            timeControl.opponentTimerExpiredSignal.RemoveListener(OnOpponentTimerExpired);

            takeTurnSwapTimeControlSignal.RemoveListener(OnTakeTurnSwapTimeControl);
            receiveTurnSwapTimeControlSignal.RemoveListener(OnReceiveTurnSwapTimeControl);
            stopTimersSignal.RemoveListener(OnStopTimersSignal);
        }
    }
}
