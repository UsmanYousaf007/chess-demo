/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using HUF.Auth.Runtime.API;
using HUF.AuthSIWA.Runtime.API;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;

namespace TurboLabz.InstantFramework
{
    public class SignInWithAppleService : ISignInWithAppleService
    {
        //Dispatch Signals
        [Inject] public SignOutSocialAccountSignal signOutSocialAccountSignal { get; set; }

        //Services
        [Inject] public IAnalyticsService analyticsService { get; set; }

        IPromise<bool, string> promise;
        bool isInitliazed = false;

        public IPromise<bool, string> Login()
        {
            promise = new Promise<bool, string>();
            HAuthSIWA.Service.OnSignInResult += OnLogin;
            HAuthSIWA.Service.SignIn();
            return promise;
        }

        private void OnLogin(string name, AuthSignInResult result)
        {
            HAuthSIWA.Service.OnSignInResult -= OnLogin;
            promise.Dispatch(result == AuthSignInResult.Success, HAuthSIWA.Service.AuthorizationCode);
            promise = null;
        }

        public bool IsSignedIn()
        {
            Init();
            return IsSupported() && HAuthSIWA.Service.IsSignedIn;
        }

        public bool IsSupported()
        {
            return HAuthSIWA.IsSupported;
        }

        public string GetDisplayName()
        {
            return HAuthSIWA.DisplayName;
        }

        public string GetTokenId()
        {
            return HAuthSIWA.Service.AuthorizationCode;
        }

        private void Init()
        {
            if (isInitliazed)
            {
                return;
            }

            isInitliazed = true;

            if (!IsSupported())
            {
                return;
            }

            HAuthSIWA.Service.OnSignOutComplete += OnSignOut;
        }

        private void OnSignOut(string service)
        {
            if (service.Equals(AuthServiceName.SIWA))
            {
                signOutSocialAccountSignal.Dispatch();
            }
        }
    }
}