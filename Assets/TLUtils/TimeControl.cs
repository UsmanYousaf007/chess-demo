/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-19 19:28:01 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System;
using System.Collections;

using strange.extensions.signal.impl;

namespace TurboLabz.TLUtils
{
    // TODO: Refactor class
    public class TimeControl : ITimeControl
    {
        private enum State
        {
            UNINITIALIZED = -1,
            STOPPED,
            PLAYER_TIMER_RUNNING,
            OPPONENT_TIMER_RUNNING
        }

        // Utils
        private IRoutineRunner routineRunner = new NormalRoutineRunner();

        public TimeSpan playerRealTimer { get; set; }
        public TimeSpan opponentRealTimer { get; set; }

        private DateTime swapTimestamp;
        private TimeSpan timerAtSwap;

        private IEnumerator runPlayerTimerCR;
        private IEnumerator runOpponentTimerCR;

        private State state = State.UNINITIALIZED;
        private bool isPlayerTurn;
        private int lastPlayerTimerSeconds;
        private int lastOpponentTimerSeconds;

        private bool isPaused;
        private DateTime pauseTime;

        public Signal playerTickSignal { get; private set; }
        public Signal opponentTickSignal { get; private set; }
        public Signal playerTimerExpiredSignal { get; private set; }
        public Signal opponentTimerExpiredSignal { get; private set; }

        public TimeControl()
        {
            playerTickSignal = new Signal();
            opponentTickSignal = new Signal();
            playerTimerExpiredSignal = new Signal();
            opponentTimerExpiredSignal = new Signal();
        }

        public void SetTimers(TimeSpan playerTimer, TimeSpan opponentTimer)
        {
            state = State.STOPPED;

            playerRealTimer = playerTimer;
            opponentRealTimer = opponentTimer;
        }

        public void Reset()
        {
            state = State.UNINITIALIZED;

            playerRealTimer = default(TimeSpan);
            opponentRealTimer = default(TimeSpan);
        }

        public void StartTimers(bool isPlayerTurn)
        {
            Assertions.Assert(state != State.UNINITIALIZED, "Timers must be set first!");

            this.isPlayerTurn = isPlayerTurn;
            SwapTimers();
        }

        public void StopTimers()
        {
            Assertions.Assert(state != State.UNINITIALIZED, "Timers must be set first!");

            state = State.STOPPED;

            StopPlayerTimer();
            StopOpponentTimer();
        }

        public void SwapTimers()
        {
            Assertions.Assert(state != State.UNINITIALIZED, "Timers must be set first!");

            if (isPlayerTurn)
            {
                StopOpponentTimer();
                StartPlayerTimer();
            }
            else
            {
                StopPlayerTimer();
                StartOpponentTimer();
            }

            isPlayerTurn = !isPlayerTurn;
        }

        public void PauseTimers()
        {
            if (!isPaused)
            {
                pauseTime = DateTime.UtcNow;
                isPaused = true;
            }
        }

        public void ResumeTimers()
        {
            if (isPaused)
            {
                swapTimestamp = swapTimestamp.Add(DateTime.UtcNow.Subtract(pauseTime));
                isPaused = false;
            }
        }

        private void StartPlayerTimer()
        {
            Assertions.Assert(state != State.PLAYER_TIMER_RUNNING, "Player timer must NOT already be running!");

            state = State.PLAYER_TIMER_RUNNING;

            swapTimestamp = DateTime.UtcNow;
            timerAtSwap = playerRealTimer;
            runPlayerTimerCR = RunPlayerTimerCR();
            routineRunner.StartCoroutine(runPlayerTimerCR);
        }

        private IEnumerator RunPlayerTimerCR()
        {
            while (true)
            {
                if (!isPaused)
                {
                    playerRealTimer = timerAtSwap - (DateTime.UtcNow - swapTimestamp);

                    if (playerRealTimer < TimeSpan.Zero)
                    {
                        playerRealTimer = TimeSpan.Zero;
                    }

                    playerTickSignal.Dispatch();

                    if (playerRealTimer == TimeSpan.Zero)
                    {
                        playerTimerExpiredSignal.Dispatch();
                        StopTimers();
                    }
                }

                yield return null;
            }
        }

        private void StopPlayerTimer()
        {
            if (runPlayerTimerCR != null)
            {
                routineRunner.StopCoroutine(runPlayerTimerCR);
                runPlayerTimerCR = null;
            }
        }

        private void StartOpponentTimer()
        {
            Assertions.Assert(state != State.OPPONENT_TIMER_RUNNING, "Opponent timer must NOT already be running!");

            state = State.OPPONENT_TIMER_RUNNING;

            swapTimestamp = DateTime.UtcNow;
            timerAtSwap = opponentRealTimer;
            runOpponentTimerCR = RunOpponentTimerCR();
            routineRunner.StartCoroutine(runOpponentTimerCR);
        }

        private IEnumerator RunOpponentTimerCR()
        {
            while (true)
            {
                if (!isPaused)
                {
                    opponentRealTimer = timerAtSwap - (DateTime.UtcNow - swapTimestamp);

                    if (opponentRealTimer < TimeSpan.Zero)
                    {
                        opponentRealTimer = TimeSpan.Zero;
                    }

                    opponentTickSignal.Dispatch();

                    if (opponentRealTimer == TimeSpan.Zero)
                    {
                        opponentTimerExpiredSignal.Dispatch();
                        StopTimers();
                    }
                }

                yield return null;
            }
        }

        private void StopOpponentTimer()
        {
            if (runOpponentTimerCR != null)
            {
                routineRunner.StopCoroutine(runOpponentTimerCR);
                runOpponentTimerCR = null;
            }
        }
    }
}
