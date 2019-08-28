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
using TMPro;
using TurboLabz.InstantGame;
using System.Collections;

namespace TurboLabz.CPU
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
        public TextMeshProUGUI hintCountLabel;
        public Image hintAdd;
        public GameObject hintThinking;
        public GameObject strengthPanel;
        public Text strengthLabel;
        public Image[] barArray;

        private int availableHints;
        private const float MIN_WAIT = 0.1f;
        public float dotWaitSeconds;

        public DrawLine line;

        public void InitHint()
        {
            //hintButtonLabel.text = localizationService.Get(LocalizationKey.CPU_GAME_HINT_BUTTON);
            hintButton.onClick.AddListener(HintButtonClicked);
            hintThinking.SetActive(false);
            strengthPanel.SetActive(false);
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

            ShowStrengthPanel(vo.strength);

        }

        private Coroutine barAnim = null;
        private Coroutine panelActive;

        private void ShowStrengthPanel(int strength)
        {
            strengthPanel.SetActive(true);

            //draw line
            line.Draw(hintFromIndicator.transform.position, hintToIndicator.transform.position);

            Vector3 localPos = hintToIndicator.transform.localPosition;

            float addPosX = 150;
            float addPosY = 110;

            if(localPos.x < 0)
                localPos.x = localPos.x + addPosX;
            else
                localPos.x = localPos.x - addPosX;

            localPos.y += addPosY;

            strengthPanel.transform.localPosition = localPos;


            DisableBar();
            int strengthString = strength * 10;
            strengthLabel.text = "Strength " + strengthString.ToString() + "%";

            if (strength > 0 && strength <= barArray.Length)
            {
                barAnim = StartCoroutine(Animate(strength));
            }

            StartCoroutine(HideStrengthPanel(3.0f));
        }


        public IEnumerator HideStrengthPanel(float t)
        {
            yield return new WaitForSeconds(t);
            strengthPanel.SetActive(false);
            HideHint();

            if (barAnim != null)
            {
                StopCoroutine(barAnim);
                barAnim = null;
            }
        }

        public void DisableBar()
        {
            for(int i=0; i<barArray.Length; i++)
            {
                barArray[i].color = Colors.strengthBarDisableColor;
            }
        }

        IEnumerator Animate(int strength)
        {
            float waitTime = Mathf.Max(MIN_WAIT, dotWaitSeconds);

            while (true)
            {
                for (int i = 0; i < strength; i++)
                {
                    barArray[i].color = Colors.strengthBarColor[i];
                    yield return new WaitForSeconds(waitTime);

                }

                yield return new WaitForSeconds(waitTime);
            }
        }

        public void HideHint()
        {
            hintFromIndicator.SetActive(false);
            hintToIndicator.SetActive(false);
            hintThinking.SetActive(false);
            line.Hide();
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

                analyticsService.Event(AnalyticsEventId.tap_pow_hint, AnalyticsContext.computer_match);
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
            hintButton.interactable = true;
            hintCountLabel.color = Colors.ColorAlpha(hintCountLabel.color, 1f);
            hintAdd.color = Colors.ColorAlpha(hintAdd.color, 1f);
            hintLabel.color = Colors.ColorAlpha(hintLabel.color, 0.87f);
            hintIcon.color = Colors.ColorAlpha(hintIcon.color, 1f);
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
