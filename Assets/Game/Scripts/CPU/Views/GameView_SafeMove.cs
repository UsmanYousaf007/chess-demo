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
        public Image safeMoveBg;
        public Text safeMoveLabel;
        public Image safeMoveIcon;
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

        bool safeMoveOn;

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
            if (safeMoveOn)
            {
                audioService.Play(audioService.sounds.SFX_CLICK);
            }


            if (!safeMoveOn && safeMoveAdd.gameObject.activeSelf)
            {
                openSpotPurchaseSignal.Dispatch(SpotPurchaseView.PowerUpSections.SAFEMOVES);
            }
            else
            {
                if (!safeMoveOn)
                {
                    audioService.Play(audioService.sounds.SFX_HINT);
                }

                safeMoveBtnClickedSignal.Dispatch();
            }
        }

        public void ToggleSafeMove(bool on)
        {
            if (on)
            {
                SafeMoveOn();
            }
            else
            {
                SafeMoveOff();
            }
        }

        void SafeMoveOn()
        {
            safeMoveLabel.text = "Undo On";
            safeMoveBg.color = Colors.GLASS_GREEN;
            safeMoveOn = true;
            safeMoveIcon.color = Colors.ColorAlpha(safeMoveIcon.color, 1.0f);
        }

        void SafeMoveOff()
        {
            safeMoveLabel.text = "Undo Off";
            safeMoveBg.color = Color.white;
            safeMoveOn = false;
            safeMoveIcon.color = Colors.ColorAlpha(safeMoveIcon.color, 0.5f);
        }

        void OnSafeMoveConfirmClicked()
        {
            safeMoveConfirmClickedSignal.Dispatch();
        }

        void OnSafeMoveUndoClicked()
        {
            safeMoveUndoClickedSignal.Dispatch();
        }

        public void DisableSafeMoveButton()
        {
            safeMoveBtn.interactable = false;
            safeMoveCountTxt.color = Colors.ColorAlpha(safeMoveCountTxt.color, 0.5f);
            safeMoveAdd.color = Colors.ColorAlpha(safeMoveAdd.color, 0.5f);
            safeMoveLabel.color = Colors.ColorAlpha(safeMoveLabel.color, 0.5f);
            safeMoveIcon.color = Colors.ColorAlpha(safeMoveIcon.color, 0.5f);
            safeMoveBg.color = Color.white;
        }

        public void EnableSafeMoveButton()
        {
            safeMoveBtn.interactable = true;
            safeMoveCountTxt.color = Colors.ColorAlpha(safeMoveCountTxt.color, 1f);
            safeMoveAdd.color = Colors.ColorAlpha(safeMoveAdd.color, 1f);
            safeMoveLabel.color = Colors.ColorAlpha(safeMoveLabel.color, 0.87f);
            safeMoveIcon.color = Colors.ColorAlpha(safeMoveIcon.color, 1.0f);
        }

        public void UpdateSafeMoveCount(int count)
        {
            if (count == 0)
            {
                safeMoveAdd.gameObject.SetActive(true);
                safeMoveCountTxt.gameObject.SetActive(false);
                SafeMoveOff();
            }
            else
            {
                if (count.ToString() != safeMoveCountTxt.text && !safeMoveOn)
                {
                    safeMoveBtnClickedSignal.Dispatch();
                }

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
