using UnityEngine.UI;
using TurboLabz.Chess;
using UnityEngine;
using TurboLabz.InstantFramework;
using strange.extensions.signal.impl;

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

        int safeMoveCount;

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
            safeMoveCount = count;
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
            safeMoveBtnClickedSignal.Dispatch();
        }

        void OnSafeMoveConfirmClicked()
        {
            safeMoveConfirmClickedSignal.Dispatch();
        }

        void OnSafeMoveUndoClicked()
        {
            safeMoveUndoClickedSignal.Dispatch();
        }
    }
}
