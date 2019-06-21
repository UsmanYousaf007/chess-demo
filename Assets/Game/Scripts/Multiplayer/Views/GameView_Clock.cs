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
            TLUtils.LogUtil.Log("VIEW: ExpirePlayerTimer");

            waitingLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_WAITING_FOR_ACCEPT);

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

        public void InitTimers(InitTimerVO vo)
        {
            TLUtils.LogUtil.Log("VIEW: InitTimers");

            startingTimer = vo.startingTimer;
            playerClockFill.gameObject.SetActive(true);
            opponentClockFill.gameObject.SetActive(true);
            playerClockLabel.gameObject.SetActive(true);
            opponentClockLabel.gameObject.SetActive(true);
            waitingLabel.gameObject.SetActive(false);

            playerTimer = vo.playerTimer;
            opponentTimer = vo.opponentTimer;
            playerClockLabel.text = TimeUtil.FormatPlayerClock(playerTimer);
            opponentClockLabel.text = TimeUtil.FormatPlayerClock(opponentTimer);

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
            TLUtils.LogUtil.Log("VIEW: PlayerTurnComplete");

            DisablePlayerTimer();
            EnableOpponentTimer();
        }

        public void OpponentTurnComplete()
        {
            TLUtils.LogUtil.Log("VIEW: OpponentTurnComplete");

            EnablePlayerTimer();
            DisableOpponentTimer();
        }

        public void TickPlayerTimer(TimeSpan playerTimer)
        {
            TLUtils.LogUtil.Log("VIEW: TickPlayerTimer");

            this.playerTimer = playerTimer;
            playerClockLabel.text = TimeUtil.FormatPlayerClock(playerTimer);
            SetPlayerTimerActiveColors();
            StopPlayerClockCR();
            playerClockCR = StartCoroutine(AnimateTimerCR(playerClockFill, playerTimer));
        }

        public void TickOpponentTimer(TimeSpan opponentTimer)
        {
            TLUtils.LogUtil.Log("VIEW: TickOpponentTimer");

                        this.opponentTimer = opponentTimer;
            opponentClockLabel.text = TimeUtil.FormatPlayerClock(opponentTimer);
            SetOpponentTimerActiveColors();
            StopOpponentClockCR();
            opponentClockCR = StartCoroutine(AnimateTimerCR(opponentClockFill, opponentTimer));
        }

        public void ExpirePlayerTimer()
        {
            TLUtils.LogUtil.Log("VIEW: ExpirePlayerTimer");
            TickPlayerTimer(TimeSpan.Zero);
            StopPlayerClockCR();
            playerClockFill.fillAmount = 0;
        }

        public void ExpireOpponentTimer()
        {
            TLUtils.LogUtil.Log("VIEW: ExpireOpponentTimer");
            TickOpponentTimer(TimeSpan.Zero);
            StopOpponentClockCR();
            opponentClockFill.fillAmount = 0;
        }

        private void EnablePlayerTimer()
        {
            TLUtils.LogUtil.Log("VIEW: EnablePlayerTimer");

            SetPlayerTimerActiveColors();
        }

        private void DisablePlayerTimer()
        {
            TLUtils.LogUtil.Log("VIEW: DisablePlayerTimer");

            playerClockLabel.color = Colors.WHITE_150;
            playerClockImage.color = Colors.DISABLED_WHITE;
            StopPlayerClockCR();
            playerClockFill.fillAmount = (float)(playerTimer.TotalSeconds / startingTimer.TotalSeconds);
        }

        private void EnableOpponentTimer()
        {
            TLUtils.LogUtil.Log("VIEW: EnableOpponentTimer");

            SetOpponentTimerActiveColors();
        }

        private void SetOpponentTimerWaitingState(bool isPlayerTurn)
        {
            TLUtils.LogUtil.Log("VIEW: SetOpponentTimerWaitingState");

            opponentClockLabel.gameObject.SetActive(false);
            waitingLabel.gameObject.SetActive(true);

            if (isPlayerTurn)
            {
                waitingLabel.color = Colors.DISABLED_WHITE;
            }
        }

        private void DisableOpponentTimer()
        {
            TLUtils.LogUtil.Log("VIEW: DisableOpponentTimer");

            opponentClockLabel.color = Colors.WHITE_150;
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
            TLUtils.LogUtil.Log("VIEW: StopPlayerClockCR");

            if (playerClockCR != null)
            {
                StopCoroutine(playerClockCR);
                playerClockCR = null;
            }
        }

        private void StopOpponentClockCR()
        {
            TLUtils.LogUtil.Log("VIEW: StopOpponentClockCR");

            if (opponentClockCR != null)
            {
                StopCoroutine(opponentClockCR);
                opponentClockCR = null;
            }
        }

        private void EmptyClock()
        {
            TLUtils.LogUtil.Log("VIEW: EmptyClock");

            playerClockLabel.text = "";
            opponentClockLabel.text = "";
            //playerScore.text = "";
            //opponentScore.text = "";
        }
    }
}
