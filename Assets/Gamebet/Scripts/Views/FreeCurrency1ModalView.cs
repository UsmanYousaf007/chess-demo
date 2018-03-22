/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Irtaza Mumtaz <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-12-09 12:52:31 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl ;

namespace TurboLabz.Gamebet
{
    public class FreeCurrency1ModalView : View
    {
        public Text titleLabel;
        public Button closeButton;

        public GameObject adsAvailableGroup;
        public GameObject adsNotAvailableGroup;

        public Button playAdButton;
        public Text playAdButtonLabel;

        public Text adsNotAvailableLabel;

        public Button okButton;
        public Text okButtonLabel;

        // View signals
        public Signal closeButtonClickedSignal = new Signal();
        public Signal playAdButtonClickedSignal = new Signal();
        public Signal okButtonClickedSignal = new Signal();

        public void Init()
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
            playAdButton.onClick.AddListener(OnPlayAdButtonClicked);
            okButton.onClick.AddListener(OnOkButtonClicked);
        }

        public void UpdateView(bool isAdAvailable)
        {
            playAdButton.interactable = true;
            titleLabel.text = "Get Free Coins";
            playAdButtonLabel.text = "PLAY AD";
            adsNotAvailableLabel.text = "No ads available. Try later.";
            okButtonLabel.text = "OK";

            adsAvailableGroup.SetActive(isAdAvailable);
            adsNotAvailableGroup.SetActive(!isAdAvailable);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnCloseButtonClicked()
        {
            closeButtonClickedSignal.Dispatch();
        }

        private void OnPlayAdButtonClicked()
        {
            playAdButtonClickedSignal.Dispatch();
            playAdButton.interactable = false;
        }

        private void OnOkButtonClicked()
        {
            okButtonClickedSignal.Dispatch();
        }
    }
}
