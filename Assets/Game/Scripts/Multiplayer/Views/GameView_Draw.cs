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

using TurboLabz.Gamebet;

namespace TurboLabz.MPChess
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

        public GameObject claimDrawDialog;
        public Button drawYesButton;
        public Button drawNoButton;
        public Text drawHeading;
        public Text drawDetails;
        public Text drawYesButtonText;
        public Text drawNoButtonText;

        public void InitDraw()
        {
            drawYesButton.onClick.AddListener(OnDrawClaimed);
            drawNoButton.onClick.AddListener(OnDrawRejected);
            drawHeading.text = localizationService.Get(LocalizationKey.GM_DRAW_DIALOG_HEADING);
            drawHeading.color = greenText;
            drawYesButtonText.text = localizationService.Get(LocalizationKey.GM_DRAW_DIALOG_YES_BUTTON);
            drawNoButtonText.text = localizationService.Get(LocalizationKey.GM_DRAW_DIALOG_NO_BUTTON);

            resignButton.onClick.AddListener(OnDrawClaimed);
        }

        public void CleanupDraw()
        {
            drawYesButton.onClick.RemoveListener(OnDrawClaimed);
            drawNoButton.onClick.RemoveListener(OnDrawRejected);
            resignButton.onClick.RemoveListener(OnDrawClaimed);
        }

        public void ShowFiftyMoveDraw()
        {
            drawDetails.text = localizationService.Get(LocalizationKey.GM_DRAW_DIALOG_CLAIM_BY_FIFTY_MOVE_RULE);
            ActivateDialog();
        }

        public void ShowThreefoldRepeatDraw()
        {
            drawDetails.text = localizationService.Get(LocalizationKey.GM_DRAW_DIALOG_CLAIM_BY_THREEFOLD_REPEAT_RULE);
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
            HideDrawDialog();
            drawClaimedSignal.Dispatch();
        }

        private void OnDrawRejected()
        {
            HideDrawDialog();
            drawRejectedSignal.Dispatch();
        }
    }
}
