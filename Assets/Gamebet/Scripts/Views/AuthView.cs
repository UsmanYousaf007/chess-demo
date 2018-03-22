/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-16 19:21:55 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine.UI;

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

namespace TurboLabz.Gamebet
{
    public class AuthView : View
    {
        // TODO: Remove this injection, views cannot inject services or models
        [Inject] public ILocalizationService localizationService { get; set; }

        public Text selectLoginLabel;

        public Button authGuestButton;
        public Text authGuestButtonLabel;

        public Button authFacebookButton;
        public Text authFacebookButtonLabel;

        public Signal authGuestButtonClickedSignal = new Signal();
        public Signal authFacebookButtonClickedSignal = new Signal();

        public void Init()
        {
            // TODO: set language strings
            authGuestButton.onClick.AddListener(OnAuthGuestButtonClicked);
            authFacebookButton.onClick.AddListener(OnAuthFacebookButtonClicked);

            UpdateView();
        }

        public void UpdateView()
        {
            selectLoginLabel.text = localizationService.Get(LocalizationKey.AUTH_SELECT_LOGIN_LABEL);
            authGuestButtonLabel.text = localizationService.Get(LocalizationKey.AUTH_AUTH_GUEST_BUTTON_LABEL);
            authFacebookButtonLabel.text = localizationService.Get(LocalizationKey.AUTH_AUTH_FACEBOOK_BUTTON_LABEL);
        }

        public void OnAuthGuestButtonClicked()
        {
            authGuestButtonClickedSignal.Dispatch();
        }

        public void OnAuthFacebookButtonClicked()
        {
            authFacebookButtonClickedSignal.Dispatch();
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
