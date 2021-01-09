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
using System.Collections.Generic;
using System.Text;
using TurboLabz.InstantGame;
using TurboLabz.CPU;
using System.Collections;

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
        public GameObject lowBetTooltip;
        public Text tooltipText;
        public Button playBtn;

        //Models
        [Inject] public ILeaguesModel leaguesModel { get; set; }
        [Inject] public ITournamentsModel tournamentsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }

        //Signals
        public Signal<string, long> playMultiplayerButtonClickedSignal = new Signal<string, long>();
        public Signal OnInfoBtnClickedSignal = new Signal();
        public Signal<long> OnPlayButtonClickedSignal = new Signal<long>();
        public Signal<long> notEnoughCoinsSignal = new Signal<long>();

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        LeagueTierIconsContainer leagueTierIconsContainer;
        int bettingIndex;
        int minimumBettingIndex;
        bool minimumBetReached;
        bool defaultBetIndexUsed;

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
            trophiesCountOnLosses.text = $"-{leaguesModel.leagues[playerModel.league.ToString()].lossTrophies}";
            trophiesCountOnWins.text = leaguesModel.leagues[playerModel.league.ToString()].winTrophies.ToString();

            var leagueAssets = tournamentsModel.GetLeagueSprites((playerModel.league + 1).ToString());
            nextTitleTrophy.sprite = leagueAssets.trophyImg;
            nextTitle.sprite = leagueAssets.nameImg;
            nextTitleGlow.sprite = leagueAssets.glowBg;
            bgImage.sprite = leagueAssets.cardBg;

            SetupTrophyProgressionBar(playerModel.trophies);

            bettingIndex = vo.betIndex;
            minimumBettingIndex = vo.minimumBetIndex;
            defaultBetIndexUsed = true;
            SetupBetting();

            tooltipText.text = $"You cant bet lower than {(int)(settingsModel.defaultBetIncrementByGamesPlayed[0]*100)}%";
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
            bettingIndex++;
            defaultBetIndexUsed = false;
            SetupBetting();
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
            bettingPlus.interactable = !lastBettingIndex;

            var firstBettingIndex = bettingIndex <= 0;
            bettingIndex = firstBettingIndex ? 0 : bettingIndex;
            bettingMinus.interactable = !firstBettingIndex;

            minimumBetReached = bettingIndex <= minimumBettingIndex;
            bettingMinus.image.color = minimumBetReached ? Colors.DISABLED_WHITE : Color.white;

            bettingValue.text = FormatUtil.AbbreviateNumber(settingsModel.bettingIncrements[bettingIndex]);
        }
    }
}
