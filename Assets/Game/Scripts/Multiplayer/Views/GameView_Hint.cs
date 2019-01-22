/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


using UnityEngine;
using UnityEngine.UI;

using strange.extensions.signal.impl;
using TurboLabz.Chess;
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        public Signal hintClickedSignal = new Signal();

        [Header("Hint")]
        public GameObject hintFromIndicator;
        public GameObject hintToIndicator;
        public Button hintButton;
        public Text hintButtonLabel;
        public Text hintCountLabel;
        public GameObject hintAdd;
        public GameObject hintThinking;

        private int availableHints;

        public void InitHint()
        {
            //hintButtonLabel.text = localizationService.Get(LocalizationKey.CPU_GAME_HINT_BUTTON);
            hintButton.onClick.AddListener(HintButtonClicked);
            hintThinking.SetActive(false);
        }

        public void OnParentShowHint()
        {
            HideHint();
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
        }

        public void HideHint()
        {
            hintFromIndicator.SetActive(false);
            hintToIndicator.SetActive(false);
            hintThinking.SetActive(false);
        }

        private void HintButtonClicked()
        {
            if (hintAdd.activeSelf)
            {
                LogUtil.Log("Show hint spot purchase", "cyan");
            }
            else
            {
                hintThinking.SetActive(true);
                EnableModalBlocker(false);
                hintClickedSignal.Dispatch();
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
        }

        public void EnableHintButton()
        {
            hintButton.interactable = true;
        }

        public void UpdateHintCount(int count)
        {
            if (count == 0)
            {
                DisableHintButton();
                hintAdd.SetActive(true);
                hintCountLabel.gameObject.SetActive(false);
            }
            else
            {
                hintAdd.SetActive(false);
                hintCountLabel.gameObject.SetActive(true);
            }

            hintCountLabel.text = count.ToString();
        }

    }
}
