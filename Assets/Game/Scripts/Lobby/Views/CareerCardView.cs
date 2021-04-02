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

        [Header("Career Progression View")]
        public Image careerTxtImg;
        public Image starImg;
        public RectTransform progressionBarFiller;
        public GameObject progressionBar;
        public Text playerStarsCountLabel;
        private float progressionBarOriginalWidth;
        public Image congratulationsImg;
        public TMP_Text careerProgressionText;
        public Button continueBtn;
        public TMP_Text continueBtnText;
        public Image blurredBgImg;
        public ParticleSystem barParticleSystem;
        public Slider progressBarSlider;
        public CanvasGroup canvasGroup;

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
            continueBtn.onClick.AddListener(FadeOutCareerProgressionView);
        }

        public void UpdateView(CareerCardVO vo)
        {
            if(matchInfoModel.lastCompletedMatch != null && matchInfoModel.lastCompletedMatch.winnerId == playerModel.id)
            {
                AnimateCareerProgression(vo);
            }
            else
            {
                loadRewardsSignal.Dispatch();
                PlayerInfo(vo);
            }
        }

        private void PlayerInfo(CareerCardVO vo)
        {
            trophiesCountOnLosses.text = $"-{leaguesModel.leagues[playerModel.league.ToString()].lossTrophies}";
            trophiesCountOnWins.text = leaguesModel.leagues[playerModel.league.ToString()].winTrophies.ToString();

            var leagueAssets = tournamentsModel.GetLeagueSprites((playerModel.league + 1).ToString());
            nextTitleTrophy.sprite = leagueAssets.trophyImg;
            nextTitle.sprite = leagueAssets.nameImg;
            nextTitleGlow.sprite = leagueAssets.glowBg;
            bgImage.sprite = leagueAssets.cardBg;

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
                    var oldPoints = (int)barFillPercentage * requiredPoints;
                    barFillPercentage = (float)currentPoints / requiredPoints;
                    trophyProgressionBarFiller.sizeDelta = new Vector2(trophyProgressionBarOriginalWidth * barFillPercentage, trophyProgressionBarFiller.sizeDelta.y);

                    if (!barParticleSystem.isPlaying)
                    {
                        barParticleSystem.Play();
                    }
                    progressBarSlider.DOValue(barFillPercentage, 1.5f).OnComplete(AnimationComplete);
                    StartCoroutine(CountTo(currentPoints, oldPoints, 1.5f));

                  
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

        void LoadRewards()
        {
            loadRewardsSignal.Dispatch();
            canvasGroup.gameObject.SetActive(false);
            ResetCareerProgressoin();
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

        #region Animations

        private void AnimateCareerProgression(CareerCardVO vo)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(1f);
            sequence.AppendCallback(() => BlurBg());
            sequence.AppendCallback(() => ResetCareerProgressoin());
            sequence.AppendCallback(() => AnimateBlurredBg());
            sequence.AppendInterval(1f);
            sequence.AppendCallback(() => ScaleInCareerProgressionElements());
            sequence.AppendInterval(1.3f);
            sequence.AppendCallback(() => PlayerInfo(vo));
            sequence.AppendInterval(3f);
            sequence.AppendCallback(() => AnimateNextTitle());
            sequence.PlayForward();
        }

        private void BlurBg()
        {
            blurBackgroundService.BlurBackground(blurredBgImg, 6, Colors.BLUR_BG_BRIGHTNESS_NORMAL, canvasGroup.gameObject);
        }    

        private void AnimateBlurredBg()
        {
            blurredBgImg.DOFade(1, 1);
        }

        private void ScaleInCareerProgressionElements()
        {
            starImg.transform.DOScale(1.3f, 1.3f);
            careerTxtImg.transform.DOScale(1.3f, 1.3f);
            playerStarsCountLabel.transform.DOScale(1.3f, 1.3f);
            progressBarSlider.transform.DOScale(1, 1.3f);
        }

        private void AniamteProgressionBar()
        {
            if (!barParticleSystem.isPlaying)
            {
                barParticleSystem.Play();
            }
            SetupTrophyProgressionBar(playerModel.trophies);
        }

        private void AnimationComplete()
        {
            barParticleSystem.Stop();
        }

        private void AnimateNextTitle()
        {
            if(playerModel.leaguePromoted)
            {
                congratulationsImg.DOFade(1, 1.3f);
                careerProgressionText.DOFade(1, 1.3f);
                continueBtn.image.DOFade(1, 1.3f);
                continueBtnText.DOFade(1, 1.3f);
            }else
            {
                FadeOutCareerProgressionView();
            }
        }

        private void FadeOutCareerProgressionView()
        {
            canvasGroup.DOFade(0, 1).OnComplete(LoadRewards);

        }

        private void ResetCareerProgressoin()
        {
            barParticleSystem.Stop();
            progressBarSlider.DOValue(barFillPercentage, 0);
            progressBarSlider.transform.DOScale(0, 0);
            starImg.transform.DOScale(0, 0);
            careerTxtImg.transform.DOScale(0, 0);
            playerStarsCountLabel.transform.DOScale(0, 0);
            congratulationsImg.DOFade(0, 0);
            careerProgressionText.DOFade(0, 0);
            continueBtn.image.DOFade(0, 0);
            continueBtnText.DOFade(0, 0);
            blurredBgImg.DOFade(0, 0);
            canvasGroup.alpha = 1;
        }

        IEnumerator CountTo(int target, int score, float duration)
        {
            int start = score;
            for (float timer = 0; timer < duration; timer += Time.deltaTime)
            {
                float progress = timer / duration;
                score = (int)Mathf.Lerp(start, target, progress);
                playerStarsCountLabel.text = score.ToString();
                yield return null;
            }
            score = target;
        }

        #endregion
    }
}
