/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-06 17:38:06 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.signal.impl;
using TurboLabz.Chess;
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;

namespace TurboLabz.InstantChess
{
    public partial class GameView
    {
        public Signal hintClickedSignal = new Signal();

        public GameObject hintFromIndicator;
        public GameObject hintToIndicator;
        public Button hintButton;
        public Text hintButtonLabel;
        public Text hintCountLabel;

        private int availableHints;

        public void InitHint()
        {
            hintButtonLabel.text = localizationService.Get(LocalizationKey.CPU_GAME_HINT_BUTTON);
            hintButton.onClick.AddListener(HintButtonClicked);
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

            audioSource.PlayOneShot(hintFx, FX_VOLUME);

            UpdateHintCount(vo.availableHints);
            DisableHintButton();
        }

        public void HideHint()
        {
            hintFromIndicator.SetActive(false);
            hintToIndicator.SetActive(false);
        }

        public void UpdateHintCount(int count)
        {
            availableHints = count;

            if (availableHints == 0)
            {
                DisableHintButton();
            }

            hintCountLabel.text = count.ToString();
        }

        public void ToggleHintButton(bool isPlayerTurn)
        {
            if (isPlayerTurn && availableHints > 0)
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
            DisableLabel(hintButtonLabel);
            DisableLabel(hintCountLabel);
        }

        public bool IsHintButtonActive()
        {
            return hintButton.interactable;
        }

        private void HintButtonClicked()
        {
            hintClickedSignal.Dispatch();
        }

        private void EnableHintButton()
        {
            if (availableHints > 0)
            {
                hintButton.interactable = true;
                EnableLabel(hintButtonLabel);
                EnableLabel(hintCountLabel);
            }
        }
    }
}
