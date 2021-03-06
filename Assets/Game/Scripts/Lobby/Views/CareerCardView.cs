/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.TLUtils;
using System;
using TMPro;
using TurboLabz.InstantGame;
using System.Collections;
using DG.Tweening;

namespace TurboLabz.InstantFramework
{
    [CLSCompliant(false)]
    public class CareerCardView : View
    {
        public TMP_Text subTitle;

        public Image nextTitleTrophy;
        public Image nextTitle;
        public Image nextTitleGlow;

        public TMP_Text nextTitleText;

        public TMP_Text trophiesLossLabel;
        public TMP_Text trophiesCountOnLosses;

        public RectTransform trophyProgressionBarFiller;
        public GameObject trophyProgressionBar;
        public TMP_Text playerTrophiesCountLabel;
        private float trophyProgressionBarOriginalWidth;

        public GameObject nextTitleGO;

        public TMP_Text trophiesWinLabel;
        public TMP_Text trophiesCountOnWins;

        public Button infoButton;

        public Image bgImage;

        public Button bettingPlus;
        public Button bettingMinus;
        public Text bettingValue;
        public TMP_Text rewardValue;
        public GameObject lowBetTooltip;
        public Text tooltipText;
        public Button playBtn;
        public GameObject maxBetTooltip;

        //Models
        [Inject] public ILeaguesModel leaguesModel { get; set; }
        [Inject] public ITournamentsModel tournamentsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }

        //Signals
        public Signal<string, long> playMultiplayerButtonClickedSignal = new Signal<string, long>();
        public Signal OnInfoBtnClickedSignal = new Signal();
        public Signal<long> OnPlayButtonClickedSignal = new Signal<long>();
        public Signal<long> notEnoughCoinsSignal = new Signal<long>();
        public Signal loadRewardsSignal = new Signal();

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IBlurBackgroundService blurBackgroundService { get; set; }

        LeagueTierIconsContainer leagueTierIconsContainer;
        int bettingIndex;
        int minimumBettingIndex;
        bool minimumBetReached;
        bool defaultBetIndexUsed;
        bool maxBetReached;

        public void Init()
        {
            trophyProgressionBarOriginalWidth = trophyProgressionBarFiller.sizeDelta.x;
            leagueTierIconsContainer = leagueTierIconsContainer == null ? LeagueTierIconsContainer.Load() : leagueTierIconsContainer;

            infoButton.onClick.AddListener(OnInfoBtnClicked);
            trophiesLossLabel.text = localizationService.Get(LocalizationKey.LOSS_TEXT);
            trophiesWinLabel.text = localizationService.Get(LocalizationKey.WIN_TEXT);

            bettingPlus.onClick.AddListener(OnIncrementBetting);
            bettingMinus.onClick.AddListener(OnDecrementBetting);

            playBtn.onClick.AddListener(OnPlayButtonClicked);
        }

        public void UpdateView(CareerCardVO vo)
        {
            var currentLeagueInfo = leaguesModel.GetCurrentLeagueInfo();
            trophiesCountOnLosses.text = $"-{currentLeagueInfo.lossTrophies}";
            trophiesCountOnWins.text = currentLeagueInfo.winTrophies.ToString();

            var leagueAssets = tournamentsModel.GetLeagueSprites((playerModel.league + 1).ToString());

            if (leagueAssets != null)
            {
                nextTitleTrophy.sprite = leagueAssets.trophyImg;
                nextTitle.sprite = leagueAssets.nameImg;
                nextTitleGlow.sprite = leagueAssets.glowBg;
                bgImage.sprite = leagueAssets.cardBg;
            }

            SetupTrophyProgressionBar(playerModel.trophies);

            if (vo.coinsStockChanged)
            {
                bettingIndex = vo.betIndex;
                minimumBettingIndex = vo.minimumBetIndex;
                defaultBetIndexUsed = true;
                SetupBetting();
            }

            tooltipText.text = $"Minimum bet is {(int)(settingsModel.defaultBetIncrementByGamesPlayed[0] * 100)}% of your coin stock";
        }

        float barFillPercentage;

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
                    barFillPercentage = (float)currentPoints / requiredPoints;
                    trophyProgressionBarFiller.sizeDelta = new Vector2(trophyProgressionBarOriginalWidth * barFillPercentage, trophyProgressionBarFiller.sizeDelta.y);
                    playerTrophiesCountLabel.text = $"{currentTrophies} / {requiredPoints}";
                }
            }
            else
            {
                playerTrophiesCountLabel.text = currentTrophies.ToString();
                trophyProgressionBar.SetActive(false);
                nextTitleGO.SetActive(false);
            }
        }

        void OnStartGameBtnClicked(string actionCode)
        {
            Debug.Log("OnQuickMatchBtnClicked");
            if (playerModel.coins >= settingsModel.bettingIncrements[bettingIndex])
            {
                playMultiplayerButtonClickedSignal.Dispatch(actionCode, settingsModel.bettingIncrements[bettingIndex]);
            }
        }

        void OnInfoBtnClicked()
        {
            audioService.PlayStandardClick();
            OnInfoBtnClickedSignal.Dispatch();
        }

        void OnIncrementBetting()
        {
            audioService.PlayStandardClick();
            if (!maxBetReached)
            {
                bettingIndex++;
                defaultBetIndexUsed = false;
                SetupBetting();
            }
            else
            {
                maxBetTooltip.SetActive(true);
            }
        }

        void OnDecrementBetting()
        {
            audioService.PlayStandardClick();

            if (minimumBetReached)
            {
                lowBetTooltip.SetActive(true);
                return;
            }

            defaultBetIndexUsed = false;
            bettingIndex--;
            SetupBetting();
        }

        void OnPlayButtonClicked()
        {
            audioService.PlayStandardClick();

            if (playerModel.coins >= settingsModel.bettingIncrements[bettingIndex])
            {
                OnPlayButtonClickedSignal.Dispatch(settingsModel.bettingIncrements[bettingIndex]);
            }
            else
            {
                notEnoughCoinsSignal.Dispatch(settingsModel.bettingIncrements[bettingIndex]);
            }

            var defaultIndexUsed = defaultBetIndexUsed ? "used" : "not_used";
            var gamesPlayedIndex = preferencesModel.gamesPlayedPerDay;
            var lastIndex = settingsModel.defaultBetIncrementByGamesPlayed.Count - 1;
            gamesPlayedIndex = gamesPlayedIndex >= lastIndex ? lastIndex : gamesPlayedIndex;
            analyticsService.Event(AnalyticsEventId.bet_increment_default, AnalyticsParameter.context, $"default_{gamesPlayedIndex + 1}_{defaultIndexUsed}");
        }

        void SetupBetting()
        {
            var lastBettingIndex = bettingIndex >= settingsModel.bettingIncrements.Count - 1;
            bettingIndex = lastBettingIndex ? settingsModel.bettingIncrements.Count - 1 : bettingIndex;
            //bettingPlus.interactable = !lastBettingIndex;
            maxBetReached = lastBettingIndex;
            bettingPlus.image.color = lastBettingIndex ? Colors.DISABLED_BUTTON : Color.white;

            var firstBettingIndex = bettingIndex <= 0;
            bettingIndex = firstBettingIndex ? 0 : bettingIndex;
            bettingMinus.interactable = !firstBettingIndex;

            minimumBetReached = bettingIndex <= minimumBettingIndex;
            bettingMinus.image.color = minimumBetReached ? new Color(1f, 1f, 1f, 128f / 255f) : Color.white;

            bettingValue.text = FormatUtil.AbbreviateNumber(settingsModel.bettingIncrements[bettingIndex], false);

            var reward = settingsModel.bettingIncrements[bettingIndex] * settingsModel.GetSafeCoinsMultiplyer(Settings.ABTest.COINS_TEST_GROUP);
            rewardValue.text = FormatUtil.AbbreviateNumber((long)reward, true);
        }

    }
}
