using UnityEngine.UI;
using TurboLabz.Chess;
using UnityEngine;
using TurboLabz.InstantFramework;
using strange.extensions.signal.impl;
using TurboLabz.TLUtils;
using TMPro;
using TurboLabz.InstantGame;

namespace TurboLabz.CPU
{
    public partial class GameView
    {
        [Header("Safe Move")]
        public Button safeMoveBtn;
        public TextMeshProUGUI safeMoveCountTxt;
        public Image safeMoveAdd;
        public GameObject safeMoveBorder;

        public GameObject safeMoveDlg;
        public Text safeMoveDlgTitleTxt;
        public Button safeMoveDlgConfirmBtn;
        public Text safeMoveDlgConfirmTxt;
        public Button safeMoveDlgUndoBtn;
        public Text safeMoveDlgUndoBtnTxt;

        public Signal safeMoveBtnClickedSignal = new Signal();
        public Signal safeMoveConfirmClickedSignal = new Signal();
        public Signal safeMoveUndoClickedSignal = new Signal();

        public void InitSafeMove()
        {
            safeMoveBtn.onClick.AddListener(OnSafeMoveBtnClicked);
            safeMoveDlgConfirmBtn.onClick.AddListener(OnSafeMoveConfirmClicked);
            safeMoveDlgUndoBtn.onClick.AddListener(OnSafeMoveUndoClicked);
        }

        public void OnParentShowSafeMove()
        {
            safeMoveBorder.SetActive(false);
        }

        public void UpdateSafeMoves(int count)
        {
            safeMoveCountTxt.gameObject.SetActive(count > 0);
            safeMoveAdd.gameObject.SetActive(count <= 0);
        }

        public void ShowSafeMoveDlg()
        {
            safeMoveDlg.SetActive(true);
            EnableModalBlocker();
        }

        public void HideSafeMoveDlg()
        {
            safeMoveDlg.SetActive(false);
            DisableModalBlocker();
            safeMoveBorder.SetActive(false);
        }

        void OnSafeMoveBtnClicked()
        {
            if (safeMoveAdd.gameObject.activeSelf)
            {
                openSpotPurchaseSignal.Dispatch(SpotPurchaseView.PowerUpSections.SAFEMOVES);
            }
            else
            {
                safeMoveBtnClickedSignal.Dispatch();
                DisableSafeMoveButton();
                safeMoveBorder.SetActive(true);
                audioService.Play(audioService.sounds.SFX_HINT);
            }
        }

        void OnSafeMoveConfirmClicked()
        {
            safeMoveConfirmClickedSignal.Dispatch();
        }

        void OnSafeMoveUndoClicked()
        {
            safeMoveUndoClickedSignal.Dispatch();
        }

        public void ToggleSafeMoveButton(bool isPlayerTurn)
        {
            if (isPlayerTurn)
            {
                EnableSafeMoveButton();
            }
            else
            {
                DisableSafeMoveButton();
            }
        }

        public void DisableSafeMoveButton()
        {
            safeMoveBtn.interactable = false;
            safeMoveCountTxt.color = Colors.ColorAlpha(safeMoveCountTxt.color, 0.5f);
            safeMoveAdd.color = Colors.ColorAlpha(safeMoveAdd.color, 0.5f);
        }

        public void EnableSafeMoveButton()
        {
            safeMoveBtn.interactable = true;
            safeMoveCountTxt.color = Colors.ColorAlpha(safeMoveCountTxt.color, 1f);
            safeMoveAdd.color = Colors.ColorAlpha(safeMoveAdd.color, 1f);
        }

        public void UpdateSafeMoveCount(int count)
        {
            if (count == 0)
            {
                safeMoveAdd.gameObject.SetActive(true);
                safeMoveCountTxt.gameObject.SetActive(false);
            }
            else
            {
                safeMoveAdd.gameObject.SetActive(false);
                safeMoveCountTxt.gameObject.SetActive(true);
            }

            safeMoveCountTxt.text = count.ToString();
        }

        void HideSafeMoveBorder()
        {
            safeMoveBorder.SetActive(false);
        }
    }
}
