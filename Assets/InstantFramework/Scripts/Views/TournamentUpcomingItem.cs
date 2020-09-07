/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using System.Collections;
using TurboLabz.TLUtils;
using UnityEngine;
using UnityEngine.UI;


namespace TurboLabz.InstantFramework
{
    public class TournamentUpcomingItem : MonoBehaviour
    {
        [SerializeField] private ChestIconsContainer chestIconsContainer;
        [SerializeField] private TournamentAssetsContainer tournamentAssetsContainer;

        public Image bg;
        public Text startsInLabel;
        public Image tournamentImage;
        public Text countdownTimerText;
        public Text getNotifiedLabel;
        public Button button;

        private long timeLeft;

        public void Init()
        {
            ChestIconsContainer.Load();
            TournamentAssetsContainer.Load();
        }

        public void UpdateItem(LiveTournamentData liveTournamentData, long timeLeft)
        {
            bg.sprite = tournamentAssetsContainer.GetTile(liveTournamentData.type);
            tournamentImage.sprite = tournamentAssetsContainer.GetSticker(liveTournamentData.type);
            tournamentImage.SetNativeSize();
            this.timeLeft = timeLeft;
            var timeLeftText = TimeUtil.FormatTournamentClock(TimeSpan.FromMilliseconds(timeLeft * 1000));
            countdownTimerText.text = timeLeftText;
        }

        public void UpdateTime()
        {
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
