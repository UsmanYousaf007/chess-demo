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
        public Text headingLabel;
        public Text subHeadingLabel;
        public Image tournamentImage;
        public Image prizeImage;
        public Text countdownTimerText;

        public Text grandPrizeTrophiesCountText;
        public Text playerRankCountText;

        public Button button;

        [HideInInspector]
        public LiveTournamentData openTournamentData = null;
        [HideInInspector]
        public JoinedTournamentData joinedTournamentData = null;

        private long timeLeft;

        public void Init()
        {
            chestIconsContainer = ChestIconsContainer.Load();
            tournamentAssetsContainer = TournamentAssetsContainer.Load();
        }

        public void UpdateItem(LiveTournamentData liveTournamentData, long timeLeft)
        {
            openTournamentData = liveTournamentData;
            joinedTournamentData = null;

            bg.sprite = tournamentAssetsContainer.GetTile(liveTournamentData.type);
            tournamentImage.sprite = tournamentAssetsContainer.GetSticker(liveTournamentData.type);
            prizeImage.sprite = chestIconsContainer.GetChest(liveTournamentData.grandPrize.chestType);
            playerRankCountText.text = "";
            grandPrizeTrophiesCountText.text = liveTournamentData.grandPrize.trophies.ToString();
            liveImage?.gameObject.SetActive(true);

            this.timeLeft = timeLeft;
            var timeLeftText = TimeUtil.FormatTournamentClock(TimeSpan.FromMilliseconds(timeLeft * 1000));
            countdownTimerText.text = timeLeftText;

            if (timeLeftText.Contains("s") && gameObject.activeInHierarchy)
            {
                StartCoroutine(CountdownTimer());
            }
        }

        public void UpdateItem(JoinedTournamentData joinedTournamentData, long timeLeft)
        {
            openTournamentData = null;
            this.joinedTournamentData = joinedTournamentData;

            bg.sprite = tournamentAssetsContainer.GetTile(joinedTournamentData.type);
            tournamentImage.sprite = tournamentAssetsContainer.GetSticker(joinedTournamentData.type);
            prizeImage.sprite = chestIconsContainer.GetChest(joinedTournamentData.grandPrize.chestType);
            playerRankCountText.text = joinedTournamentData.rank.ToString();
            grandPrizeTrophiesCountText.text = joinedTournamentData.grandPrize.trophies.ToString();
            liveImage?.gameObject.SetActive(false);

            this.timeLeft = timeLeft;
            var timeLeftText = TimeUtil.FormatTournamentClock(TimeSpan.FromMilliseconds(timeLeft * 1000));
            countdownTimerText.text = timeLeftText;

            if (timeLeftText.Contains("s") && gameObject.activeInHierarchy)
            {
                StartCoroutine(CountdownTimer());
            }
        }

        IEnumerator CountdownTimer()
        {
            if (gameObject.activeInHierarchy)
            {
                yield return new WaitForSeconds(1);
                timeLeft--;
                if (timeLeft > 0)
                {
                    var timeLeftText = TimeUtil.FormatTournamentClock(TimeSpan.FromMilliseconds(timeLeft * 1000));
                    countdownTimerText.text = timeLeftText;
                    StartCoroutine(CountdownTimer());
                }
            }

            yield return null;
        }
    }
}
