/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.mediation.impl;
using UnityEngine.UI;
using TurboLabz.InstantFramework;
using UnityEngine;

namespace TurboLabz.InstantGame
{
    public class RateAppDialogView : View
    {
        public Text titleLabel;
        public Text subTitleLabel;
        public Button notNowButton;
        public Text notNowButtonLabel;
        public Button rateButton;
        public Text rateButtonLabel;

        [Inject] public ILocalizationService localizationService { get; set; }

        public void Init()
        {
            titleLabel.text = localizationService.Get(LocalizationKey.RATE_APP_TITLE) + " " + Application.productName + "?";
            subTitleLabel.text = localizationService.Get(LocalizationKey.RATE_APP_SUB_TITLE);
            notNowButtonLabel.text = localizationService.Get(LocalizationKey.RATE_APP_NOT_NOW);
            rateButtonLabel.text = localizationService.Get(LocalizationKey.RATE_APP_RATE);  
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}