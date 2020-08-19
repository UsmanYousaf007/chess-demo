/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine.UI;
using strange.extensions.mediation.impl;
using UnityEngine;


namespace TurboLabz.InstantFramework
{
    public class LeagueProfileStripView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }


        [Header ("Player Info")]
        public Text playerLeagueTitleLabel;
        public Image playerLeagueThumbnailImage;
        public Text playerTrophiesCountLabel;
        public Text playerRankCountLabel;
        public Image playerRankStatusImage;

        [Header ("Tournament Info")]
        public Text tournamentCountdownTimerLabel;

        [Header("Localization")]
        public Text leagueEndsInLabel;
        public Text tapLabel;
        public Text trophiesLabel;
        public Text rankLabel;

        public void Init()
        {
            leagueEndsInLabel.text = localizationService.Get(LocalizationKey.PLAYER_LEAGUE_PROFILE_STRIP_ENDS_IN);
            tapLabel.text = localizationService.Get(LocalizationKey.PLAYER_LEAGUE_PROFILE_STRIP_TAP);
            trophiesLabel.text = localizationService.Get(LocalizationKey.PLAYER_LEAGUE_PROFILE_STRIP_TROPHIES);
            rankLabel.text = localizationService.Get(LocalizationKey.PLAYER_LEAGUE_PROFILE_STRIP_RANK);
        }

        public void UpdateView(LeagueProfileStripVO vo)
        {
            playerLeagueTitleLabel.text = vo.playerLeagueTitle;
            playerLeagueThumbnailImage = vo.playerLeagueThumbnailImage;
            playerTrophiesCountLabel.text = vo.playerTrophiesCount.ToString();
            playerRankCountLabel.text = vo.playerRankCount.ToString();
            playerRankStatusImage = vo.playerRankStatusImage;
            tournamentCountdownTimerLabel.text = vo.tournamentCountdownTimer;

            playerRankStatusImage.gameObject.SetActive(playerRankStatusImage != null);
        }

        public bool IsVisible()
        {
            return gameObject.activeSelf;
        }

    }
}
