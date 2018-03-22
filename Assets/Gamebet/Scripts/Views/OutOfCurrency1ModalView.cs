/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Irtaza Mumtaz <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-12-09 14:25:01 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl ;

namespace TurboLabz.Gamebet
{
    public class OutOfCurrency1ModalView : View
    {
        public Text titleLabel;
        public Button closeButton;

        public Button buyCurrency1Button;
        public Text buyCurrency1ButtonLabel;

        public GameObject adsAvailableGroup;
        public GameObject adsNotAvailableGroup;

        public Button playAdButton;
        public Text playAdButtonLabel;

        public Text adsNotAvailableLabel;

        // View signals
        public Signal closeButtonClickedSignal = new Signal();
        public Signal buyCurrency1ButtonClickedSignal = new Signal();
        public Signal playAdButtonClickedSignal = new Signal();

        public void Init()
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
            buyCurrency1Button.onClick.AddListener(OnBuyCurrency1ButtonClicked);
            playAdButton.onClick.AddListener(OnPlayAdButtonClicked);
        }

        public void UpdateView(bool isAdAvailable)
        {
            playAdButton.interactable = true;
            titleLabel.text = "Out of Coins?";
            buyCurrency1ButtonLabel.text = "Buy Coins";
            playAdButtonLabel.text = "Watch Ad to Get Coins";
            adsNotAvailableLabel.text = "No ads available. Try later.";

            adsAvailableGroup.SetActive(isAdAvailable);
            adsNotAvailableGroup.SetActive(!isAdAvailable);

            playAdButton.interactable = isAdAvailable;
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

        private void OnBuyCurrency1ButtonClicked()
        {
            buyCurrency1ButtonClickedSignal.Dispatch();
        }

        private void OnPlayAdButtonClicked()
        {
            playAdButtonClickedSignal.Dispatch();
            playAdButton.interactable = false;
        }
    }
}
