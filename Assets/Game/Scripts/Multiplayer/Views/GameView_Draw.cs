/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-06 17:39:25 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        private enum DrawClaimType
        {
            FIFTY_MOVE,
            THREEFOLD_REPEAT
        }
            
        public Signal drawClaimedSignal = new Signal();
        public Signal drawRejectedSignal = new Signal();

        [Header("Draw")]
        public GameObject claimDrawDialog;
        public Button drawYesButton;
        public Button drawNoButton;
        public Text drawHeading;
        public Text drawYesButtonText;
        public Text drawNoButtonText;


        //Offer Draw
        public Signal drawOfferAcceptedSignal = new Signal();
        public Signal drawOfferRejectedSignal = new Signal();

        [Header("Offer Draw")]
        public GameObject offerDrawDialog;
        public Button drawOfferAcceptButton;
        public Button drawOfferRejectButton;
        public Text drawOfferText;
        public Text drawOfferAcceptButtonText;
        public Text drawOfferRejectButtonText;
        public GameObject offerButtonsDlg;
        public GameObject offerTextDlg;


        public void InitDraw()
        {
            drawYesButton.onClick.AddListener(OnDrawClaimed);
            drawNoButton.onClick.AddListener(OnDrawRejected);
            drawYesButtonText.text = localizationService.Get(LocalizationKey.GM_DRAW_DIALOG_YES_BUTTON);
            drawNoButtonText.text = localizationService.Get(LocalizationKey.GM_DRAW_DIALOG_NO_BUTTON);

            drawOfferAcceptButton.onClick.AddListener(OfferDrawAcceptButtonClicked);
            drawOfferRejectButton.onClick.AddListener(OfferDrawRejectButtonClicked);
            drawOfferAcceptButtonText.text = localizationService.Get(LocalizationKey.GM_DRAW_DIALOG_YES_BUTTON);
            drawOfferRejectButtonText.text = localizationService.Get(LocalizationKey.GM_DRAW_DIALOG_NO_BUTTON);
        }

        public void CleanupDraw()
        {
            drawYesButton.onClick.RemoveListener(OnDrawClaimed);
            drawNoButton.onClick.RemoveListener(OnDrawRejected);
        }

        public void ShowFiftyMoveDraw()
        {
            drawHeading.text = localizationService.Get(LocalizationKey.GM_DRAW_DIALOG_CLAIM_BY_FIFTY_MOVE_RULE);
            ActivateDialog();
        }

        public void ShowThreefoldRepeatDraw()
        {
            drawHeading.text = localizationService.Get(LocalizationKey.GM_DRAW_DIALOG_CLAIM_BY_THREEFOLD_REPEAT_RULE);
            ActivateDialog();
        }

        public void HideDrawDialog()
        {
            DisableModalBlocker();
            claimDrawDialog.SetActive(false);
        }

        private void ActivateDialog()
        {
            EnableModalBlocker();
            claimDrawDialog.SetActive(true);
            claimDrawDialog.transform.localPosition = new Vector3(0f, 1024f, 0f);
            claimDrawDialog.transform.DOLocalMove(new Vector3(0f, 0f, 0f), 0.5f).SetEase(Ease.OutBack);
        }

        private void OnDrawClaimed()
        {
            drawClaimedSignal.Dispatch();
            EnableModalBlocker();
        }

        private void OnDrawRejected()
        {
            drawRejectedSignal.Dispatch();
        }

        public void OfferDraw(string status, string offeredBy)
        {
            if (status == "offered")
            {
                if (playerModel.id == offeredBy)
                {
                    //show text draw offer sent
                    drawOfferText.text = "Draw offer sent";
                    offerDrawDialog.SetActive(true);
                    offerButtonsDlg.SetActive(false);
                    offerTextDlg.SetActive(true);
                    CancelInvoke();
                    Invoke("CloseDialogue", 4f);
                    analyticsService.Event(AnalyticsEventId.offer_draw_sent);
                }
                else
                {
                    //accept or reject
                    offerDrawDialog.SetActive(true);
                    offerButtonsDlg.SetActive(true);
                    offerTextDlg.SetActive(false);
                }
            }else if(status == "rejected")
            {
                if (playerModel.id == offeredBy)
                {
                    //show text draw offer rejected
                    drawOfferText.text = "Draw offer rejected";
                    offerButtonsDlg.SetActive(false);
                    offerTextDlg.SetActive(true);
                    CancelInvoke();
                    Invoke("CloseDialogue", 8f);
                }else
                {
                    offerDrawDialog.SetActive(false);
                }
            }
            else if (status == null)
            {
                offerDrawDialog.SetActive(false);
            }
        }

        public void OfferDrawRejectButtonClicked()
        {
            drawOfferRejectedSignal.Dispatch();
            analyticsService.Event(AnalyticsEventId.offer_draw_rejected);
        }

        public void OfferDrawAcceptButtonClicked()
        {
            offerDrawDialog.SetActive(false);
            drawOfferAcceptedSignal.Dispatch();
            analyticsService.Event(AnalyticsEventId.offer_draw_accepted);
        }

        void CloseDialogue()
        {
            offerTextDlg.SetActive(false);
        }
    }
}
