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
        public Text titleRateLabel;
        public Text titleTeamLoveLabel;
        public Button closeButton;

        public Button improveButton;
        public Text improveButtonLabel;

        public Button likeButton;
        public Text likeButtonLabel;

        public Button loveButton;
        public Text loveButtonLabel;

        public Button rateButton;
        public Text rateButtonLabel;

        public Button tellUsButton;
        public Text tellUsButtonLabel;

        public Button notNowButton;
        public Text notNowButtonLabel;

        [Inject] public ILocalizationService localizationService { get; set; }

        public void Init()
        {
            titleLabel.text         = localizationService.Get(LocalizationKey.RATE_APP_TITLE) + " " + Application.productName + "?";
            titleRateLabel.text     = "";
            titleTeamLoveLabel.text = localizationService.Get(LocalizationKey.RATE_APP_LOVE_FROM_TEAM);
            notNowButtonLabel.text  = localizationService.Get(LocalizationKey.RATE_APP_NOT_NOW);
            rateButtonLabel.text    = localizationService.Get(LocalizationKey.RATE_APP_RATE);

            improveButtonLabel.text = localizationService.Get(LocalizationKey.RATE_APP_IMPROVE);
            likeButtonLabel.text    = localizationService.Get(LocalizationKey.RATE_APP_LIKE);
            loveButtonLabel.text    = localizationService.Get(LocalizationKey.RATE_APP_LOVE);
            tellUsButtonLabel.text  = localizationService.Get(LocalizationKey.RATE_APP_TELL);

            //dialog 2
            HideRateDlg();

        }

        public void ShowAreYouEnjoying()
        {
            gameObject.SetActive(true);
        }

        public void ShowRateDlg()
        {
            titleLabel.gameObject.SetActive(false);
            titleTeamLoveLabel.gameObject.SetActive(false);
            improveButton.gameObject.SetActive(false);
            likeButton.gameObject.SetActive(false);
            loveButton.gameObject.SetActive(false);

            tellUsButton.gameObject.SetActive(false);
            rateButton.gameObject.SetActive(false);
            notNowButton.gameObject.SetActive(true);
            titleRateLabel.gameObject.SetActive(true);
        }

        public void HideRateDlg()
        {
            tellUsButton.gameObject.SetActive(false);
            rateButton.gameObject.SetActive(false);
            notNowButton.gameObject.SetActive(false);
            titleRateLabel.gameObject.SetActive(false);
        }

        public void ShowRateUs()
        {
            ShowRateDlg();

            titleRateLabel.text = localizationService.Get(LocalizationKey.RATE_APP_SUB_TITLE_RATE);
            rateButton.gameObject.SetActive(true);

        }

        public void ShowTellUs()
        {
            ShowRateDlg();

            titleRateLabel.text = localizationService.Get(LocalizationKey.RATE_APP_SUB_TITLE_TELL);
            tellUsButton.gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}