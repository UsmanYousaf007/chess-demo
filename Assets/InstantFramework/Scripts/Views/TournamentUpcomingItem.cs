/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

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

        public Button button;

        public void Init()
        {
            ChestIconsContainer.Load();
            TournamentAssetsContainer.Load();
        }

        public void UpdateItem(LiveTournamentData liveTournamentData, string timeLeftText)
        {
            bg.sprite = tournamentAssetsContainer.GetTile(liveTournamentData.type);
            tournamentImage.sprite = tournamentAssetsContainer.GetSticker(liveTournamentData.type);
            countdownTimerText.text = timeLeftText;
        }
    }
}
