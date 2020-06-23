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
        public Text titleLabel;
        public Text subTitle;
        public Button closeButton;
        public Button rateButton;
        public Text rateButtonLabel;
        public Button leaveFeedbackButton;
        public Text leaveFeedbackLabel;
        public GameObject dlg;

        private const float RATEAPP_SHORT_DELAY_TIME = 0.8f;
        private const float RATEAPP_DIALOG_DURATION = 0.4f;

        [Inject] public ILocalizationService localizationService { get; set; }

        public void Init()
        {
            titleLabel.text = localizationService.Get(LocalizationKey.RATE_APP_TITLE) + " " + Application.productName + "?";
            subTitle.text = localizationService.Get(LocalizationKey.RATE_APP_SUB_TITLE_RATE);
            leaveFeedbackLabel.text  = localizationService.Get(LocalizationKey.RATE_APP_TELL);
            rateButtonLabel.text = localizationService.Get(LocalizationKey.RATE_APP_RATE);
        }

        private void AnimateYouEnjoingDialog()
        {
            dlg.transform.DOLocalMove(Vector3.zero, RATEAPP_DIALOG_DURATION).SetEase(Ease.OutBack);
        }

        public void ShowAreYouEnjoying()
        {
            gameObject.SetActive(true);
            dlg.transform.localPosition = new Vector3(0f, Screen.height + 800, 0f);

            Invoke("AnimateYouEnjoingDialog", RATEAPP_SHORT_DELAY_TIME);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}