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

        private const float TRANSITION_DURATION = 0.5f;

        private void StartEndAnimationSequence()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.AppendCallback(() => EnableModalBlocker());
            sequence.AppendInterval(0.5f);
            sequence.AppendCallback(() => SetBlurBg(true));
            sequence.AppendCallback(() => ShowRewardsDialog());
            sequence.AppendCallback(() => AnimateRewardsDialog());
            sequence.PlayForward();
        }

        #region Rewards Dialogue

        private void AnimateRewardsDialog()
        {
            if (!isRankedGame || (isRankedGame && !playerWins && !isDraw))
                return;

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
            if (playerWins && !animationPlayed && isRankedGame)
            {
                _winAnimationSequence.PlayAnimation().Then(() => OnAnimationRewardsComplete());
            }
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
            rewardsCanvasGroup.DOFade(val, TRANSITION_DURATION);
        }

        #endregion


        #region Results Dialogue

        private void ScaleInResultsDialog()
        {
            resultsDialog.transform.DOScale(Vector3.one, TRANSITION_DURATION).SetEase(Ease.OutBack);
        }

        private void FadeOutResultsDialog(float val)
        {
            resultsCanvasGroup.DOFade(val, TRANSITION_DURATION);
        }

        #endregion

    }
}
