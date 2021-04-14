/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using TurboLabz.Chess;
using TurboLabz.InstantGame;
using strange.extensions.promise.api;
using HUFEXT.CrossPromo.Runtime.API;
using TMPro;
using System.Collections.Generic;
using System.Collections;

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        public WinResultAnimSequence _winAnimationSequence;
        private bool animationPlayed = false;

        private float resultsDialogHalfHeight;
        private Sequence analysisFreeTagAnimationSequence;
        private float animDelay;

        private const float REWARDS_DIALOG_DURATION = 0.5f;
        private const float REWARDS_TO_RESULTS_DELAY_TIME = 1f;

        private const float RESULTS_DIALOG_DURATION = 0.5f;
        private const float RESULTS_DELAY_TIME = 1f;
        private const float RESULTS_SHORT_DELAY_TIME = 0.3f;

        private const float GAME_ANALYZING_DURATION = 4f;

        private const float TRANSITION_DURATION = 0.5f;

        public void StartEndAnimationSequence()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.AppendCallback(() => EnableModalBlocker());
            sequence.AppendInterval(0.5f);
            sequence.AppendCallback(() => ShowRewardsDialog());
            sequence.AppendCallback(() => AnimateRewardsDialog());
            sequence.PlayForward();
        }

        #region Rewards Dialogue

        private void AnimateRewardsDialog()
        {
            mirrorPanel.transform.DOLocalMove(Vector3.zero, REWARDS_DIALOG_DURATION).SetEase(Ease.OutBack).OnComplete(OnMirrorPanelAnimationComplete);

            if (isDraw || !playerWins)
            {
                audioService.Play(audioService.sounds.SFX_DEFEAT);
            }
            else
            {
                audioService.Play(audioService.sounds.SFX_VICTORY);
            }
        }

        private void OnMirrorPanelAnimationComplete()
        {
            _winAnimationSequence.PlayAnimation().Then(() => OnAnimationRewardsComplete());
        }

        private void OnAnimationRewardsComplete()
        {
            animationPlayed = true;

            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(REWARDS_TO_RESULTS_DELAY_TIME);
            sequence.AppendCallback(() => FadeRewardsDialogue(0));
            sequence.AppendCallback(() => resultsDialogOpenedSignal.Dispatch());
            //sequence.AppendCallback(() => HideRewardsDialog());
            //sequence.AppendCallback(() => ShowResultsDialog());
            //sequence.AppendCallback(() => ScaleInResultsDialog());
            sequence.PlayForward();
        }

        private void FadeRewardsDialogue(float val)
        {
            rewardsCanvasGroup.DOFade(val, TRANSITION_DURATION).OnComplete(HideRewardsDialog);
        }

        #endregion


        #region Results Dialogue

        private void ScaleInResultsDialog()
        {
            resultsDialog.transform.DOScale(Vector3.one, TRANSITION_DURATION).SetEase(Ease.OutBack);
        }

        private void FadeOutResultsDialog(float val)
        {
            resultsCanvasGroup.DOFade(val, TRANSITION_DURATION).OnComplete(HideResultsDialog);
        }

        private void AnimateResultsDlg()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.AppendCallback(() => EnableModalBlocker());
            sequence.AppendInterval(0.5f);
            sequence.AppendCallback(() => ShowResultsDialog());
            //sequence.AppendCallback(() => ScaleInResultsDialog());
            sequence.PlayForward();
        }

        private void AnimateFreeTagOnFullAnalysis(bool animate)
        {
            if (analysisFreeTagAnimationSequence != null && analysisFreeTagAnimationSequence.IsPlaying())
            {
                analysisFreeTagAnimationSequence.Kill();
                analysisFreeTagAnimationSequence = null;
            }

            if (!animate)
            {
                return;
            }

            if (analysisFreeTagAnimationSequence == null)
            {
                analysisFreeTagAnimationSequence = DOTween.Sequence();
                analysisFreeTagAnimationSequence.AppendCallback(() => resultsFullAnalysisFreeTag.transform.localEulerAngles = Vector3.zero);
                analysisFreeTagAnimationSequence.AppendCallback(() => resultsFullAnalysisFreeTag.transform.DOPunchRotation(Vector3.forward * 8, 1.3f));
                analysisFreeTagAnimationSequence.AppendInterval(3.0f);
                analysisFreeTagAnimationSequence.SetLoops(-1);
            }

            analysisFreeTagAnimationSequence.PlayForward();
        }

        #endregion

        #region Results Dialogue

        private void FadeInOrOutAnalyzingDialog(float val)
        {
            analyzingDlgCanvasGroup.DOFade(val, TRANSITION_DURATION);
        }

        public IEnumerator AnimateBars()
        {
            float animateDuration = 0.3f;
            while (animateBarsEnabled) {
                foreach (Image bar in loadingBars)
                {
                    int val = (int)Random.Range(averageHeightOfAnalyzingBar / 2, averageHeightOfAnalyzingBar * 2);
                    AnimateBar(bar, val, animateDuration);
                }
                yield return new WaitForSeconds(animateDuration);
            }

            yield return null;
        }

        private void AnimateAnalyzingDlg()
        {
            //UIDlgManager.ShowScreenDlg(analyzingDlg);
            UIDlgManager.Show(analyzingDlg, Colors.BLUR_BG_BRIGHTNESS_NORMAL, true);
            animateBarsEnabled = true;
            StartCoroutine(AnimateBars());
        }

        private void AnimateBar(Image bar, int val, float animateDuration)
        {
            bar.rectTransform.DOSizeDelta(new Vector2(bar.rectTransform.sizeDelta.x, val), animateDuration).SetEase(Ease.InOutBounce);
        }

        #endregion

    }
}
