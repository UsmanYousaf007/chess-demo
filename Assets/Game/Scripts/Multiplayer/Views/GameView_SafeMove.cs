using UnityEngine.UI;
using TurboLabz.Chess;
using UnityEngine;
using TurboLabz.InstantFramework;
using strange.extensions.signal.impl;
using TurboLabz.TLUtils;

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        [Header("Safe Move")]
        public Button safeMoveBtn;
        public Text safeMoveCountTxt;
        public GameObject safeMoveAdd;

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
            // Nothing here yet
        }

        public void UpdateSafeMoves(int count)
        {
            safeMoveCountTxt.gameObject.SetActive(count > 0);
            safeMoveAdd.SetActive(count <= 0);
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
        }

        void OnSafeMoveBtnClicked()
        {
            if (safeMoveAdd.activeSelf)
            {
                LogUtil.Log("Show safe move spot purchase", "cyan");
            }
            else
            {
                safeMoveBtnClickedSignal.Dispatch();
                DisableSafeMoveButton();
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
        }

        private void EnableSafeMoveButton()
        {
            safeMoveBtn.interactable = true;
        }

        public void UpdateSafeMoveCount(int count)
        {
            if (count == 0)
            {
                DisableSafeMoveButton();
                safeMoveAdd.SetActive(true);
                safeMoveCountTxt.gameObject.SetActive(false);
            }
            else
            {
                safeMoveAdd.SetActive(false);
                safeMoveCountTxt.gameObject.SetActive(true);
            }

            safeMoveCountTxt.text = count.ToString();
        }
    }
}
