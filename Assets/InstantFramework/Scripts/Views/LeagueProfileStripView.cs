/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine.UI;
using strange.extensions.mediation.impl;
using UnityEngine;
using strange.extensions.signal.impl;
using System;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    public class LeagueProfileStripView : View
    {
        [Header("Player Info")]
        public Text playerLeagueTitleLabel;
        public Image playerLeagueChest;
        public Image playerLeagueBG;
        public Image playerLeagueProfilePicBorder;
        public Image playerLeagueTitleUnderlayImage;

        public Image playerLeagueThumbnailImage;
        public Text playerRankCountLabel;
        public Image playerRankStatusImage;

        public RectTransform trophyProgressionBar;
        public Text playerTrophiesCountLabel;
        private float trophyProgressionBarOriginalWidth;
        public Text yourLeagueText;
        public Text nextLeagueText;

        [Header("Tournament Info")]
        public Text tournamentCountdownTimerLabel;

        [Header("Localization")]
        public Text leagueEndsInLabel;
        public Text tapLabel;
        public Text trophiesLabel;
        public Text rankLabel;

        [Header("Strip")]
        public Button stripButton;

        private Signal stripClickedSignal;
        private GameObject gameObjectPlayerRank;

        //Models
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }// Models
        [Inject] public ITournamentsModel tournamentsModel { get; set; }

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IBackendService backendService { get; set; }

        //Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        public Signal leagueProfileClickedSignal = new Signal();


        public void Init()
        {
            leagueEndsInLabel.text = localizationService.Get(LocalizationKey.PLAYER_LEAGUE_PROFILE_STRIP_ENDS_IN);
            tapLabel.text = localizationService.Get(LocalizationKey.PLAYER_LEAGUE_PROFILE_STRIP_TAP);
            trophiesLabel.text = localizationService.Get(LocalizationKey.PLAYER_LEAGUE_PROFILE_STRIP_TROPHIES);
            rankLabel.text = localizationService.Get(LocalizationKey.PLAYER_LEAGUE_PROFILE_STRIP_RANK);
            yourLeagueText.text = localizationService.Get(LocalizationKey.PLAYER_LEAGUE_PROFILE_STRIP_YOUR_LEAGUE_TEXT);

            stripButton.onClick.AddListener(OnLeagueProfileButtonClicked);

            gameObjectPlayerRank = playerRankStatusImage.gameObject;
            trophyProgressionBarOriginalWidth = trophyProgressionBar.sizeDelta.x;
        }

        public void UpdateView(LeagueProfileStripVO vo)
        {
            playerLeagueThumbnailImage = vo.playerLeagueThumbnailImage;
            playerTrophiesCountLabel.text = vo.playerTrophiesCount.ToString();
            playerRankCountLabel.text = vo.playerRankCount.ToString();
            playerRankStatusImage = vo.playerRankStatusImage;
            tournamentCountdownTimerLabel.text = vo.tournamentCountdownTimer;
            gameObjectPlayerRank.SetActive(vo.playerRankStatusImage != null);
            SetupTrophyProgressionBar(vo.playerRankCount);

            LeagueTierIconsContainer.LeagueAsset leagueAssets = tournamentsModel.GetLeagueSprites(vo.playerLeagueTitle);
            playerLeagueBG.sprite = leagueAssets.bgSprite;
            playerLeagueChest.sprite = leagueAssets.chestSprite;
            playerLeagueProfilePicBorder.sprite = leagueAssets.ringSprite;
            playerLeagueTitleLabel.text = leagueAssets.typeName;
            playerLeagueTitleUnderlayImage.sprite = leagueAssets.textUnderlaySprite;
        }

        private void SetupTrophyProgressionBar(int currentTrophies)
        {
            //var currentPoints = currentTrophies;
            //var requiredTrophies = get required trophies to advance to the next league;
            //var barFillPercentage = (float)currentTrophies / requiredTrophies;
            //trophyProgressionBar.sizeDelta = new Vector2(trophyProgressionBarOriginalWidth * barFillPercentage, trophyProgressionBar.sizeDelta.y);
            //playerTrophiesCountLabel.text = $"{currentTrophies}/{requiredTrophies}";
        }

        public bool IsVisible()
        {
            return gameObject.activeSelf;
        }

        public void SetStripClickedSignal(Signal signal)
        {
            stripClickedSignal = signal;
        }

        public void OnStripButtonClicked()
        {
            TLUtils.LogUtil.Log("LeagueProfileStripView::OnStripButtonClicked()");
            if (stripClickedSignal != null)
            {
                stripClickedSignal.Dispatch();
            }
        }

        public void OnLeagueProfileButtonClicked()
        {
            leagueProfileClickedSignal.Dispatch();
        }
       
    }
}
