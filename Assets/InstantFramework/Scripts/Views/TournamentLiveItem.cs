/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using System.Collections;
using System.Collections.Generic;
using TurboLabz.TLUtils;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.InstantFramework
{
    public class TournamentLiveItem : MonoBehaviour
    {
        [SerializeField] private ChestIconsContainer chestIconsContainer;
        [SerializeField] private TournamentAssetsContainer tournamentAssetsContainer;

        public Image bg;
        public Image liveImage;
        public Text liveImageText;
        public Image entriesClosedImage;
        public Text headingLabel;
        public Text subHeadingLabel;
        public Image tournamentImage;
        public Image prizeImage;
        public Text countdownTimerText;
        public Image infoBg;
        public Text grandPrizeTrophiesCountText;
        public Text playerRankCountText;

        public Image resultsTournamentImage;
        public Text resultsYourRankLabel;
        public Text resultsYourRankText;

        public Button button;
        public Text buttonText;
        public Image joinButtonImage;

        [HideInInspector]
        public LiveTournamentData openTournamentData = null;
        [HideInInspector]
        public JoinedTournamentData joinedTournamentData = null;

        private long endTimeUTCSeconds;

        public void Init()
        {
            chestIconsContainer = ChestIconsContainer.Load();
            tournamentAssetsContainer = TournamentAssetsContainer.Load();
        }

        public void UpdateItem(LiveTournamentData liveTournamentData)
        {
            EnableResultsGroup(false);
            prizeImage.gameObject.SetActive(true);
            grandPrizeTrophiesCountText.gameObject.SetActive(true);

            openTournamentData = liveTournamentData;
            joinedTournamentData = null;

            bg.sprite = tournamentAssetsContainer.GetTile(liveTournamentData.type);
            tournamentImage.sprite = tournamentAssetsContainer.GetSticker(liveTournamentData.type);
            tournamentImage.SetNativeSize();
            prizeImage.sprite = chestIconsContainer.GetChest(liveTournamentData.grandPrize.chestType);
            playerRankCountText.text = "";
            grandPrizeTrophiesCountText.text = liveTournamentData.grandPrize.trophies.ToString();
            liveImage?.gameObject.SetActive(!liveTournamentData.concluded);
            if (liveImageText != null)
            {
                liveImageText.text = "LIVE";

                if (buttonText != null)
                {
                    buttonText.text = "Join";
                }
            }
            entriesClosedImage?.gameObject.SetActive(liveTournamentData.concluded);
            joinButtonImage?.gameObject.SetActive(!liveTournamentData.concluded);

            if (button != null)
                button.enabled = !liveTournamentData.concluded;

            endTimeUTCSeconds = liveTournamentData.endTimeUTCSeconds - (TournamentConstants.BUFFER_TIME_MINS * 60);

            long timeLeft = endTimeUTCSeconds - DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string timeLeftText;
            if (timeLeft > 0)
            {
                timeLeftText = TimeUtil.FormatTournamentClock(TimeSpan.FromMilliseconds(timeLeft * 1000));
            }
            else
            {
                timeLeftText = "0:00";
            }
            countdownTimerText.text = timeLeftText;

            if (infoBg != null)
            {
                infoBg.color = tournamentAssetsContainer.GetColor(liveTournamentData.type);
            }
        }

        public void UpdateItem(JoinedTournamentData joinedTournamentData)
        {
            openTournamentData = null;
            this.joinedTournamentData = joinedTournamentData;

            bg.sprite = tournamentAssetsContainer.GetTile(joinedTournamentData.type);
            tournamentImage.sprite = tournamentAssetsContainer.GetSticker(joinedTournamentData.type);
            tournamentImage.SetNativeSize();

            if (joinedTournamentData.ended == false)
            {
                prizeImage.sprite = chestIconsContainer.GetChest(joinedTournamentData.grandPrize.chestType);
                grandPrizeTrophiesCountText.text = joinedTournamentData.grandPrize.trophies.ToString();

                prizeImage.gameObject.SetActive(true);
                grandPrizeTrophiesCountText.gameObject.SetActive(true);

                EnableResultsGroup(false);
            }
            else
            {
                prizeImage.gameObject.SetActive(false);
                grandPrizeTrophiesCountText.gameObject.SetActive(false);

                if (resultsTournamentImage != null)
                {
                    resultsTournamentImage.sprite = tournamentImage.sprite;
                }
                if (resultsYourRankText != null)
                {
                    resultsYourRankText.text = joinedTournamentData.rank.ToString();
                }

                EnableResultsGroup(true);
            }

            playerRankCountText.text = joinedTournamentData.rank.ToString();

            liveImage?.gameObject.SetActive(true);
            entriesClosedImage?.gameObject.SetActive(false);
            if (button != null)
            {
                button.enabled = true;
            }

            string timeLeftText;
            if (joinedTournamentData.ended == false)
            {
                if (liveImageText != null)
                {
                    liveImageText.text = "JOINED";
                    if (buttonText != null)
                    {
                        buttonText.text = "Play";
                    }
                }

                endTimeUTCSeconds = joinedTournamentData.endTimeUTCSeconds;

                long timeLeft = endTimeUTCSeconds - DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                if (timeLeft > 0)
                {
                    timeLeftText = TimeUtil.FormatTournamentClock(TimeSpan.FromMilliseconds(timeLeft * 1000));
                }
                else
                {
                    timeLeftText = "0:00";
                }
            }
            else
            {
                endTimeUTCSeconds = 0;
                timeLeftText = "0:00";
            }
            countdownTimerText.text = timeLeftText;

            if (infoBg != null)
            {
                infoBg.color = tournamentAssetsContainer.GetColor(joinedTournamentData.type);
            }
        }

        private void EnableResultsGroup(bool enable)
        {
            resultsTournamentImage?.gameObject.SetActive(enable);
            resultsYourRankText?.gameObject.SetActive(enable);
        }

        public void UpdateTime()
        {
            long timeLeft = endTimeUTCSeconds - DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (timeLeft > 0)
            {
                timeLeft--;
                var timeLeftText = TimeUtil.FormatTournamentClock(TimeSpan.FromMilliseconds(timeLeft * 1000));
                countdownTimerText.text = timeLeftText;
            }
            else
            {
                countdownTimerText.text = "0:00";
            }
        }
    }
}
