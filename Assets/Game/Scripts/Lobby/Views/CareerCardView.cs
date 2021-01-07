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

        public Button playBtn;

        //Models
        [Inject] public ILeaguesModel leaguesModel { get; set; }
        [Inject] public ITournamentsModel tournamentsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }

        //Signals
        public Signal<string, long> playMultiplayerButtonClickedSignal = new Signal<string, long>();
        public Signal OnInfoBtnClickedSignal = new Signal();
        public Signal<long> OnPlayButtonClickedSignal = new Signal<long>();
        public Signal<long> notEnoughCoinsSignal = new Signal<long>();

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }

        LeagueTierIconsContainer leagueTierIconsContainer;
        int bettingIndex;

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

        public void UpdateView(int bettingIndex)
        {
            trophiesCountOnLosses.text = $"-{leaguesModel.leagues[playerModel.league.ToString()].lossTrophies}";
            trophiesCountOnWins.text = leaguesModel.leagues[playerModel.league.ToString()].winTrophies.ToString();

            var leagueAssets = tournamentsModel.GetLeagueSprites((playerModel.league + 1).ToString());
            nextTitleTrophy.sprite = leagueAssets.trophyImg;
            nextTitle.sprite = leagueAssets.nameImg;
            nextTitleGlow.sprite = leagueAssets.glowBg;
            bgImage.sprite = leagueAssets.cardBg;

            SetupTrophyProgressionBar(playerModel.trophies);

            this.bettingIndex = bettingIndex;
            SetupBetting();
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
            OnInfoBtnClickedSignal.Dispatch();
        }

        void OnIncrementBetting()
        {
            bettingIndex++;
            SetupBetting();
        }

        void OnDecrementBetting()
        {
            bettingIndex--;
            SetupBetting();
        }

        void OnPlayButtonClicked()
        {
            if (playerModel.coins >= settingsModel.bettingIncrements[bettingIndex])
            {
                OnPlayButtonClickedSignal.Dispatch(settingsModel.bettingIncrements[bettingIndex]);
            }
            else
            {
                notEnoughCoinsSignal.Dispatch(settingsModel.bettingIncrements[bettingIndex]);
            }
        }

        void SetupBetting()
        {
            var lastBettingIndex = bettingIndex >= settingsModel.bettingIncrements.Count - 1;
            bettingIndex = lastBettingIndex ? settingsModel.bettingIncrements.Count - 1 : bettingIndex;
            bettingPlus.interactable = !lastBettingIndex;

            var firstBettingIndex = bettingIndex <= 0;
            bettingIndex = firstBettingIndex ? 0 : bettingIndex;
            bettingMinus.interactable = !firstBettingIndex;

            bettingValue.text = FormatUtil.AbbreviateNumber(settingsModel.bettingIncrements[bettingIndex]);
        }
    }
}
