/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-03 16:10:44 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using System;
using System.Collections;
using DG.Tweening;

namespace TurboLabz.InstantGame
{
    [CLSCompliant(false)]
    public class SocialConnectionView : View
    {
        [Header("Social Connection Section")]
        public RectTransform fbBtnPos;
        public Button facebookButton;
        public Text facebookButtonLabel;
        public GameObject facebookConnectAnim;
        public Button signInWithAppleButton;
        public Text signInWithAppleLabel;
        public Text label;
        public GameObject blackBG;

        //Signals
        public Signal facebookButtonClickedSignal = new Signal();
        public Signal signInWithAppleClicked = new Signal();

        //Models
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        // Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        public void Init()
        {
            facebookButton.onClick.AddListener(OnFacebookButtonClicked);
            facebookConnectAnim.SetActive(false);
            signInWithAppleButton.onClick.AddListener(OnSignInWithAppleClicked);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void CleanUp()
        {
            if (facebookButton != null)
            {
                facebookButton.onClick.RemoveAllListeners();
            }

            if (signInWithAppleButton != null)
            {
                signInWithAppleButton.onClick.RemoveAllListeners();
            }
        }

        public void UpdateView(ProfileVO vo)
        {
            facebookButtonLabel.text = localizationService.Get(LocalizationKey.FACEBOOK_LOGIN);
            signInWithAppleLabel.text = localizationService.Get(LocalizationKey.SIGN_IN);

            var showLoginButton = !(vo.isFacebookLoggedIn || vo.isAppleSignedIn);
            facebookConnectAnim.SetActive(false);

            if (label != null)
            {
                label.text = vo.isAppleSignInSupported ? "Log in to Apple or Facebook" : "Log in with Facebook";
            }

            ChangeSocialAccountButtonsState(showLoginButton, vo.isAppleSignInSupported && showLoginButton);
        }

        public void FacebookAuthResult(AuthFacebookResultVO vo)
        {
            if (vo.isSuccessful)
            {
                ChangeSocialAccountButtonsState(false, false);
            }

             facebookConnectAnim.SetActive(false);
        }

        public void SignInWithAppleResult(AuthSignInWIthAppleResultVO vo)
        {
            if (vo.isSuccessful)
            {
                ChangeSocialAccountButtonsState(false, false);
            }

            facebookConnectAnim.SetActive(false);
        }

        public void SignOutSocialAccount()
        {
            ChangeSocialAccountButtonsState(true, true);
        }

        void ChangeSocialAccountButtonsState(bool LoginButton, bool showSignInWithAppleButton)
        {
            facebookButton.gameObject.SetActive(LoginButton);
            signInWithAppleButton.gameObject.SetActive(showSignInWithAppleButton);

            if (label != null)
            {
                label.enabled = LoginButton;
            }
            if (blackBG != null)
            {
                blackBG.gameObject.SetActive(LoginButton);
            }
        }

        public void ToggleFacebookButton(bool toggle)
        {
            facebookButton.interactable = toggle;
            signInWithAppleButton.interactable = toggle;
        }

        private void OnFacebookButtonClicked()
        {
            facebookButtonClickedSignal.Dispatch();
            facebookConnectAnim.SetActive(true);
        }

        private void OnSignInWithAppleClicked()
        {
            signInWithAppleClicked.Dispatch();
            facebookConnectAnim.SetActive(true);
        }
    }
}