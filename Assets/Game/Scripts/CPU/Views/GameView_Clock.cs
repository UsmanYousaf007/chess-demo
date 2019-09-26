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

namespace TurboLabz.CPU
{
    public partial class GameView
    {
        public Text playerClockLabel;
        public Image playerClockFill;
        public Image playerClockImage;
        public Image playerClockBg;
        public GameObject playerInfinityImage;
        public Text opponentClockLabel;
        public Image opponentClockFill;
        public Image opponentClockImage;
        public Image opponentClockBg;
        public GameObject opponentInfinityImage;

        private Coroutine playerClockCR;
        private Coroutine opponentClockCR;
        private TimeSpan startingTimer;
        private TimeSpan playerTimer;
        private TimeSpan opponentTimer;
        private float clockSpeed;

        private readonly Color imageDisabledColor = new Color(0.49f, 0.49f, 0.49f);
        private const double clockEmergencyThresholdSeconds = 10;

        public void InitClock()
        {
            EmptyClock();
        }

        public void OnParentShowClock()
        {
            // Do nothing
        }

        public void CleanupClock()
        {
            // Do nothing
        }

        public void InitInfiniteTimers(bool isPlayerTurn)
        {
            startingTimer = TimeSpan.Zero;
//            playerInfinityImage.SetActive(true);
 //           opponentInfinityImage.SetActive(true);
            playerClockFill.fillAmount = 1f;
            opponentClockFill.fillAmount = 1f;

            if (isPlayerTurn)
            {
                EnablePlayerTimer();
                DisableOpponentTimer();
            }
            else
            {
                DisablePlayerTimer();
                EnableOpponentTimer();
            }

            playerClockLabel.text = localizationService.Get(LocalizationKey.CPU_GAME_TURN_PLAYER);
            opponentClockLabel.text = localizationService.Get(LocalizationKey.CPU_GAME_TURN_OPPONENT);
        }

        public void InitTimers(InitTimerVO vo)
        {
            startingTimer = vo.startingTimer;
            playerClockFill.gameObject.SetActive(true);
            opponentClockFill.gameObject.SetActive(true);
            playerClockLabel.gameObject.SetActive(true);
            opponentClockLabel.gameObject.SetActive(true);
            playerInfinityImage.SetActive(false);
            opponentInfinityImage.SetActive(false);

            playerTimer = vo.playerTimer;
            opponentTimer = vo.opponentTimer;
            //playerClockLabel.text = FormatTimer(playerTimer);
            //opponentClockLabel.text = FormatTimer(opponentTimer);

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
            /*
            this.playerTimer = playerTimer;
            playerClockLabel.text = FormatTimer(playerTimer);
            SetPlayerTimerActiveColors();
            StopPlayerClockCR();
            playerClockCR = StartCoroutine(AnimateTimerCR(playerClockFill, playerTimer));
            */
        }

        public void TickOpponentTimer(TimeSpan opponentTimer)
        {
            /*
            this.opponentTimer = opponentTimer;
            opponentClockLabel.text = FormatTimer(opponentTimer);
            SetOpponentTimerActiveColors();
            StopOpponentClockCR();
            opponentClockCR = StartCoroutine(AnimateTimerCR(opponentClockFill, opponentTimer));
            */
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
            if (startingTimer == TimeSpan.Zero)
            {
                playerClockImage.color = Colors.YELLOW;
                playerClockLabel.color = Colors.YELLOW;
                playerClockBg.color = Colors.YELLOW_128;
            }
            else
            {
                SetPlayerTimerActiveColors();
            }
        }

        private void DisablePlayerTimer()
        {
           // playerClockLabel.color = labelDisabledColor;
            playerClockImage.color = Colors.DISABLED_WHITE;
            StopPlayerClockCR();
            playerClockFill.fillAmount = (float)(playerTimer.TotalSeconds / startingTimer.TotalSeconds);

            playerClockLabel.color = Colors.WHITE_150;
            playerClockBg.color = Colors.DISABLED_WHITE;
        }

        private void EnableOpponentTimer()
        {
            if (startingTimer == TimeSpan.Zero)
            {
                opponentClockImage.color = Colors.YELLOW;
                opponentClockLabel.color = Colors.YELLOW;
                opponentClockBg.color = Colors.YELLOW_128;
            }
            else
            {
                SetOpponentTimerActiveColors();
            }
        }

        private void DisableOpponentTimer()
        {
         //   opponentClockLabel.color = labelDisabledColor;
            opponentClockImage.color = Colors.DISABLED_WHITE;
            StopOpponentClockCR();
            opponentClockFill.fillAmount = (float)(opponentTimer.TotalSeconds / startingTimer.TotalSeconds);

            opponentClockLabel.color = Colors.WHITE_150;
            opponentClockBg.color = Colors.DISABLED_WHITE;
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

        // TODO(mubeeniqbal): Use constants in place of all literals. WTH man?!
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

        private string FormatTimer(TimeSpan timer)
        {
            if (timer.TotalMilliseconds > 0)
            {
                timer = TimeSpan.FromMilliseconds(timer.TotalMilliseconds + 999);
            }

            return string.Format("{0:00}:{1:00}", Mathf.FloorToInt((float)timer.TotalMinutes), timer.Seconds);
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
            /*
            playerClockLabel.text = "";
            opponentClockLabel.text = "";
            playerScore.text = "";
            opponentScore.text = "";
            */
        }
    }
}
