﻿using UnityEngine.UI;
using TurboLabz.Chess;
using UnityEngine;
using TurboLabz.InstantFramework;
using strange.extensions.signal.impl;
using TurboLabz.TLUtils;
using TMPro;
using TurboLabz.InstantGame;

namespace TurboLabz.Multiplayer
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

        public Text safeMoveONLabel;
        public Text safeMoveOFFLabel;

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
            safeMoveONLabel.gameObject.SetActive(false);
            safeMoveOFFLabel.gameObject.SetActive(true);
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
            safeMoveONLabel.gameObject.SetActive(false);
            safeMoveOFFLabel.gameObject.SetActive(true);
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
                safeMoveBorder.SetActive(true);
                safeMoveONLabel.gameObject.SetActive(true);
                safeMoveOFFLabel.gameObject.SetActive(false);
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
            safeMoveONLabel.gameObject.SetActive(false);
            safeMoveOFFLabel.gameObject.SetActive(true);
        }
    }
}
