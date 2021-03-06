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
            drawOfferAcceptButtonText.text = localizationService.Get(LocalizationKey.ACCEPT_TEXT);
            drawOfferRejectButtonText.text = localizationService.Get(LocalizationKey.DECLINE_TEXT);
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
            UIDlgManager.AnimateDlgDisable(claimDrawDialog);
        }

        private void ActivateDialog()
        {
            UIDlgManager.AnimateDlg(claimDrawDialog);
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
                    drawOfferText.text = "Draw offer sent.";
                    offerDrawDialog.SetActive(true);
                    offerButtonsDlg.SetActive(false);
                    offerTextDlg.SetActive(true);
                    var tempColor = offerDrawButtonLabel.color;
                    tempColor.a = 0.3f;
                    offerDrawButtonLabel.color = tempColor;
                    offerDrawButton.interactable = false;
                }
                else
                {
                    //accept or reject
                    offerDrawDialog.SetActive(true);
                    offerButtonsDlg.SetActive(true);
                    offerTextDlg.SetActive(false);

                    var tempColor = offerDrawButtonLabel.color;
                    tempColor.a = 0.3f;
                    offerDrawButtonLabel.color = tempColor;

                    offerDrawButton.interactable = false;
                }
            }else if(status == "rejected")
            {
                if (playerModel.id == offeredBy)
                {
                    //show text draw offer rejected
                    drawOfferText.text = "Draw offer rejected.";
                    offerButtonsDlg.SetActive(false);
                    offerTextDlg.SetActive(true);
                    offerDrawButton.interactable = true;
                    var tempColor = offerDrawButtonLabel.color;
                    tempColor.a = 0.87f;
                    offerDrawButtonLabel.color = tempColor;
                    CancelInvoke();
                    Invoke("CloseDialogue", 8f);
                }else
                {
                    offerDrawDialog.SetActive(false);
                    offerDrawButton.interactable = true;
                    var tempColor = offerDrawButtonLabel.color;
                    tempColor.a = 0.7f;
                    offerDrawButtonLabel.color = tempColor;
                }
            }
            else if (status == null)
            {
                offerDrawDialog.SetActive(false);
                offerDrawButton.interactable = true;
                var tempColor = offerDrawButtonLabel.color;
                tempColor.a = 0.87f;
                offerDrawButtonLabel.color = tempColor;
            }
        }

        public void OfferDrawRejectButtonClicked()
        {
            drawOfferRejectedSignal.Dispatch();
            analyticsService.Event(AnalyticsEventId.offer_draw, AnalyticsContext.rejected);
        }

        public void OfferDrawAcceptButtonClicked()
        {
            offerDrawDialog.SetActive(false);
            drawOfferAcceptedSignal.Dispatch();
            analyticsService.Event(AnalyticsEventId.offer_draw, AnalyticsContext.accepted);
        }

        void CloseDialogue()
        {
            offerTextDlg.SetActive(false);
        }
    }
}
