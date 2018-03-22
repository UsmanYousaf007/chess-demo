/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-24 11:33:35 UTC+05:00
/// 
/// @description
/// [add_description_here]
/// 
using System.Collections.Generic;

using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using TurboLabz.Common;
using strange.extensions.mediation.impl;

namespace TurboLabz.Gamebet
{
    public partial class RoomsView : View
    {
        private IEnumerator RoomCountdownCR(TimeSpan timeRemaining, Text timeLabel, RoomInfo roomInfo, RoomCard card)
        {
            bool isRunning = true;

            while (isRunning)
            {
                if (timeRemaining.TotalDays >= 1)
                {
                    timeLabel.text = localizationService.Get(LocalizationKey.RC_REMAINING_DAYS_HOURS_LABEL, timeRemaining.Days, timeRemaining.Hours);
                }
                else if (timeRemaining.TotalDays <= 1 && timeRemaining.TotalHours >= 1)
                {
                    timeLabel.text = localizationService.Get(LocalizationKey.RC_REMAINING_HOURS_MINUTES_LABEL, timeRemaining.Hours, timeRemaining.Minutes);
                }
                else if (timeRemaining.TotalHours <= 1 && timeRemaining.TotalMinutes >= 1)
                {
                    timeLabel.text = localizationService.Get(LocalizationKey.RC_REMAINING_MINUTES_SECONDS_LABEL, timeRemaining.Minutes, timeRemaining.Seconds);
                }
                else if (timeRemaining.TotalMinutes <= 1 && timeRemaining.TotalSeconds >= 0)
                {
                    timeLabel.text = localizationService.Get(LocalizationKey.RC_REMAINING_SECONDS_LABEL, timeRemaining.Seconds);
                }
                else if (timeRemaining.TotalSeconds <= 0)
                {
                    timeLabel.text = localizationService.Get(LocalizationKey.RC_TIME_UP_LABEL);

                    roomInfo.roomCountdownCR = null;
                    isRunning = false;

                    card.rotatingRoomInfoCard.card.SetActive(false);
                    card.rotatingRoomCard.roomCard.SetActive(false);
                    card.rotatingRoomCard.nextRoomCountdown.SetActive(true);

                    roomInfo.nextRoomCountdownCR = StartCoroutine(NextRoomCountdownCR(roomInfo.timeBuffer, card.rotatingRoomCard.nextRoomCountdownText, roomInfo, card));
                 }

                yield return new WaitForSeconds(1);
                timeRemaining = timeRemaining.Subtract(TimeSpan.FromSeconds(1));
            } 
        }

        private IEnumerator NextRoomCountdownCR(TimeSpan timeBuffer, Text timeLabel, RoomInfo roomInfo, RoomCard card)
        {
            bool isRunning = true;

            while (isRunning)
            {
                if (timeBuffer.TotalDays >= 1)
                {
                    timeLabel.text = localizationService.Get(LocalizationKey.RC_REMAINING_DAYS_HOURS_LABEL, timeBuffer.Days, timeBuffer.Hours);
                }
                else if (timeBuffer.TotalDays <= 1 && timeBuffer.TotalHours >= 1)
                {
                    timeLabel.text = localizationService.Get(LocalizationKey.RC_REMAINING_HOURS_MINUTES_LABEL, timeBuffer.Hours, timeBuffer.Minutes);
                }
                else if (timeBuffer.TotalHours <= 1 && timeBuffer.TotalMinutes >= 1)
                {
                    timeLabel.text = localizationService.Get(LocalizationKey.RC_REMAINING_MINUTES_SECONDS_LABEL, timeBuffer.Minutes, timeBuffer.Seconds);
                }
                else if (timeBuffer.TotalMinutes <= 1 && timeBuffer.TotalSeconds >= 0)
                {
                    timeLabel.text = localizationService.Get(LocalizationKey.RC_REMAINING_SECONDS_LABEL, timeBuffer.Seconds);
                }
                else if (timeBuffer.TotalSeconds <= 0)
                {
                    timeLabel.text = localizationService.Get(LocalizationKey.RC_TIME_UP_LABEL);

                    roomInfo.nextRoomCountdownCR = null;
                    isRunning = false;

                    rotatingRoomTimeCompleteSignal.Dispatch();
                }

                yield return new WaitForSeconds(1);
                timeBuffer = timeBuffer.Subtract(TimeSpan.FromSeconds(1));
            } 
        }

        private void StopRoomCountdownClockCR(RoomsVO vo)
        {
            foreach (KeyValuePair<string, RoomInfo> roomInfo in vo.rooms)
            {
                if (roomInfo.Value.roomCountdownCR != null)
                {
                    StopCoroutine(roomInfo.Value.roomCountdownCR);
                    roomInfo.Value.roomCountdownCR = null;
                }
            }
        }

        private void StopNewRoomCountdownClockCR(RoomsVO vo)
        {
            foreach (KeyValuePair<string, RoomInfo> roomInfo in vo.rooms)
            {
                if (roomInfo.Value.nextRoomCountdownCR != null)
                {
                    StopCoroutine(roomInfo.Value.nextRoomCountdownCR);
                    roomInfo.Value.nextRoomCountdownCR = null;
                }
            }
        }
    }
}
