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
using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;

namespace TurboLabz.Multiplayer
{
    public class RunTimeControlCommand : Command
    {
        // Dispatch signals
        [Inject] public UpdatePlayerTimerSignal updatePlayerTimerSignal { get; set; }
        [Inject] public UpdateOpponentTimerSignal updateOpponentTimerSignal { get; set; }
        [Inject] public PlayerTimerExpiredSignal playerTimerExpiredSignal { get; set; }
        [Inject] public OpponentTimerExpiredSignal opponentTimerExpiredSignal { get; set; }
        [Inject] public InitTimersSignal initTimersSignal { get; set; }

        // Listen to signals
        [Inject] public TakeTurnSwapTimeControlSignal takeTurnSwapTimeControlSignal { get; set; }
        [Inject] public ReceiveTurnSwapTimeControlSignal receiveTurnSwapTimeControlSignal { get; set; }
        [Inject] public StopTimersSignal stopTimersSignal { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Utils
        [Inject] public ITimeControl timeControl { get; set; }

        private Chessboard chessboard;

        public override void Execute()
        {
            Retain();
            AddListeners();

            chessboard = chessboardModel.chessboards[matchInfoModel.activeChallengeId];

            // Initialize player timer and adjust for joining the game late
            bool isPlayerTurn = chessboard.isPlayerTurn;
            long gameStartTime = matchInfoModel.activeMatch.gameStartTimeMilliseconds;
            long elapsedTimeSinceGameStart = backendService.serverClock.currentTimestamp - gameStartTime;
            TimeSpan playerTimer = chessboard.backendPlayerTimer;
            TimeSpan opponentTimer = chessboard.backendOpponentTimer;

            if (isPlayerTurn)
            {
                playerTimer -= TimeSpan.FromMilliseconds(elapsedTimeSinceGameStart);
            }
            else
            {
                opponentTimer -= TimeSpan.FromMilliseconds(elapsedTimeSinceGameStart);
            }

            // Update the view for starting values
            InitTimerVO vo;
            vo.startingTimer = chessboard.gameDuration;
            vo.playerTimer = playerTimer;
            vo.opponentTimer = opponentTimer;
            vo.isPlayerTurn = isPlayerTurn;
            initTimersSignal.Dispatch(vo);

            // Kick off
            timeControl.SetTimers(playerTimer, opponentTimer);
            timeControl.StartTimers(isPlayerTurn);
        }

        private void OnPlayerTimerTick()
        {
            updatePlayerTimerSignal.Dispatch(timeControl.playerDisplayTimer);
        }

        private void OnOpponentTimerTick()
        {
            updateOpponentTimerSignal.Dispatch(timeControl.opponentDisplayTimer);
        }

        private void OnTakeTurnSwapTimeControl()
        {
            AdjustTimersOutgoing();
            timeControl.SwapTimers();
        }

        private void OnReceiveTurnSwapTimeControl()
        {
            AdjustTimersIncoming();
            timeControl.SwapTimers();
        }

        private void AdjustTimersOutgoing()
        {
            timeControl.playerRealTimer -= TimeSpan.FromMilliseconds(backendService.serverClock.latency);
        }

        private void AdjustTimersIncoming()
        {
            timeControl.playerRealTimer = chessboard.backendPlayerTimer - TimeSpan.FromMilliseconds(backendService.serverClock.latency);
            timeControl.opponentRealTimer = chessboard.backendOpponentTimer;
        }

        private void OnPlayerTimerExpired()
        {
            playerTimerExpiredSignal.Dispatch();
            RemoveListeners();
            Release();
        }

        private void OnOpponentTimerExpired()
        {
            opponentTimerExpiredSignal.Dispatch();
            RemoveListeners();
            Release();
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
