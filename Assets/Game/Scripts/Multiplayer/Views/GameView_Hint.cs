﻿/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


using UnityEngine;
using UnityEngine.UI;

using strange.extensions.signal.impl;
using TurboLabz.Chess;
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;
using TMPro;
using TurboLabz.InstantGame;
using System.Collections;

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        public Signal hintClickedSignal = new Signal();

        [Header("Hint")]
        public GameObject hintFromIndicator;
        public GameObject hintToIndicator;

        public Button hintButton;
        public Text hintLabel;
        public Image hintIcon;
        public Text hintCountLabel;
        public Image hintAdd;
        public GameObject hintThinking;
        public StrengthAnim strengthPanel;

        public void InitHint()
        {
            //hintButtonLabel.text = localizationService.Get(LocalizationKey.CPU_GAME_HINT_BUTTON);
            hintButton.onClick.AddListener(HintButtonClicked);
            hintThinking.SetActive(false);
            strengthPanel.Hide();
        }

        public void OnParentShowHint()
        {
            HideHint();
            DisableHintButton();
        }

        public void RenderHint(HintVO vo)
        {
            int fromSquareIndex = RankFileMap.Map[vo.fromSquare.fileRank.rank, vo.fromSquare.fileRank.file];
            hintFromIndicator.transform.position = chessboardSquares[fromSquareIndex].position;
            hintFromIndicator.SetActive(true);

            int toSquareIndex = RankFileMap.Map[vo.toSquare.fileRank.rank, vo.toSquare.fileRank.file];
            hintToIndicator.transform.position = chessboardSquares[toSquareIndex].position;
            hintToIndicator.SetActive(true);

            audioService.Play(audioService.sounds.SFX_HINT);

            //UpdateHintCount(vo.availableHints);
            //DisableHintButton();
            hintThinking.SetActive(false);
            DisableModalBlocker();
            DisableHintButton();

            var strengthVO = new StrengthVO();
            strengthVO.strength = vo.strength;
            strengthVO.fromPosition = hintFromIndicator.transform.position;
            strengthVO.toPosition = hintToIndicator.transform.position;
            strengthVO.fromIndicator = hintFromIndicator;
            strengthVO.toIndicator = hintToIndicator;

            strengthPanel.ShowStrengthPanel(strengthVO);
            StartCoroutine(HideHint(4.0f));
        }

        public void CancelHint()
        {
            hintThinking.SetActive(false);
            DisableModalBlocker();
            DisableHintButton();
            strengthPanel.Hide();
        }

        public IEnumerator HideHint(float t)
        {
            yield return new WaitForSeconds(t);
            HideHint();
        }

        public void HideHint()
        {
            hintFromIndicator.SetActive(false);
            hintToIndicator.SetActive(false);
            hintThinking.SetActive(false);
        }

        private void HintButtonClicked()
        {
            if (hintAdd.gameObject.activeSelf)
            {
                openSpotPurchaseSignal.Dispatch(SpotPurchaseView.PowerUpSections.HINTS);
            }
            else
            {
                hintThinking.SetActive(true);
                EnableModalBlocker(Colors.UI_BLOCKER_INVISIBLE_ALPHA);
                hintClickedSignal.Dispatch();

                if (isLongPlay)
                {
                    analyticsService.Event(AnalyticsEventId.tap_pow_hint, AnalyticsContext.long_match);
                }
                else
                {
                    analyticsService.Event(AnalyticsEventId.tap_pow_hint, AnalyticsContext.quick_match);
                }
            }
        }

        public void ToggleHintButton(bool isPlayerTurn)
        {
            if (isPlayerTurn)
            {
                EnableHintButton();
            }
            else
            {
                DisableHintButton();
            }
        }

        public void DisableHintButton()
        {
            hintButton.interactable = false;
            hintCountLabel.color = Colors.ColorAlpha(hintCountLabel.color, 0.5f);
            hintAdd.color = Colors.ColorAlpha(hintAdd.color, 0.5f);
            hintLabel.color = Colors.ColorAlpha(hintLabel.color, 0.5f);
            hintIcon.color = Colors.ColorAlpha(hintIcon.color, 0.5f);
        }

        public void EnableHintButton()
        {
            //if (isLongPlay && !isRankedGame)
            //{
                hintButton.interactable = true;
                hintCountLabel.color = Colors.ColorAlpha(hintCountLabel.color, 1f);
                hintAdd.color = Colors.ColorAlpha(hintAdd.color, 1f);
                hintLabel.color = Colors.ColorAlpha(hintLabel.color, 0.87f);
                hintIcon.color = Colors.ColorAlpha(hintIcon.color, 1f);
            //}
        }

        public void UpdateHintCount(int count)
        {
            if (count == 0)
            {
                hintAdd.gameObject.SetActive(true);
                hintCountLabel.gameObject.SetActive(false);
            }
            else
            {
                hintAdd.gameObject.SetActive(false);
                hintCountLabel.gameObject.SetActive(true);
            }

            hintCountLabel.text = count.ToString();
        }

    }
}
