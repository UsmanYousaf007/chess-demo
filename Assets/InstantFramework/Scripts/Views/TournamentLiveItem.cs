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
        public Image infoBg;
        public Text grandPrizeTrophiesCountText;
        public Text playerRankCountText;

        public Button button;

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
            openTournamentData = liveTournamentData;
            joinedTournamentData = null;

            bg.sprite = tournamentAssetsContainer.GetTile(liveTournamentData.type);
            tournamentImage.sprite = tournamentAssetsContainer.GetSticker(liveTournamentData.type);
            tournamentImage.SetNativeSize();
            prizeImage.sprite = chestIconsContainer.GetChest(liveTournamentData.grandPrize.chestType);
            playerRankCountText.text = "";
            grandPrizeTrophiesCountText.text = liveTournamentData.grandPrize.trophies.ToString();
            liveImage?.gameObject.SetActive(true);

            endTimeUTCSeconds = liveTournamentData.concludeTimeUTCSeconds;

            long timeLeft = endTimeUTCSeconds - DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var timeLeftText = TimeUtil.FormatTournamentClock(TimeSpan.FromMilliseconds(timeLeft * 1000));
            countdownTimerText.text = timeLeftText;

            if(infoBg != null)
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
            prizeImage.sprite = chestIconsContainer.GetChest(joinedTournamentData.grandPrize.chestType);
            playerRankCountText.text = joinedTournamentData.rank.ToString();
            grandPrizeTrophiesCountText.text = joinedTournamentData.grandPrize.trophies.ToString();
            liveImage?.gameObject.SetActive(false);

            endTimeUTCSeconds = joinedTournamentData.concludeTimeUTCSeconds;

            long timeLeft = endTimeUTCSeconds - DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var timeLeftText = TimeUtil.FormatTournamentClock(TimeSpan.FromMilliseconds(timeLeft * 1000));
            countdownTimerText.text = timeLeftText;

            if (infoBg != null)
            {
                infoBg.color = tournamentAssetsContainer.GetColor(joinedTournamentData.type);
            }
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
        }
    }
}
