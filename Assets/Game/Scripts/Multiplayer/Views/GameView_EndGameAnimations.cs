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

        private float animDelay;

        private const float REWARDS_DIALOG_DURATION = 0.5f;
        private const float REWARDS_TO_RESULTS_DELAY_TIME = 1f;

        private const float RESULTS_DIALOG_DURATION = 0.5f;
        private const float RESULTS_DELAY_TIME = 1f;
        private const float RESULTS_SHORT_DELAY_TIME = 0.3f;

        private const float GAME_ANALYZING_DURATION = 4f;

        private const float TRANSITION_DURATION = 0.5f;

        private void StartEndAnimationSequence()
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
            sequence.AppendCallback(() => ShowResultsDialog());
            sequence.AppendCallback(() => ScaleInResultsDialog());
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

        #endregion

        #region Results Dialogue

        private void FadeInOrOutAnalyzingDialog(float val)
        {
            analyzingDlgCanvasGroup.DOFade(val, TRANSITION_DURATION);
        }

        public IEnumerator AnimateBars(float averageHeight)
        {
            float animateDuration = 0.3f;
            while (true) {
                foreach (Image bar in loadingBars)
                {
                    int val = (int)Random.Range(averageHeight / 2, averageHeight * 2);
                    AnimateBar(bar, val, animateDuration);
                }
                yield return new WaitForSeconds(animateDuration);
            }
        }

        private void AnimateAnalyzingDlg()
        {
            //resultsCanvasGroup.alpha = 0;
            //analyzingDlg.SetActive(true);
            UIDlgManager.ShowScreenDlg(analyzingDlg);
            //FadeInOrOutAnalyzingDialog(1);
            float averageHeight = loadingBars[0].rectTransform.sizeDelta.y;
            StartCoroutine(AnimateBars(averageHeight));
        }

        private void AnimateBar(Image bar, int val, float animateDuration)
        {
            bar.rectTransform.DOSizeDelta(new Vector2(bar.rectTransform.sizeDelta.x, val), animateDuration).SetEase(Ease.InOutBounce);
        }

        #endregion

    }
}
