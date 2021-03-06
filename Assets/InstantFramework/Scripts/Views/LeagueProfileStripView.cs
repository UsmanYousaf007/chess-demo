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
using TMPro;

namespace TurboLabz.InstantFramework
{
    [CLSCompliant (false)]
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

        public RectTransform trophyProgressionBarFiller;
        public GameObject trophyProgressionBar;
        //public Text playerTrophiesCountLabel;
        public TMP_Text playerTrophiesCountLabel;
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
        [Inject] public ILeaguesModel leaguesModel { get; set; }

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
            trophyProgressionBarOriginalWidth = trophyProgressionBarFiller.sizeDelta.x;
        }

        public void UpdateView(LeagueProfileStripVO vo)
        {
            playerLeagueThumbnailImage = vo.playerLeagueThumbnailImage;
            playerRankCountLabel.text = vo.playerRankCount.ToString();
            playerRankStatusImage = vo.playerRankStatusImage;
            tournamentCountdownTimerLabel.text = vo.tournamentCountdownTimer;
            gameObjectPlayerRank.SetActive(vo.playerRankStatusImage != null);
            SetupTrophyProgressionBar(vo.playerTrophiesCount);

            SetupLeague(vo.playerLeagueID);
        }

        private void SetupLeague(string leageueId)
        {
            LeagueTierIconsContainer.LeagueAsset leagueAssets = tournamentsModel.GetLeagueSprites(leageueId);
            playerLeagueBG.sprite = leagueAssets.bgSprite;
            playerLeagueChest.sprite = leagueAssets.chestSprite;
            //playerLeagueProfilePicBorder.sprite = leagueAssets.ringSprite;
            //playerLeagueProfilePicBorder.SetNativeSize();
            playerLeagueTitleLabel.text = leagueAssets.typeName;
            playerLeagueTitleUnderlayImage.sprite = leagueAssets.textUnderlaySprite;
        }

        private void SetupTrophyProgressionBar(int currentTrophies)
        {
            int league = playerModel.league;
            if (league < (leaguesModel.leagues.Count - 1))
            {
                league = playerModel.league + 1;
                var currentPoints = currentTrophies;
                leaguesModel.leagues.TryGetValue((league).ToString(), out League value);
                if (value != null)
                {
                    var requiredPoints = value.qualifyTrophies;
                    var barFillPercentage = (float)currentPoints / requiredPoints;
                    trophyProgressionBarFiller.sizeDelta = new Vector2(trophyProgressionBarOriginalWidth * barFillPercentage, trophyProgressionBarFiller.sizeDelta.y);
                    playerTrophiesCountLabel.text = $"{currentTrophies} / {requiredPoints}";
                }
            }
            else
            {
                playerTrophiesCountLabel.text = currentTrophies.ToString();
                trophyProgressionBar.SetActive(false);
                nextLeagueText.gameObject.SetActive(false);
            }
        }

        public void UpdateLeague(int league)
        {
            SetupLeague(league.ToString());
        }

        public void UpdateTrophies (int trophies)
        {
            SetupTrophyProgressionBar(trophies);
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
            audioService.PlayStandardClick();
            leagueProfileClickedSignal.Dispatch();
        }
       
    }
}
