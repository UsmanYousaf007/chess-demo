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
    public class CareerProgressionView : View
    {
        public GameObject careerTitleRef;
        public GameObject careerStarRef;
        public GameObject careerBarRef;
        public GameObject careerStarCountRef;

        public Image careerTxtImg;
        public Image starImg;
        public RectTransform progressionBarFiller;
        public GameObject progressionBar;
        public Text playerStarsCountLabel;
        private float progressionBarOriginalWidth;
        public Image congratulationsImg;
        public TMP_Text careerProgressionText;
        public Button continueBtn;
        public Text continueBtnText;
        public Image blurredBgImg;
        public ParticleSystem barParticleSystem;
        public Slider progressBarSlider;
        public CanvasGroup canvasGroup;
        public CanvasGroup careerSectionGroup;

        private bool isCareerProgressionShown;
        private float barFillPercentage;        

        //Models
        [Inject] public ILeaguesModel leaguesModel { get; set; }
        [Inject] public ITournamentsModel tournamentsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }

        //Signals
        public Signal loadLobby = new Signal();
        public Signal updateTrophyBarSignal = new Signal();

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        LeagueTierIconsContainer leagueTierIconsContainer;

        public void Init()
        {
            leagueTierIconsContainer = leagueTierIconsContainer == null ? LeagueTierIconsContainer.Load() : leagueTierIconsContainer;
            continueBtn.onClick.AddListener(FadeOutCareerProgressionView);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void ShowCareerProgression()
        {
            careerTxtImg.transform.position = careerTitleRef.transform.position;
            starImg.transform.position = careerStarRef.transform.position;
            progressBarSlider.transform.position = careerBarRef.transform.position;
            playerStarsCountLabel.transform.position = careerStarCountRef.transform.position;

            playerStarsCountLabel.text = playerModel.trophiesPrev.ToString();
            SetBarFilledPercentage();

            if (!isCareerProgressionShown && (playerModel.league < (leaguesModel.leagues.Count - 1)) &&
                matchInfoModel.lastCompletedMatch != null && matchInfoModel.lastCompletedMatch.winnerId == playerModel.id &&
                matchInfoModel.lastCompletedMatch.isRanked)
            {
                AnimateCareerProgression();
            }
            else
            {
                LoadLobby();
            }
        }

        private void SetupTrophyProgressionBar()
        {
            int league = playerModel.league;
            int currentTrophies = playerModel.trophies;

            league = playerModel.league + 1;
            var updatedPointsCount = currentTrophies;
            leaguesModel.leagues.TryGetValue((league).ToString(), out League value);

            if (value != null)
            {
                var requiredPoints = value.qualifyTrophies;
                barFillPercentage = (float)updatedPointsCount / requiredPoints;
                float barFilledPercentage = (float)playerModel.trophiesPrev / requiredPoints;

                if (!barParticleSystem.isPlaying)
                {
                    barParticleSystem.Play();
                }

                progressBarSlider.DOValue(barFilledPercentage, 0);
                progressBarSlider.DOValue(barFillPercentage, 1.5f).OnComplete(AnimationComplete);
                StartCoroutine(CountTo(playerModel.trophies, playerModel.trophiesPrev, 1.5f));
            }
        }

        private void SetBarFilledPercentage()
        {
            int league = playerModel.league;
            int currentTrophies = playerModel.trophiesPrev;

            league = playerModel.league + 1;
            var updatedPointsCount = currentTrophies;
            leaguesModel.leagues.TryGetValue((league).ToString(), out League value);
            if (value != null)
            {
                var requiredPoints = value.qualifyTrophies;
                barFillPercentage = (float)updatedPointsCount / requiredPoints;
            }
        }

        void LoadLobby()
        {
            loadLobby.Dispatch();
        }

        #region Animations

        private void AnimateCareerProgression()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(0.25f);
            sequence.AppendCallback(() => Reset());
            sequence.AppendCallback(() => FadeInCareerProgressionView());
            sequence.AppendCallback(() => BlurBg());
            sequence.AppendCallback(() => ScaleInCareerProgressionElements());
            sequence.AppendInterval(1f);
            sequence.AppendCallback(() => AniamteProgressionBar());
            sequence.AppendInterval(3.5f);
            sequence.AppendCallback(() => AnimateNextTitle());
            sequence.PlayForward();
        }

        private void BlurBg()
        {
            UIBlurBackground.BlurBackground(blurredBgImg, 3, Colors.BLUR_BG_BRIGHTNESS_NORMAL, canvasGroup.gameObject);
            blurredBgImg.DOFade(1.0f, 0);
            UIBlurBackground.AnimateBrightness(Colors.BLUR_BG_BRIGHTNESS_NORMAL, 1.0f, 0.25f);
        }

        private void AnimateBlurredBg()
        {
            blurredBgImg.DOFade(1, 1);
        }

        private void ScaleInCareerProgressionElements()
        {
            careerSectionGroup.transform.DOScale(1.3f, 0.8f).SetEase(Ease.InSine);
        }

        private void AniamteProgressionBar()
        {
            SetupTrophyProgressionBar();
        }

        private void AnimationComplete()
        {
            barParticleSystem.Stop();
        }

        private void AnimateNextTitle()
        {
            if (playerModel.leaguePromoted)
            {
                congratulationsImg.DOFade(1, 1.3f);
                careerProgressionText.DOFade(1, 1.3f);
                continueBtn.image.DOFade(1, 1.3f);
                continueBtnText.DOFade(1, 1.3f);
            }
            else
            {
                FadeOutCareerProgressionView();
            }
        }

        private void FadeOutCareerProgressionView()
        {
            blurredBgImg.DOFade(0, 0.3f);
            UIBlurBackground.AnimateBrightness(0.7f, 0.0f, 0.3f);
            careerSectionGroup.transform.DOScale(1.0f, 0.25f).SetEase(Ease.OutSine);
            careerSectionGroup.DOFade(0.0f, 0.35f).OnComplete(LoadLobby); ;
            isCareerProgressionShown = true;
        }

        private void FadeInCareerProgressionView()
        {
            canvasGroup.DOFade(1, 0);
        }

        public void Reset()
        {
            barParticleSystem.Stop();
            progressBarSlider.DOValue(barFillPercentage, 0);
            progressBarSlider.transform.DOScale(1, 0);
            starImg.transform.DOScale(1, 0);
            careerTxtImg.transform.DOScale(1, 0);
            playerStarsCountLabel.transform.DOScale(1, 0);
            congratulationsImg.DOFade(0, 0);
            careerProgressionText.DOFade(0, 0);
            continueBtn.image.DOFade(0, 0);
            continueBtnText.DOFade(0, 0);
            blurredBgImg.DOFade(0, 0);
            careerSectionGroup.DOFade(1, 0);
            careerSectionGroup.transform.DOScale(1, 0);
            canvasGroup.alpha = 0;
            isCareerProgressionShown = false;
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

            playerStarsCountLabel.text = target.ToString();
            playerStarsCountLabel.transform.DOScale(1.25f, 0.3f).SetEase(Ease.OutBounce).OnComplete(()=> playerStarsCountLabel.transform.DOScale(1.0f, 0.2f));
        }

        #endregion
    }
}
