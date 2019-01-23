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

namespace TurboLabz.CPU
{
    public partial class GameView
    {
        public Signal hindsightClickedSignal = new Signal();

        [Header("Hindsight")]
        public GameObject hindsightFromIndicator;
        public GameObject hindsightToIndicator;
        public Button hindsightButton;
        public TextMeshProUGUI hindsightCountLabel;
        public Image hindsightAdd;
        public GameObject hindsightThinking;

        public void InitHindsight()
        {
            //hintButtonLabel.text = localizationService.Get(LocalizationKey.CPU_GAME_HINT_BUTTON);
            hindsightButton.onClick.AddListener(HindsightButtonClicked);
            hindsightThinking.SetActive(false);
        }

        public void OnParentShowHindsight()
        {
            HideHindsight();
        }

        public void RenderHindsight(HintVO vo)
        {
            int fromSquareIndex = RankFileMap.Map[vo.fromSquare.fileRank.rank, vo.fromSquare.fileRank.file];
            hindsightFromIndicator.transform.position = chessboardSquares[fromSquareIndex].position;
            hindsightFromIndicator.SetActive(true);

            int toSquareIndex = RankFileMap.Map[vo.toSquare.fileRank.rank, vo.toSquare.fileRank.file];
            hindsightToIndicator.transform.position = chessboardSquares[toSquareIndex].position;
            hindsightToIndicator.SetActive(true);

            audioService.Play(audioService.sounds.SFX_HINT);

            hindsightThinking.SetActive(false);
            DisableModalBlocker();
            DisableHindsightButton();
        }

        public void HideHindsight()
        {
            hindsightFromIndicator.SetActive(false);
            hindsightToIndicator.SetActive(false);
            hindsightThinking.SetActive(false);
        }

        private void HindsightButtonClicked()
        {
            if (hindsightAdd.gameObject.activeSelf)
            {
                LogUtil.Log("Show hindsight spot purchase", "cyan");
            }
            else
            {
                hindsightThinking.SetActive(true);
                EnableModalBlocker(false);
                hindsightClickedSignal.Dispatch();
            }
        }

        public void ToggleHindsightButton(bool on)
        {
            if (on)
            {
                EnableHindsightButton();
            }
            else
            {
                DisableHindsightButton();
            }
        }

        public void DisableHindsightButton()
        {
            hindsightButton.interactable = false;
            hindsightCountLabel.color = Colors.ColorAlpha(hindsightCountLabel.color, 0.5f);
            hindsightAdd.color = Colors.ColorAlpha(hindsightAdd.color, 0.5f);
        }

        private void EnableHindsightButton()
        {
            hindsightButton.interactable = true;
            hindsightCountLabel.color = Colors.ColorAlpha(hindsightCountLabel.color, 1f);
            hindsightAdd.color = Colors.ColorAlpha(hindsightAdd.color, 1f);
        }

        public void UpdateHindsightCount(int count)
        {
            if (count == 0)
            {
                DisableHindsightButton();
                hindsightAdd.gameObject.SetActive(true);
                hindsightCountLabel.gameObject.SetActive(false);
            }
            else
            {
                hindsightAdd.gameObject.SetActive(false);
                hindsightCountLabel.gameObject.SetActive(true);
            }

            hindsightCountLabel.text = count.ToString();
        }
    }
}
