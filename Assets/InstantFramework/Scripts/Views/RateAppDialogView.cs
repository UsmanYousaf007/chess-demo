/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.mediation.impl;
using UnityEngine.UI;
using TurboLabz.InstantFramework;
using UnityEngine;
using DG.Tweening;

namespace TurboLabz.InstantGame
{
    public class RateAppDialogView : View
    {
        [Header("Main Dialogue")]
        public Text titleLabel;
        //public Text subTitle;
        public Button closeButton;
        public Button yesButton;
        public Text yesButtonLabel;
        public Button noButton;
        public Text noButtonLabel;
        public Button leaveFeedbackButton;
        //public Text leaveFeedbackLabel;
        public GameObject dlg;

        [Header("Yes Dialogue")]
        public GameObject yesDlg;
        public Text yesDlgTitleLabel;
        public Button rateButton;
        public Text rateButtonLabel;
        public Button yesDlgCloseButton;
        public Button yesDlgMaybeButton;
        public Text yesDlgMaybeButtonLabel;

        [Header("No Dialogue")]
        public GameObject noDlg;
        public Text noDlgTitleLabel;
        public Button tellUsButton;
        public Text tellUsButtonLabel;
        public Button noDlgCloseButton;
        public Button noDlgMaybeButton;
        public Text noDlgMaybeButtonLabel;

        private const float RATEAPP_SHORT_DELAY_TIME = 0.8f;
        private const float RATEAPP_DIALOG_DURATION = 0.4f;

        [Inject] public ILocalizationService localizationService { get; set; }

        public void Init()
        {
            titleLabel.text = localizationService.Get(LocalizationKey.RATE_APP_TITLE) + " " + Application.productName + "?";
            yesButtonLabel.text = localizationService.Get(LocalizationKey.RATE_APP_LOVE);
            noButtonLabel.text = localizationService.Get(LocalizationKey.RATE_APP_IMPROVE);


            yesDlgTitleLabel.text = localizationService.Get(LocalizationKey.RATE_APP_SUB_TITLE_RATE);
            rateButtonLabel.text = localizationService.Get(LocalizationKey.RATE_APP_RATE);
            yesDlgMaybeButtonLabel.text = localizationService.Get(LocalizationKey.RATE_APP_NOT_NOW);


            noDlgTitleLabel.text = localizationService.Get(LocalizationKey.RATE_APP_SUB_TITLE_TELL);
            tellUsButtonLabel.text = localizationService.Get(LocalizationKey.RATE_APP_TELL);
            noDlgMaybeButtonLabel.text = localizationService.Get(LocalizationKey.RATE_APP_NOT_NOW);
        }

        private void AnimateYouEnjoingDialog()
        {
            dlg.transform.DOLocalMove(Vector3.zero, RATEAPP_DIALOG_DURATION).SetEase(Ease.OutBack);
        }

        public void ShowAreYouEnjoying()
        {
            gameObject.SetActive(true);
            //dlg.transform.localPosition = new Vector3(0f, Screen.height + 800, 0f);

            //Invoke("AnimateYouEnjoingDialog", RATEAPP_SHORT_DELAY_TIME);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void ShowYesDialogue()
        {
            dlg.SetActive(false);
            yesDlg.SetActive(true);
        }

        public void ShowNoDialogue()
        {
            dlg.SetActive(false);
            noDlg.SetActive(true);
        }
    }
}