/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-06 17:48:38 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using TurboLabz.Chess;
using TurboLabz.TLUtils;
using TurboLabz.InstantGame;
using TurboLabz.InstantFramework;
using System.Text;

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        [Header("Clock")]
        public Text playerClockLabel;
        public Image playerClockFill;
        public Image playerClockImage;
        public Text opponentClockLabel;
        public Image opponentClockFill;
        public Image opponentClockImage;
        public Text waitingLabel;

        private Coroutine playerClockCR;
        private Coroutine opponentClockCR;
        private TimeSpan startingTimer;
        private TimeSpan playerTimer;
        private TimeSpan opponentTimer;
        private float clockSpeed;

        private const double clockEmergencyThresholdSeconds = 10;

        public void InitClock()
        {
            waitingLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_WAITING_FOR_ACCEPT);

            EmptyClock();

            TimeSpan t1 = new TimeSpan(2, 0, 0, 0);        // 2d
            TimeSpan t2 = new TimeSpan(1, 23, 59, 59);    // 2d
            TimeSpan t3 = new TimeSpan(1, 23, 0, 0);    // 1d 23h
            TimeSpan t4 = new TimeSpan(1, 22, 59, 59);    // 1d 23h
            TimeSpan t5 = new TimeSpan(1, 0, 59, 59);    // 1d 1h
            TimeSpan t6 = new TimeSpan(1, 0, 0, 59);    // 1d 1h
            TimeSpan t7 = new TimeSpan(1, 0, 0, 0);        // 1d
            TimeSpan t8 = new TimeSpan(23, 59, 59);        // 1d
            TimeSpan t9 = new TimeSpan(1, 1, 0);        // 1h 1m
            TimeSpan t10 = new TimeSpan(1, 0, 1);        // 1h 1m
            TimeSpan t11 = new TimeSpan(1, 0, 0);        // 1h
            TimeSpan t12 = new TimeSpan(0, 59, 59);        // 59:59

            Debug.Log("t1=" + FormatTimer(t1));
            Debug.Log("t2=" + FormatTimer(t2));
            Debug.Log("t3=" + FormatTimer(t3));
            Debug.Log("t4=" + FormatTimer(t4));
            Debug.Log("t5=" + FormatTimer(t5));
            Debug.Log("t6=" + FormatTimer(t6));
            Debug.Log("t7=" + FormatTimer(t7));
            Debug.Log("t8=" + FormatTimer(t8));
            Debug.Log("t9=" + FormatTimer(t9));
            Debug.Log("t10=" + FormatTimer(t10));
            Debug.Log("t11=" + FormatTimer(t11));
            Debug.Log("t12=" + FormatTimer(t12));
        }

        private string FormatTimer(TimeSpan timer)
        {
            // TODO: localize clock

            if (timer.TotalHours > 1)
            {
                int days = timer.Days;
                int hours = timer.Hours;
                int minutes = timer.Minutes;

                // Day calculation
                if (timer.Hours == 23 && (timer.Minutes > 0 || timer.Seconds > 0))
                {
                    days++;
                    return days + "d ";
                }
                // Hours calculation
                if (timer.Minutes > 0 || timer.Seconds > 0)
                {
                    hours++;
                }
                // Minutes calculation
                if (timer.Seconds > 0)
                {
                    minutes++;
                }

                StringBuilder sb = new StringBuilder();
                if (days > 0)
                {
                    sb.Append(days);
                    sb.Append("d");
                }

                if (hours > 0)
                {
                    if (days > 0)
                    {
                        sb.Append(" ");

                    }
                    sb.Append(hours);
                    sb.Append("h");
                }

                if (minutes > 0)
                {
                    if (hours > 0)
                    {
                        sb.Append(" ");

                    }
                    sb.Append(minutes);
                    sb.Append("m");
                }

                return sb.ToString();
            }

            // else show 00:00 format
            // This code is similar to rounding the seconds to a ceiling
            if (timer.TotalMilliseconds > 0)
            {
                timer = TimeSpan.FromMilliseconds(timer.TotalMilliseconds + 999);
            }

            return string.Format("{0:00}:{1:00}", Mathf.FloorToInt((float)timer.TotalMinutes), timer.Seconds);
        }

        public void OnParentShowClock()
        {
            // Do nothing
        }

        public void CleanupClock()
        {
            // Do nothing
        }

        public void InitTimers(InitTimerVO vo)
        {
            startingTimer = vo.startingTimer;
            playerClockFill.gameObject.SetActive(true);
            opponentClockFill.gameObject.SetActive(true);
            playerClockLabel.gameObject.SetActive(true);
            opponentClockLabel.gameObject.SetActive(true);
            waitingLabel.gameObject.SetActive(false);

            playerTimer = vo.playerTimer;
            opponentTimer = vo.opponentTimer;
            playerClockLabel.text = FormatTimer(playerTimer);
            opponentClockLabel.text = FormatTimer(opponentTimer);

            playerClockFill.fillAmount =(float)(playerTimer.TotalSeconds / startingTimer.TotalSeconds);
            opponentClockFill.fillAmount = (float)(opponentTimer.TotalSeconds / startingTimer.TotalSeconds);
            clockSpeed = (float)(1 / startingTimer.TotalSeconds);

            if (vo.isPlayerTurn)
            {
                EnablePlayerTimer();
                DisableOpponentTimer();
            }
            else
            {
                DisablePlayerTimer();
                EnableOpponentTimer();
            }

            if (vo.waitingForOpponentToAccept)
            {
                SetOpponentTimerWaitingState(vo.isPlayerTurn);
            }
        }

        public void PlayerTurnComplete()
        {
            DisablePlayerTimer();
            EnableOpponentTimer();
        }

        public void OpponentTurnComplete()
        {
            EnablePlayerTimer();
            DisableOpponentTimer();
        }

        public void TickPlayerTimer(TimeSpan playerTimer)
        {
            this.playerTimer = playerTimer;
            playerClockLabel.text = FormatTimer(playerTimer);
            SetPlayerTimerActiveColors();
            StopPlayerClockCR();
            playerClockCR = StartCoroutine(AnimateTimerCR(playerClockFill, playerTimer));
        }

        public void TickOpponentTimer(TimeSpan opponentTimer)
        {
            this.opponentTimer = opponentTimer;
            opponentClockLabel.text = FormatTimer(opponentTimer);
            SetOpponentTimerActiveColors();
            StopOpponentClockCR();
            opponentClockCR = StartCoroutine(AnimateTimerCR(opponentClockFill, opponentTimer));
        }

        public void ExpirePlayerTimer()
        {
            TickPlayerTimer(TimeSpan.Zero);
            StopPlayerClockCR();
            playerClockFill.fillAmount = 0;
        }

        public void ExpireOpponentTimer()
        {
            TickOpponentTimer(TimeSpan.Zero);
            StopOpponentClockCR();
            opponentClockFill.fillAmount = 0;
        }

        private void EnablePlayerTimer()
        {
            SetPlayerTimerActiveColors();
        }

        private void DisablePlayerTimer()
        {
            playerClockLabel.color = Colors.DISABLED_WHITE;
            playerClockImage.color = Colors.DISABLED_WHITE;
            StopPlayerClockCR();
            playerClockFill.fillAmount = (float)(playerTimer.TotalSeconds / startingTimer.TotalSeconds);
        }

        private void EnableOpponentTimer()
        {
            SetOpponentTimerActiveColors();
        }

        private void SetOpponentTimerWaitingState(bool isPlayerTurn)
        {
            opponentClockLabel.gameObject.SetActive(false);
            waitingLabel.gameObject.SetActive(true);

            if (isPlayerTurn)
            {
                waitingLabel.color = Colors.DISABLED_WHITE;
            }
        }

        private void DisableOpponentTimer()
        {
            opponentClockLabel.color = Colors.DISABLED_WHITE;
            opponentClockImage.color = Colors.DISABLED_WHITE;
            StopOpponentClockCR();
            opponentClockFill.fillAmount = (float)(opponentTimer.TotalSeconds / startingTimer.TotalSeconds);
        }

        private void SetPlayerTimerActiveColors()
        {
            playerClockLabel.color = GetLabelColor(playerTimer);
            playerClockImage.color = GetClockColor(playerTimer);
        }

        private void SetOpponentTimerActiveColors()
        {
            opponentClockLabel.color = GetLabelColor(opponentTimer);
            opponentClockImage.color = GetClockColor(opponentTimer);
            waitingLabel.color = opponentClockLabel.color;
        }

        private IEnumerator AnimateTimerCR(Image filler, TimeSpan currentTimer)
        {
            float startFill = filler.fillAmount;
            float endFill = (float)(currentTimer.TotalSeconds / startingTimer.TotalSeconds);
            float animStartTime = Time.time;

            while (true)
            {
                yield return null;

                float timeElapsed = Time.time - animStartTime;
                filler.fillAmount = startFill - (timeElapsed * clockSpeed);

                if (filler.fillAmount <= endFill)
                {
                    yield break;
                }
            } 
        }
            
        private Color GetClockColor(TimeSpan currentTimer)
        {
            if (currentTimer.TotalSeconds < clockEmergencyThresholdSeconds)
            {
                return Colors.RED;
            }

            return Colors.YELLOW;
        }

        private Color GetLabelColor(TimeSpan currentTimer)
        {
            if (currentTimer.TotalSeconds < clockEmergencyThresholdSeconds)
            {
                return Colors.RED;
            }

            return Colors.YELLOW;
        }



        private void StopPlayerClockCR()
        {
            if (playerClockCR != null)
            {
                StopCoroutine(playerClockCR);
                playerClockCR = null;
            }
        }

        private void StopOpponentClockCR()
        {
            if (opponentClockCR != null)
            {
                StopCoroutine(opponentClockCR);
                opponentClockCR = null;
            }
        }

        private void EmptyClock()
        {
            playerClockLabel.text = "";
            opponentClockLabel.text = "";
            //playerScore.text = "";
            //opponentScore.text = "";
        }
    }
}
